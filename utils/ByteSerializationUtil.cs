// Project: 3DNetworkedMovement
// File: ByteSerializationUtil.cs
// 
// Author: Ethan Kisiel (ethan.a.kisiel@gmail.com)
// 
// File Created: 14 07 2025 18:07:20
// Last Modified: 14 07 2025 18:07:20
// 
// Modified By: Ethan Kisiel (ethan.a.kisiel@gmail.com>)
// 
// Copyright 2025 - 2025 Ethan Kisiel, Ethan Kisiel

using System.IO;
using Godot;

namespace Miserum.Scripts.Utils;

public static class ByteSerializationUtil
{
    // Vector2
    public static void WriteVector2(BinaryWriter writer, Vector2 vector2)
    {
        writer.Write(vector2.X);
        writer.Write(vector2.Y);
    }

    public static Vector2 ReadVector2(BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }
    
    // Vector3
    public static void WriteVector3(BinaryWriter writer, Vector3 vector3)
    {
        writer.Write(vector3.X);
        writer.Write(vector3.Y);
        writer.Write(vector3.Z);
    }

    public static Vector3 ReadVector3(BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
}