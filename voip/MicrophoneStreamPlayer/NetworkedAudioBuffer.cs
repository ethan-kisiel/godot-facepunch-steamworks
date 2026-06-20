// Project: VoiceChatTesting
// File: NetworkedAudioBuffer.cs
// 
// Author: Ethan Kisiel (ethan.a.kisiel@gmail.com)
// 
// File Created: 08 07 2025 16:07:39
// Last Modified: 08 07 2025 16:07:39
// 
// Modified By: Ethan Kisiel (ethan.a.kisiel@gmail.com>)
// 
// Copyright 2025 - 2025 Ethan Kisiel, Ethan Kisiel

using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;

namespace facepunchsteamworkstest.VOIP.MicrophoneStreamPlayer;

public struct NetworkedAudioBuffer
{
    
    public Vector2[] Frames;

    public NetworkedAudioBuffer(Vector2[] frames)
    {
        Frames = frames;
    }

    public byte[] Serialize()
    {
        var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream))
        {
            foreach (var frame in Frames)
            {
                writer.Write(frame.X);
                writer.Write(frame.Y);
            }
        }

        return memoryStream.ToArray();
    }

    public NetworkedAudioBuffer(byte[] data)
    {
        Array<Vector2> frames = new Array<Vector2>();
        
        var memoryStream = new MemoryStream(data);
        using (var reader = new BinaryReader(memoryStream))
        {
            int currentByte = 0;
            while (currentByte < data.Length)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                
                frames.Add(new Vector2(x, y));
                currentByte += 8;
            }
        }
        
        Frames = frames.ToArray();
    }
}