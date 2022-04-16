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
        public Material Material { get; }
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
                Material = new Material(file);
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
                foreach (PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> P in gltf.staticMesh.Meshes[m].Primitives)
                {
                    IReadOnlyList<(int, int, int)> Tris = P.Triangles; // e. g. 100
                    VertexCount = P.Vertices.Count;
                    FaceCount = Tris.Count; // e. g. 300
                    Triangles = new Triangle[FaceCount]; // e. g. 100

                    for (int i = 0; i < FaceCount; i++)
                    {
                        (int, int, int) Tri = Tris[i];
                        Triangles[i] = new Triangle(Tri);
                    }

                    MeshCount = 1; // the other FVF can be ignored
                    BattleforgeSubMeshes = new BattleforgeSubMesh[MeshCount];
                    BattleforgeSubMeshes[0] = new BattleforgeSubMesh(P);
                    MaterialID = 25702; // Verified
                    MaterialParameters = -86061050; // Verified
                    SthOfMaterialCore = 0; // Verified
                    BoolParamTransfer = 0; // Find out
                    Textures = new Textures(P, m);
                    Refraction = new Refraction(P); // WIP
                    Material = new Material(P);
                    LevelOfDetail = new LevelOfDetail(P);
                    EmptyString = new EmptyString(P);
                    Flow = new Flow(P);
                }
            }
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(VertexCount);
            bw.Write(FaceCount);

            for (int i = 0; i < FaceCount; i++) Triangles[i].Write(bw);

            bw.Write(MeshCount);

            BattleforgeSubMeshes[0].Write(bw, VertexCount);
            //Meshes[1].Write(bw, VertexCount);
            //Meshes[2].Write(bw, VertexCount);
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
            Material.Write(bw);
            LevelOfDetail.Write(bw);
            EmptyString.Write(bw);
            Flow.Write(bw);
        }

        internal int Size()
        {
            return 47 + FaceCount * 6 + (8 + VertexCount * 32) + Textures.Size() + Refraction.Size() + Material.Size() + LevelOfDetail.Size() + EmptyString.Size() + Flow.Size();
        }
    }
}