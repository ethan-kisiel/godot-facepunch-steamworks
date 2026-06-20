using System;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public class GodotConnectionManager: ConnectionManager
{
    public SteamMultiplayerPeer Peer { get; set; }

    public override void OnConnected(ConnectionInfo info)
    {
        Peer.SetConnectionStatus(MultiplayerPeer.ConnectionStatus.Connected);
        Peer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, 1);
    }

    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        byte[] managed = new byte[size];
        Marshal.Copy(data, managed, 0, size);
        
        GodotSteamPacket godotSteamPacket = GodotSteamPacket.Decode(managed, (ushort)channel);
        
        GD.Print($"MESSAGE TYPE: {godotSteamPacket.MessageType}");

        if (godotSteamPacket.MessageType == MessageType.NetIdHandshake)
        {
            int uniqueId = System.Buffers.Binary.BinaryPrimitives
                .ReadInt32LittleEndian(godotSteamPacket.Payload.AsSpan(0, 4));
            
            Peer.SetUniqueId(uniqueId);
            GD.Print($"RECEIVED PEER ID: {Peer.GetUniqueId()}");
        }
        else if (godotSteamPacket.MessageType == MessageType.GameMessage)
        {
            Peer.IncomingPackets.Enqueue(godotSteamPacket);
        }
    }
}