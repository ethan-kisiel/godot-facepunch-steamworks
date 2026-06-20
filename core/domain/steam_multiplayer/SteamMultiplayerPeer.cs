using System.Collections.Generic;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public partial class SteamMultiplayerPeer: MultiplayerPeerExtension
{
    public Queue<GodotSteamPacket> IncomingPackets { get; set; } = new();
    
    // Steamworks
    private GodotSocketManager _socketManager;
    private GodotConnectionManager _connectionManager;
    
    
    // PACKET TRANSFER MODE
    private SendType _sendType = SendType.Reliable;
    private TransferModeEnum _transferMode = TransferModeEnum.Reliable;
    private int _transferChannel;
    
    // Godot High level networking
    private bool _isServer;
    private int _uniqueId;
    
    private int _targetPeer;
    private int _currentPacketPeer;
    
    private ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;
    
    private void SetIsServer(bool isServer)
    {
        _isServer = isServer;
    }
    public override bool _IsServer()
    {
        return _isServer;
    }

    public override void _SetTransferMode(TransferModeEnum mode)
    {
        _transferMode = mode;

        _sendType = mode switch
        {
            TransferModeEnum.Reliable => SendType.Reliable,
            TransferModeEnum.Unreliable => SendType.Unreliable,
            TransferModeEnum.UnreliableOrdered => SendType.Unreliable,
            _ => SendType.Reliable
        };
    }
    public override TransferModeEnum _GetTransferMode()
    {
        return _transferMode;
    }
    
    public override void _SetTransferChannel(int channel)
    {
        _transferChannel = channel;
    }
    
    public override int _GetTransferChannel()
    {
        return _transferChannel;
    }
    
    // Connection status
    public void SetConnectionStatus(ConnectionStatus connectionStatus)
    {
        _connectionStatus = connectionStatus;
    }

    public override ConnectionStatus _GetConnectionStatus()
    {
        return _connectionStatus;
    }
    public override void _SetTargetPeer(int peer)
    {
        _targetPeer = peer;
    }
    
    public override int _GetAvailablePacketCount()
    {
        return IncomingPackets.Count;
    }
    
    public void SetUniqueId(int uniqueId)
    {
        _uniqueId = uniqueId;
    }
    public override int _GetUniqueId()
    {
        return _uniqueId;
    }
    
    public override int _GetPacketChannel()
    {   
        if (IncomingPackets.Count == 0)
        {
            return 0;
        }
        return IncomingPackets.Peek().Channel;
    }
    
    public override TransferModeEnum _GetPacketMode()
    {
        if (IncomingPackets.Count == 0)
        {
            return 0;
        }
        
        return IncomingPackets.Peek().SendType == SendType.Reliable ?  TransferModeEnum.Reliable : TransferModeEnum.Unreliable;
    }
    public override int _GetPacketPeer()
    {
        if (IncomingPackets.Count == 0)
        {
            return 1;
        }
        
        return IncomingPackets.Peek().From;
    }
    
    public override void _Poll()
    {
        _socketManager?.Receive();
        _connectionManager?.Receive();
    }
    
    public override Error _PutPacketScript(byte[] pBuffer) // Packet is being given to us to send over the wire
    {
        GodotSteamPacket godotSteamPacket = new GodotSteamPacket();
        
        godotSteamPacket.MessageType = MessageTypeEnum.GameMessage;
        godotSteamPacket.SendType = _sendType;
        godotSteamPacket.Payload = pBuffer;
        godotSteamPacket.Channel = (ushort) _transferChannel;
        godotSteamPacket.From = GetUniqueId();
        
        if (!_isServer)
        {
            SendMessageOverConnection(_connectionManager.Connection,  godotSteamPacket);
            return Error.Ok;
        }
        
        if (_targetPeer < 0)
        {
            foreach (var kvp in _socketManager.Peers)
            {
                if (-_targetPeer == kvp.Key)
                {
                    continue;
                }
                SendMessageOverConnection(kvp.Value, godotSteamPacket);
            }
        }
        else if (_socketManager.Peers.TryGetValue(_targetPeer, out var connection))
        {
            SendMessageOverConnection(connection, godotSteamPacket);
        }
        return Error.Ok;
    }
    public override byte[] _GetPacketScript()
    {
        if (IncomingPackets.Count == 0)
            return [];

        return IncomingPackets.Dequeue().Payload;
    }

    public void CreateServer()
    {
        SetRefuseNewConnections(false);
        SetConnectionStatus(ConnectionStatus.Connected);
        _isServer = true;
        _uniqueId = 1;
        
        _socketManager = SteamNetworkingSockets.CreateRelaySocket<GodotSocketManager>();
        _socketManager.Peer = this;
    }

    public void CreateClient(SteamId steamId)
    { 
        SetConnectionStatus(ConnectionStatus.Connecting);
        _connectionManager = SteamNetworkingSockets.ConnectRelay<GodotConnectionManager>(steamId);
        _connectionManager.Peer = this;
        _isServer = false;
    }
    
    // Max bytes allowed by steam relay
    public override int _GetMaxPacketSize()
    {
        return 1400;
    }
    
    public static Error SendMessageOverConnection(Connection connection, GodotSteamPacket godotSteamPacket)
    {
        GD.Print($"Channel: {godotSteamPacket.Channel}, SteamChannel: {(ushort)godotSteamPacket.Channel}");
        connection.SendMessage(godotSteamPacket.Encode(), godotSteamPacket.SendType, (ushort)godotSteamPacket.Channel);
        return Error.Ok;
    }
}