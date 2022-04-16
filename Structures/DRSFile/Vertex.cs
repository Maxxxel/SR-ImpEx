using SR_ImpEx.Helpers;
using System;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class Vertex
    {
        public Vector3 Position { get; }
        public Vector3 Normal { get; }
        public Vector2 Texture { get; }
        public Vertex(FileWrapper file)
        {
            Position = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            Normal = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            Texture = new Vector2(file.ReadFloat(), file.ReadFloat());
        }

        public Vertex(Vector3 p, Vector3 n, Vector2 t)
        {
            Position = p;
            Normal = n;
            Texture = t;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Position.X);
            bw.Write(Position.Y);
            bw.Write(Position.Z);
            bw.Write(Normal.X);
            bw.Write(Normal.Y);
            bw.Write(Normal.Z);
            bw.Write(Texture.X);
            bw.Write(Texture.Y);
        }
    }
}