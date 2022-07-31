using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class MaterialContainer
    {
        public int Length { get; private set; }
        public Material[] Materials { get; private set; }
        public MaterialContainer(FileWrapper file)
        {
            Length = file.ReadInt();
            Materials = new Material[Length];

            for (int i = 0; i < Length; i++)
            {
                Materials[i] = new Material(file);
            }
        }

        public MaterialContainer(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p, int meshIndex)
        {
            Length = 12;
            Materials = new Material[Length];

            for (int i = 0; i < Length; i++)
            {
                Materials[i] = new Material(p, i, meshIndex);
            }
        }

        internal int Size()
        {
            return 4 + (Length * 8);
        }
        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);

            for (int i = 0; i < Length; i++)
            {
                Materials[i].Write(bw);
            }
        }
    }
}