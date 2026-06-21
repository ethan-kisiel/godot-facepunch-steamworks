using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public class GodotSteamPacketUnsafe: IDisposable
{
    static readonly int HeaderSize = sizeof(MessageType) + sizeof(SendType) + sizeof(int);

    private IntPtr _packetData;
    private bool disposed;

    public MessageType MessageType { get; set; }
    public IntPtr Payload { get; set; }
    public int PayloadSize { get; set; }
    public int Channel { get; set; }
    public int From { get; set; }
    public SendType SendType { get; set; }

    public int Size { get; private set; }
    
    public static GodotSteamPacketUnsafe Decode(IntPtr buffer, int payloadSize, ushort channel = 0)
    {
        GodotSteamPacketUnsafe godotSteamPacket = new GodotSteamPacketUnsafe();
        godotSteamPacket.MessageType = (MessageType)Marshal.ReadByte(buffer);
        godotSteamPacket.SendType = (SendType)Marshal.ReadInt32(buffer, 1);
        godotSteamPacket.From = Marshal.ReadInt32(buffer, 5);
        godotSteamPacket.Payload = Marshal.ReadIntPtr(buffer, 9);
        godotSteamPacket.Channel = channel;
        godotSteamPacket.PayloadSize = payloadSize;

        godotSteamPacket.Size = HeaderSize + payloadSize;

        return godotSteamPacket;
    }

    public IntPtr Encode()
    {
        IntPtr buffer = Marshal.AllocHGlobal(Size);
        Marshal.WriteByte(buffer, (byte)MessageType);
        Marshal.WriteInt32(buffer, 1, (int)SendType);
        Marshal.WriteInt32(buffer, 5, From);
        Marshal.WriteIntPtr(buffer, Payload);

        if (_packetData != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_packetData);
        }

        _packetData = buffer;

        return buffer;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (_packetData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_packetData);
            }
            Payload = IntPtr.Zero;
            disposed = true;
        }
    }
    ~GodotSteamPacketUnsafe()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}