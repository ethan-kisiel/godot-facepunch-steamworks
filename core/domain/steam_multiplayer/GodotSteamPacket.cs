using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public struct GodotSteamPacket
{
    static readonly int HeaderSize = sizeof(MessageTypeEnum) + sizeof(SendType) + sizeof(int);
    
    public MessageTypeEnum MessageType;
    public byte[] Payload;
    public int Channel;
    public int From;
    public SendType SendType;
    
    public static GodotSteamPacket Decode(byte[] buffer, ushort channel = 0)
    {
        GodotSteamPacket godotSteamPacket = new GodotSteamPacket();
        godotSteamPacket.MessageType = (MessageTypeEnum)buffer[0];
        godotSteamPacket.SendType = (SendType)System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan(1, 4));
        godotSteamPacket.From = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan(5, 4));
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
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(5, 4), From);

        for (int i = 0; i < Payload.Length; i++)
        {
            buffer[i + HeaderSize] =  Payload[i];
        }

        return buffer;
    }
}