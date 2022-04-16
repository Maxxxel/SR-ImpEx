using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SR_ImpEx
{
    public class StaticMesh
    {
        public MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>[] Meshes { get; }
        public MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty> UnifiedMesh { get; }

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
                Structures.Texture ColorMap = Mesh.Meshes[i].Textures.Texture.Where(t => t.Name.Contains("_col")).FirstOrDefault();
                Structures.Texture NormalMap = Mesh.Meshes[i].Textures.Texture.Where(t => t.Name.Contains("_nor")).FirstOrDefault();

                string CurrentPath = AppContext.BaseDirectory;
                if (!Directory.Exists(Path.Combine(CurrentPath, "GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentPath, "GLTF_Exports"));

                if (ColorMap != null)
                {
                    ImageConverter.DDSToPNG(SourcePath + ColorMap.Name, CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name);
                    Textures.WithChannelImage(KnownChannel.BaseColor, CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name.Replace(".dds", "") + ".png");
                }

                if (NormalMap != null)
                {
                    ImageConverter.DDSToPNG(SourcePath + NormalMap.Name, CurrentPath + "/GLTF_Exports/_temporary_" + NormalMap.Name);
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
        public StaticMesh(ModelRoot root)
        {
            // Meshes are the normal Meshes
            Meshes = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>[root.LogicalMeshes.Count];
            // Unified Meshes are the combined Meshes with no materials, used for OBB and CGeo
            UnifiedMesh = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>();
            // Construct one primitive for the unified Mesh
            PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> UnifiedPrimitive = UnifiedMesh.UsePrimitive(MaterialBuilder.CreateDefault());
            // Loop all Meshes
            for (int meshIndex = 0; meshIndex < root.LogicalMeshes.Count; meshIndex++)
            {
                // Create a new MeshBuilder for every Mesh in the looop
                MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> Mesh = Meshes[meshIndex] = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                // Get the partialMesh data
                Mesh partialMesh = root.LogicalMeshes[meshIndex];
                // Loop the partial primitives
                for (int primitiveIndex = 0; primitiveIndex < partialMesh.Primitives.Count; primitiveIndex++)
                {
                    // Get the primitive
                    MeshPrimitive partialPrimitive = partialMesh.Primitives[primitiveIndex];
                    // Get the material
                    SharpGLTF.Schema2.Material partialMaterial = partialPrimitive.Material;
                    // Create a new MaterialBuilder for the to-built-Mesh
                    MaterialBuilder MeshMaterial = new MaterialBuilder();
                    // Copy only the supported Channels to the MaterialBuilder
                    partialMaterial.CopyChannelsTo(MeshMaterial, new string[2] { "BaseColor", "Normal" });
                    // Create a new primitive for the to-built-Mesh
                    PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> Primitive = Mesh.UsePrimitive(MeshMaterial);
                    // Try to get the Vertex Data
                    partialPrimitive.VertexAccessors.TryGetValue("POSITION", out Accessor positionAccessor);
                    partialPrimitive.VertexAccessors.TryGetValue("NORMAL", out Accessor normalAccessor);
                    partialPrimitive.VertexAccessors.TryGetValue("TEXCOORD_0", out Accessor textureAccessor);

                    IList<Vector3> vertexPositions = positionAccessor.AsVector3Array();

                    IList<Vector3> vertexNormals = null;
                    if (normalAccessor != null && normalAccessor.Count > 0)
                    {
                        vertexNormals = normalAccessor.AsVector3Array();
                    }
                    else
                    {
                        MainWindow.LogMessage($"[WARN] Mesh #{meshIndex} has no normal coordinates. They will be set to Vector3(0, 0, 0) for compatibility.");
                    }

                    IList<Vector2> vertexTextures = null;
                    if (textureAccessor != null && textureAccessor.Count > 0)
                    {
                        vertexTextures = textureAccessor.AsVector2Array();
                    }
                    else
                    {
                        MainWindow.LogMessage($"[WARN] Mesh #{meshIndex} has no texture coordinates. They will be set to Vector2(0, 0) for compatibility.");
                    }

                    // Get the Triangles of the partialMesh and add them to the OBB/CGeo and new SubMesh
                    IEnumerable<(int, int, int)> Triangles = partialPrimitive.GetTriangleIndices();
                    foreach ((int, int, int) Triangle in Triangles)
                    {
                        Vector3 A = vertexPositions[Triangle.Item1];
                        Vector3 B = vertexPositions[Triangle.Item2];
                        Vector3 C = vertexPositions[Triangle.Item3];

                        Vector3 NA = Vector3.Zero;
                        Vector3 NB = Vector3.Zero;
                        Vector3 NC = Vector3.Zero;

                        Vector2 TA = Vector2.Zero;
                        Vector2 TB = Vector2.Zero;
                        Vector2 TC = Vector2.Zero;

                        if (vertexNormals != null)
                        {
                            NA = vertexNormals[Triangle.Item1];
                            NB = vertexNormals[Triangle.Item2];
                            NC = vertexNormals[Triangle.Item3];
                        }

                        if (vertexTextures != null)
                        {
                            TA = vertexTextures[Triangle.Item1];
                            TB = vertexTextures[Triangle.Item2];
                            TC = vertexTextures[Triangle.Item3];
                        }

                        VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> V0 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((A, NA), new VertexTexture1(TA));
                        VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> V1 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((B, NB), new VertexTexture1(TB));
                        VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> V2 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>((C, NC), new VertexTexture1(TC));
                        Primitive.AddTriangle(V0, V1, V2);

                        VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty> V00 = new VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty>(A);
                        VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty> V01 = new VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty>(B);
                        VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty> V02 = new VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty>(C);
                        UnifiedPrimitive.AddTriangle(V00, V01, V02);
                    }
                }
            }

            MainWindow.LogMessage("[INFO] StaticMesh(es) created!");
        }
    }
}