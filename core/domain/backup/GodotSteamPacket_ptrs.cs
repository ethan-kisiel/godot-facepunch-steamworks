using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public struct GodotSteamPacket_ptrs
{
    static readonly int HeaderSize = sizeof(MessageTypeEnum) + sizeof(SendType) + sizeof(int);
    
    public MessageTypeEnum MessageType;
    public IntPtr Payload;
    public int Channel;
    public int From;
    public SendType SendType;
    
    public static GodotSteamPacket_ptrs Decode(IntPtr buffer, ushort channel = 0)
    {
        GodotSteamPacket_ptrs godotSteamPacket = new GodotSteamPacket_ptrs();
        godotSteamPacket.MessageType = (MessageTypeEnum)Marshal.ReadByte(buffer);
        godotSteamPacket.SendType = (SendType)Marshal.ReadInt32(buffer, 1);
        godotSteamPacket.From = Marshal.ReadInt32(buffer, 5);
        godotSteamPacket.Payload = Marshal.ReadIntPtr(buffer, 9);
        godotSteamPacket.Channel = channel;

        return godotSteamPacket;
    }

    public byte[] Encode()
    {

        //int bufferSize = HeaderSize + Payload.Length;
        //byte[] buffer = new byte[bufferSize];

        //buffer[0] = (byte)MessageType;
        //System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(1, 4), (int)SendType);
        //System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(5, 4), From);

        //for (int i = 0; i < Payload.Length; i++)
        //{
        //    buffer[i + HeaderSize] =  Payload[i];
        //}

        //return buffer;
        return [];
    }
}