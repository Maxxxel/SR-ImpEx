using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class LevelOfDetail
    {
        public int Length { get; }
        public int LODLevel { get; }
        public LevelOfDetail(FileWrapper file)
        {
            Length = file.ReadInt();

            if (Length == 1)
            {
                LODLevel = file.ReadInt();
            }
        }

        public LevelOfDetail(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p)
        {
            Length = 1; // WIP

            if (Length == 1)
            {
                LODLevel = 0; // We need to find out what it changes ingame
            }
        }

        internal int Size()
        {
            return 4 + (Length * 4);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);

            if (Length == 1)
            {
                bw.Write(LODLevel);
            }
        }
    }
}