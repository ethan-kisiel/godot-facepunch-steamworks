// Project: VoiceChatTesting
// File: CompressionUtil.cs
// 
// Author: Ethan Kisiel (ethan.a.kisiel@gmail.com)
// 
// File Created: 08 07 2025 16:07:26
// Last Modified: 08 07 2025 16:07:26
// 
// Modified By: Ethan Kisiel (ethan.a.kisiel@gmail.com>)
// 
// Copyright 2025 - 2025 Ethan Kisiel, Ethan Kisiel


using System;
using K4os.Compression.LZ4;
using Array = System.Array;

namespace Miserum.Scripts.Utils;

public static class CompressionUtil
{
    public static byte[] Compress(byte[] data)
    {
        // Estimate max compressed length
        int maxCompressedLength = LZ4Codec.MaximumOutputSize(data.Length);
        byte[] compressed =  new byte[maxCompressedLength];

        // Perform compression
        int compressedLength = LZ4Codec.Encode(data, 0, data.Length, compressed, 0, compressed.Length);

        // Trim unused bytes
        Array.Resize(ref compressed, compressedLength);
        
        return compressed;
    }

    public static byte[] Decompress(byte[] compressedData, int originalLength)
    {
        byte[] result = new byte[originalLength];
        int decodedLength = LZ4Codec.Decode(compressedData, 0, compressedData.Length, result, 0, originalLength);
        
        if (decodedLength != originalLength)
            throw new InvalidOperationException("Decompressed size mismatch.");

        return result;
    }
}