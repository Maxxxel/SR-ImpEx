using SR_ImpEx.Helpers;

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
    }
}