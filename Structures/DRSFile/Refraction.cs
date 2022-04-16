using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Refraction
    {
        public int Length { get; }
        public int Identifier { get; }
        public float R { get; }
        public float G { get; }
        public float B { get; }
        public Refraction(FileWrapper file)
        {
            Length = file.ReadInt(); // 0 or 1

            if (Length == 1)
            {
                Identifier = file.ReadInt(); // 1668510769
                R = file.ReadFloat();
                G = file.ReadFloat();
                B = file.ReadFloat();
            }
        }

        public Refraction(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p)
        {
            Length = 0; // WIP

            if (Length == 1)
            {
                Identifier = 1668510769;
                R = 0; // WIP
                G = 0; // WIP
                B = 0; // WIP
            }
        }

        internal int Size()
        {
            return 4 + (Length * 16);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);

            if (Length == 1)
            {
                bw.Write(Identifier);
                bw.Write(R);
                bw.Write(G);
                bw.Write(B);
            }
        }
    }
}