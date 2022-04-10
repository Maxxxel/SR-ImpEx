using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SR_ImpEx.Structures;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.DRSFile;
using SR_ImpEx.Structures.SKAFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.GLTFFile
{
    public class SkinnedMesh
    {
        public MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4> Mesh { get; private set; }
        public GLTFSkeleton Skeleton { get; }
        public SkinnedMesh(CGeoMesh cGeoMesh, CDspMeshFile cDspMeshFile, CSkSkinInfo cSkSkinInfo, CSkSkeleton cSkSkeleton, HashSet<string> animations, string location)
        {
            Mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>("Main_Mesh");
            int TotalVertices = 0;
            int vOff = 0;
            int offset = 0;

            for (int s = 0; s < cDspMeshFile.Meshes.Length; s++)
            {
                for (int i = 0; i < cDspMeshFile.Meshes[s].VertexCount; i++)
                {
                    TotalVertices++;
                }
            }

            BoneWeight[] BoneWeights = new BoneWeight[TotalVertices];

            for (int s = 0; s < cDspMeshFile.Meshes.Length; s++)
            {
                BattleforgeSubMesh Mesh = cDspMeshFile.Meshes[s].BattleforgeSubMeshes[0];

                for (int i = 0; i < cSkSkinInfo.VertexWeights.Length; i++)
                {
                    int jj = 0;
                    VertexWeight VertexWeight = cSkSkinInfo.VertexWeights[i];
                    Vector3 V = new Vector3(cGeoMesh.Vertices[i].X, cGeoMesh.Vertices[i].Y, cGeoMesh.Vertices[i].Z);

                    for (int j = vOff; j < vOff + Mesh.Vertices.Length; j++)
                    {
                        if (V == Mesh.Vertices[jj].Position)
                        {
                            BoneWeight boneWeight = new BoneWeight(
                                VertexWeight.BoneIndices[0],
                                VertexWeight.BoneIndices[1],
                                VertexWeight.BoneIndices[2],
                                VertexWeight.BoneIndices[3],
                                VertexWeight.Weights[0],
                                VertexWeight.Weights[1],
                                VertexWeight.Weights[2],
                                VertexWeight.Weights[3]
                            );

                            BoneWeights[j] = boneWeight;
                        }

                        jj++;
                    }
                }

                vOff += Mesh.Vertices.Length;
            }

            for (int s = 0; s < cDspMeshFile.Meshes.Length; s++)
            {
                MaterialBuilder Textures = new MaterialBuilder("Texture");
                Texture ColorMap = cDspMeshFile.Meshes[s].Textures.Texture.Where(tx => tx.Name.Contains("_col")).Single();
                Texture NormalMap = cDspMeshFile.Meshes[s].Textures.Texture.Where(tx => tx.Name.Contains("_nor")).Single();

                string CurrentPath = Directory.GetCurrentDirectory();
                if (!Directory.Exists(Path.Combine(CurrentPath, "GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentPath, "GLTF_Exports"));

                if (ColorMap != null)
                {
                    ImageConverter.DDSToPNG(location + ColorMap.Name + ".dds", CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name);
                    Textures.WithChannelImage(KnownChannel.BaseColor, CurrentPath + "/GLTF_Exports/_temporary_" + ColorMap.Name.Replace(".dds", "") + ".png");
                }

                if (NormalMap != null)
                {
                    ImageConverter.DDSToPNG(location + NormalMap.Name + ".dds", CurrentPath + "/GLTF_Exports/_temporary_" + NormalMap.Name);
                    Textures.WithChannelImage(KnownChannel.Normal, CurrentPath + "/GLTF_Exports/_temporary_" + NormalMap.Name.Replace(".dds", "") + ".png");
                }

                PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexJoints4> Primitive = Mesh.UsePrimitive(Textures);

                if (s > 0) offset = cDspMeshFile.Meshes[s - 1].VertexCount;

                for (int i = 0; i < cDspMeshFile.Meshes[s].FaceCount; i++)
                {
                    BattleforgeMesh Mesh = cDspMeshFile.Meshes[s];

                    short[] FaceIndices = Mesh.Triangles[i].Indices;
                    short FaceIndex0 = FaceIndices[0];
                    short FaceIndex1 = FaceIndices[1];
                    short FaceIndex2 = FaceIndices[2];

                    Vertex[] Vertices = Mesh.BattleforgeSubMeshes[0].Vertices;
                    Vertex Vertex0 = Vertices[FaceIndex0];
                    Vertex Vertex1 = Vertices[FaceIndex1];
                    Vertex Vertex2 = Vertices[FaceIndex2];

                    BoneWeight Joint0 = BoneWeights[offset + FaceIndex0];
                    BoneWeight Joint1 = BoneWeights[offset + FaceIndex1];
                    BoneWeight Joint2 = BoneWeights[offset + FaceIndex2];

                    (int, float)[] Binds0 = new[]
                    {
                            (Joint0.boneIndex0, Joint0.weight0),
                            (Joint0.boneIndex1, Joint0.weight1),
                            (Joint0.boneIndex2, Joint0.weight2),
                            (Joint0.boneIndex3, Joint0.weight3)
                        };

                    (int, float)[] Binds1 = new[]
                    {
                            (Joint1.boneIndex0, Joint1.weight0),
                            (Joint1.boneIndex1, Joint1.weight1),
                            (Joint1.boneIndex2, Joint1.weight2),
                            (Joint1.boneIndex3, Joint1.weight3)
                        };

                    (int, float)[] Binds2 = new[]
                    {
                            (Joint2.boneIndex0, Joint2.weight0),
                            (Joint2.boneIndex1, Joint2.weight1),
                            (Joint2.boneIndex2, Joint2.weight2),
                            (Joint2.boneIndex3, Joint2.weight3)
                        };

                    Primitive.AddTriangle(
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>((Vertex0.Position, Vertex0.Normal), new VertexTexture1(Vertex0.Texture), Binds0),
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>((Vertex1.Position, Vertex1.Normal), new VertexTexture1(Vertex1.Texture), Binds1),
                        new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>((Vertex2.Position, Vertex2.Normal), new VertexTexture1(Vertex2.Texture), Binds2)
                    );
                }
            }

            Skeleton = new GLTFSkeleton(cSkSkeleton);

            if (animations != null)
            {
                foreach (string A in animations)
                {
                    FileWrapper s = new FileWrapper(location + "/" + A);
                    SKA skaFile = new SKA(s, A);

                    if (skaFile != null)
                    {
                        float animationLength = skaFile.TotalTime;

                        for (int b = 0; b < cSkSkeleton.BoneCount; b++)
                        {
                            NodeBuilder boneNode = Skeleton.Binds[b];

                            for (int i = 0; i < skaFile.Headers.Length; i++)
                            {
                                Bone bone = cSkSkeleton.Bones[cSkSkeleton.BoneMatrices[b].BoneVertices[1].ParentReference];

                                if (bone.Version == skaFile.Headers[i].BoneVersion)
                                {
                                    int tick = skaFile.Headers[i].StartFrame;
                                    int interval = skaFile.Headers[i].Length;
                                    int type = skaFile.Headers[i].Type;

                                    Dictionary<float, Vector3> Anim = new Dictionary<float, Vector3>();
                                    Dictionary<float, Quaternion> Rota = new Dictionary<float, Quaternion>();

                                    for (int k = 0; k < interval; k++)
                                    {
                                        float keyTime = skaFile.Times[tick + k] * animationLength;
                                        Keyframe keyframe = skaFile.Keyframes[tick + k];

                                        if (type == 0) // Position
                                        {
                                            Anim.Add(keyTime, new Vector3(keyframe.X, keyframe.Y, keyframe.Z));
                                        }
                                        else // Rotation
                                        {
                                            Rota.Add(keyTime, new Quaternion(keyframe.X, keyframe.Y, keyframe.Z, -keyframe.W)); // negative .W
                                        }
                                    }

                                    if (type == 0)
                                    {
                                        boneNode.WithLocalTranslation(skaFile.Name, Anim);
                                    }
                                    else
                                    {
                                        boneNode.WithLocalRotation(skaFile.Name, Rota);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
