using SR_ImpEx.Helpers;
using SR_ImpEx.Structures;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class CGeoMesh
    {
        public CGeoMesh(FileWrapper file)
        {
            Magic = file.ReadInt(); // 1
            IndexCount = file.ReadInt();

            if (IndexCount > 0)
                Triangles = new Triangle[IndexCount / 3];

            for (int f = 0; f < IndexCount / 3; f++)
                Triangles[f] = new Triangle(file);

            VertexCount = file.ReadInt();

            if (VertexCount > 0)
            {
                Vertices = new Vector4[VertexCount];

                for (int v = 0; v < VertexCount; v++)
                    Vertices[v] = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            }
        }

        public int Magic { get; }
        public int IndexCount { get; }
        public Triangle[] Triangles { get; }
        public int VertexCount { get; }
        public Vector4[] Vertices { get; }
    }
}