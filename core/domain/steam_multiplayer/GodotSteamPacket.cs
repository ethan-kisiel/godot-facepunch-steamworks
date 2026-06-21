using System;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public struct GodotSteamPacket
{
    static readonly int HeaderSize = sizeof(MessageType) + sizeof(SendType) + sizeof(int);
    
    public MessageType MessageType { get; set; }
    public byte[] Payload { get; set; }
    public int Channel { get; set; }
    public uint From { get; set; }
    public SendType SendType { get; set; }
    
    public static GodotSteamPacket Decode(byte[] buffer, ushort channel = 0)
    {
        GodotSteamPacket godotSteamPacket = new GodotSteamPacket();
        godotSteamPacket.MessageType = (MessageType)buffer[0];
        godotSteamPacket.SendType = (SendType)System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan(1, 4));
        godotSteamPacket.From = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(5, 4));
        godotSteamPacket.Payload = buffer[HeaderSize..];
        godotSteamPacket.Channel = channel;

        return godotSteamPacket;
    }

    public byte[] Encode()
    {
        
        int bufferSize = HeaderSize + Payload.Length;
        byte[] buffer = new byte[bufferSize];

        buffer[0] = (byte)MessageType;
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(1, 4), (int)SendType);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(5, 4), From);

        for (int i = 0; i < Payload.Length; i++)
        {
            buffer[i + HeaderSize] =  Payload[i];
        }

        return buffer;
    }
}