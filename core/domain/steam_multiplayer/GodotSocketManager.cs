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
    private int _nextPeer = 2;

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
        Peers.Add(_nextPeer, connection);
        ReversePeers.Add(connection, _nextPeer);

        GodotSteamPacket godotSteamPacket = new GodotSteamPacket();
        
        godotSteamPacket.MessageType = MessageTypeEnum.NetIdHandshake;
        godotSteamPacket.SendType = SendType.Reliable;
        godotSteamPacket.From = 1;
        
        var buffer = new byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(0, 4), _nextPeer);

        godotSteamPacket.Payload = buffer;

        SteamMultiplayerPeer.SendMessageOverConnection(connection, godotSteamPacket);
        
        GD.Print($"PEER CONNECTED: {info.Identity.SteamId}, {_nextPeer}");
        Peer.EmitSignal(MultiplayerPeer.SignalName.PeerConnected, _nextPeer);
        _nextPeer++;
        
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
        GodotSteamPacket godotSteamPacket = GodotSteamPacket.Decode(managed, (ushort)channel);
        
        godotSteamPacket.From = reversePeerId;
        
        GD.Print($"Channel: {channel}, Lane: {godotSteamPacket.Channel}, SendType: {godotSteamPacket.SendType}");

        if (reversePeerId == 0)
        {
            return;
        }
        
        Peer.IncomingPackets.Enqueue(godotSteamPacket);
    }
}