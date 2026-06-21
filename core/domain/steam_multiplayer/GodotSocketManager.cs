using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public class GodotSocketManager : SocketManager
{
    // Sockets
    public Dictionary<int, Connection> Peers { get; set; } = new();
    public Dictionary<Connection, int> ReversePeers { get; set; } = new();
    
    public SteamMultiplayerPeer Peer { get; set; }
    
    public override void OnConnecting(Connection connection, ConnectionInfo info)
    {
        base.OnConnecting(connection, info);
        connection.Accept();
        GD.Print($"ACCEPTED PEER: {info.Identity.SteamId}");
    }
    
    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
        var peerId = (int)(info.Identity.SteamId.Value & 0xffffffff);
        Peers.Add(peerId, connection);
        ReversePeers.Add(connection, peerId);

        GodotSteamPacket godotSteamPacket = new GodotSteamPacket();
        
        godotSteamPacket.MessageType = MessageType.NetIdHandshake;
        godotSteamPacket.SendType = SendType.Reliable;
        godotSteamPacket.From = 1;
        
        godotSteamPacket.Payload = [];

        SteamMultiplayerPeer.SendMessageOverConnection(connection, godotSteamPacket);
      

        GD.Print($"PEER CONNECTED: {peerId}");
        Peer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, peerId);
        
        base.OnConnected(connection, info);
    }
    
    public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        ReversePeers.TryGetValue(connection, out var  reversePeerId);
        Peers.Remove(reversePeerId);
        ReversePeers.Remove(connection);
        
        GD.Print($"PEER DISCONNECTED: {info.Identity.SteamId}, {reversePeerId}");
        Peer.EmitSignal(MultiplayerPeer.SignalName.PeerDisconnected, reversePeerId);
        base.OnDisconnected(connection, info);
    }
    
    public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        byte[] managed = new byte[size];
        Marshal.Copy(data, managed, 0, size);
        
        ReversePeers.TryGetValue(connection, out var  reversePeerId);
        var normalizedChannel = (ushort)Math.Clamp(channel, 0, ushort.MaxValue);
        GodotSteamPacket godotSteamPacket = GodotSteamPacket.Decode(managed, normalizedChannel);
        
        godotSteamPacket.From = reversePeerId;
        
        GD.Print($"Channel: {channel}, Lane: {godotSteamPacket.Channel}, SendType: {godotSteamPacket.SendType}");

        if (reversePeerId == 0)
        {
            return;
        }
        
        Peer.IncomingPackets.Enqueue(godotSteamPacket);
    }
}