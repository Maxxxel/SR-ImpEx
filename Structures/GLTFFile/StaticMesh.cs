using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures;
using System.IO;
using System.Linq;

namespace SR_ImpEx
{
    public class StaticMesh
    {
        public MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>[] Meshes { get; }
        public StaticMesh(CDspMeshFile Mesh, string SourcePath)
        {
            Meshes = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>[Mesh.MeshCount];
            int TotalVertices = 0;
            int Offset = 0;

            for (int i = 0; i < Mesh.MeshCount; i++) for (int j = 0; j < Mesh.Meshes[i].VertexCount; j++) TotalVertices++;

            for (int i = 0; i < Mesh.MeshCount; i++)
            {
                Meshes[i] = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>($"Mesh #{i}");
                MaterialBuilder Textures = new MaterialBuilder("Texture");
                Texture ColorMap = Mesh.Meshes[i].Textures.Texture.Where(t => t.Name.Contains("_col")).FirstOrDefault();
                Texture NormalMap = Mesh.Meshes[i].Textures.Texture.Where(t => t.Name.Contains("_nor")).FirstOrDefault();

                string CurrentPath = Directory.GetCurrentDirectory();
                if (!Directory.Exists(Path.Combine(CurrentPath, "GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentPath, "GLTF_Exports"));

                if (ColorMap != null)
                {
                    ImageConverter.DDSToPNG(SourcePath + ColorMap.Name + ".dds", CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name);
                    Textures.WithChannelImage(KnownChannel.BaseColor, CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name.Replace(".dds", "") + ".png");
                }

                if (NormalMap != null)
                {
                    ImageConverter.DDSToPNG(SourcePath + NormalMap.Name + ".dds", CurrentPath + "/GLTF_Exports/_temporary_" + NormalMap.Name);
                    Textures.WithChannelImage(KnownChannel.Normal, CurrentPath + "/GLTF_Exports/_temporary_" + NormalMap.Name.Replace(".dds", "") + ".png");
                }

                PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> Primitive = Meshes[i].UsePrimitive(Textures);

                if (i > 0) Offset = Mesh.Meshes[i - 1].VertexCount;

                for (int j = 0; j < Mesh.Meshes[i].FaceCount; j++)
                {
                    BattleforgeMesh BFMesh = Mesh.Meshes[i];

                    short[] FaceIndices = BFMesh.Triangles[j].Indices;
                    short FaceIndex0 = FaceIndices[0];
                    short FaceIndex1 = FaceIndices[1];
                    short FaceIndex2 = FaceIndices[2];

                    Vertex[] Vertices = BFMesh.BattleforgeSubMeshes[0].Vertices;
                    Vertex Vertex0 = Vertices[FaceIndex0];
                    Vertex Vertex1 = Vertices[FaceIndex1];
                    Vertex Vertex2 = Vertices[FaceIndex2];

                    Primitive.AddTriangle(
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((Vertex0.Position, Vertex0.Normal), new VertexTexture1(Vertex0.Texture)),
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((Vertex1.Position, Vertex1.Normal), new VertexTexture1(Vertex1.Texture)),
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((Vertex2.Position, Vertex2.Normal), new VertexTexture1(Vertex2.Texture))
                    );
                }
            }
        }
    }
}