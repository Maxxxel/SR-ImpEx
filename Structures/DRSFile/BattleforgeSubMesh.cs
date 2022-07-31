using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using SR_ImpEx.Helpers;
using System;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace SR_ImpEx.Structures
{
    public class BattleforgeSubMesh
    {
        private int Revision { get; }
        private int VertexSize { get; }
        public Vertex[] Vertices { get; }
        public BattleforgeSubMesh(FileWrapper file, int vertexCount)
        {
            Revision = file.ReadInt();
            VertexSize = file.ReadInt();

            if (Revision == 133121)
            {
                Vertices = new Vertex[vertexCount];
                for (int i = 0; i < vertexCount; i++) Vertices[i] = new Vertex(file);
            }
            else
            {
                // We dont need the old shader stuff...
                file.Seek(file.GetFilePosition() + (vertexCount * VertexSize));
            }
        }
        public BattleforgeSubMesh(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p, Matrix4x4 transform)
        {
            Revision = 133121;
            VertexSize = 32;
            Vertices = new Vertex[p.Vertices.Count];

            for (int i = 0; i < p.Vertices.Count; i++)
            {
                VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> V = p.Vertices[i];
                V = V.TransformedBy(transform);
                Vector3 P = V.Geometry.Position;
                Vector3 N = V.Geometry.Normal;
                Vector2 T = V.Material.TexCoord;
                Vertices[i] = new Vertex(P, N, T);
            }
        }
        internal void Write(BinaryWriter bw, int vertexCount)
        {
            bw.Write(Revision);
            bw.Write(VertexSize);

            for (int i = 0; i < vertexCount; i++)
            {
                Vertices[i].Write(bw);
            }
        }
    }
}