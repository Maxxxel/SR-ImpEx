using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class EmptyString
    {
        public int Length { get; }
        public byte[] Uk { get; }
        public EmptyString(FileWrapper file)
        {
            Length = file.ReadInt();
            Uk = file.ReadBytes(-1, Length * 2);
        }

        public EmptyString(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p)
        {
            Length = 0; // WIP

            if (Length != 0)
            {
                // I dont really know....
            }
        }

        internal int Size()
        {
            return 4;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);
        }
    }
}