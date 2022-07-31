using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class BattleforgeMesh
    {
        public int VertexCount { get; set; }
        public int FaceCount { get; set; }
        public Triangle[] Triangles { get; }
        public byte MeshCount { get; }
        public BattleforgeSubMesh[] BattleforgeSubMeshes { get; }
        public Vector3 BoundingBoxLowerLeftCorner2 { get; set; }
        public Vector3 BoundingBoxUpperRightCorner2 { get; set; }
        public short MaterialID { get; }
        public int MaterialParameters { get; }
        public int SthOfMaterialCore { get; }
        public int BoolParamTransfer { get; }
        public Textures Textures { get; }
        public Refraction Refraction { get; }
        public MaterialContainer Materials { get; }
        public LevelOfDetail LevelOfDetail { get; }
        public EmptyString EmptyString { get; }
        public Flow Flow { get; }
        public BattleforgeMesh(FileWrapper file)
        {
            VertexCount = file.ReadInt();
            FaceCount = file.ReadInt();
            Triangles = new Triangle[FaceCount];

            for (int i = 0; i < FaceCount; i++) Triangles[i] = new Triangle(file);

            MeshCount = file.ReadByte();
            BattleforgeSubMeshes = new BattleforgeSubMesh[MeshCount];

            for (int i = 0; i < MeshCount; i++) BattleforgeSubMeshes[i] = new BattleforgeSubMesh(file, VertexCount);

            BoundingBoxLowerLeftCorner2 = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat()); //seems like same BB as before
            BoundingBoxUpperRightCorner2 = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            MaterialID = file.ReadShort();
            MaterialParameters = file.ReadInt();

            if (MaterialParameters == -86061050)
            {
                SthOfMaterialCore = file.ReadInt();
                BoolParamTransfer = file.ReadInt();
                Textures = new Textures(file);
                Refraction = new Refraction(file);
                Materials = new MaterialContainer(file);
                LevelOfDetail = new LevelOfDetail(file);
                EmptyString = new EmptyString(file);
                Flow = new Flow(file);
            }
            else
            {
                // Unsupported Material
            }
        }
        public BattleforgeMesh(GLTF gltf, int m)
        {
            if (gltf.SkinnedModel)
            {
                // WIP
            }
            else
            {
                MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> CurrentMesh = gltf.staticMesh.Meshes[m];

                MainWindow.LogMessage($"[INFO] Exporting Mesh #{m} with {CurrentMesh.Primitives.Count} Primitives");
                int debugcounter = 0;

                foreach (PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> P in CurrentMesh.Primitives)
                {
                    IReadOnlyList<(int, int, int)> Tris = P.Triangles;
                    VertexCount = P.Vertices.Count;
                    FaceCount = Tris.Count;
                    Triangles = new Triangle[FaceCount];

                    for (int i = 0; i < FaceCount; i++)
                    {
                        (int, int, int) Tri = Tris[i];
                        Triangles[i] = new Triangle(Tri);
                    }

                    MainWindow.LogMessage($"[INFO] Mesh #{m} Primitive #{debugcounter} Vertices: {VertexCount}, Triangles: {FaceCount}");

                    MeshCount = 1; // the other FVF can be ignored
                    BattleforgeSubMeshes = new BattleforgeSubMesh[MeshCount];
                    Matrix4x4 Transform = new Matrix4x4();
                    gltf.staticMesh.MeshMatrixHashes.TryGetValue(CurrentMesh.GetHashCode(), out Transform);

                    MainWindow.LogMessage("[INFO] Creating Submeshes...");
                    BattleforgeSubMeshes[0] = new BattleforgeSubMesh(P, Transform);
                    MaterialID = 25702; // Verified
                    MaterialParameters = -86061050; // Verified
                    SthOfMaterialCore = 0; // Verified
                    MainWindow.LogMessage("[INFO] Creating Textures...");
                    Textures = new Textures(P, m);
                    Refraction = new Refraction(P); // WIP
                    Materials = new MaterialContainer(P, m);
                    LevelOfDetail = new LevelOfDetail(P);
                    EmptyString = new EmptyString(P);
                    Flow = new Flow(P);

                    long boolParameter = 0;

                    // DecalMode 
                    //if (CurrentMesh.Extras.ToJson().Contains("DECAL"))
                    //{
                    //    boolParameter += 10;
                    //}
                    // UseParameterMap 
                    if (Textures.HasParameterMap)
                    {
                        boolParameter += 10000000000000000;
                    }
                    // UseNormalMap
                    if (Textures.HasNormalMap)
                    {
                        boolParameter += 100000000000000000;
                    }

                    BoolParamTransfer = Convert.ToInt32(boolParameter.ToString(), 2);

                    debugcounter++;
                }
            }
        }
        internal void Write(BinaryWriter bw)
        {
            bw.Write(VertexCount);
            bw.Write(FaceCount);

            for (int i = 0; i < FaceCount; i++) Triangles[i].Write(bw);

            bw.Write(MeshCount);

            BattleforgeSubMeshes[0].Write(bw, VertexCount); // Its only one
            bw.Write(BoundingBoxLowerLeftCorner2.X);
            bw.Write(BoundingBoxLowerLeftCorner2.Y);
            bw.Write(BoundingBoxLowerLeftCorner2.Z);
            bw.Write(BoundingBoxUpperRightCorner2.X);
            bw.Write(BoundingBoxUpperRightCorner2.Y);
            bw.Write(BoundingBoxUpperRightCorner2.Z);
            bw.Write(MaterialID);
            bw.Write(MaterialParameters);
            bw.Write(SthOfMaterialCore);
            bw.Write(BoolParamTransfer);
            Textures.Write(bw);
            Refraction.Write(bw);
            Materials.Write(bw);
            LevelOfDetail.Write(bw);
            EmptyString.Write(bw);
            Flow.Write(bw);
        }
        internal int Size()
        {
            return 47 + FaceCount * 6 + (8 + VertexCount * 32) + Textures.Size() + Refraction.Size() + Materials.Size() + LevelOfDetail.Size() + EmptyString.Size() + Flow.Size();
        }
    }
}