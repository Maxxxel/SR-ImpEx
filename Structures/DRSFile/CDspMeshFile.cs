using SharpGLTF.Schema2;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class CDspMeshFile
    {
        public int Magic { get; }
        public int Zero { get; }
        public int MeshCount { get; }
        public Vector3 BoundingBoxLowerLeftCorner1 { get; private set; }
        public Vector3 BoundingBoxUpperRightCorner1 { get; private set; }
        public BattleforgeMesh[] Meshes { get; }
        public Vector4[] SomePoints { get; }
        public CDspMeshFile(FileWrapper file)
        {
            Magic = file.ReadInt();

            if (Magic == 1314189598)
            {
                Zero = file.ReadInt();
                MeshCount = file.ReadInt();
                BoundingBoxLowerLeftCorner1 = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
                BoundingBoxUpperRightCorner1 = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
                Meshes = new BattleforgeMesh[MeshCount];
                SomePoints = new Vector4[3];

                for (int i = 0; i < MeshCount; i++) Meshes[i] = new BattleforgeMesh(file);

                for (int i = 0; i < 3; i++) SomePoints[i] = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            }
            else
            {
                // Unsupported Mesh
            }
        }
        public CDspMeshFile(GLTF gltf)
        {
            Magic = 1314189598;
            Zero = 0;

            if (gltf.SkinnedModel)
            {

            }
            else
            {
                MeshCount = gltf.staticMesh.Meshes.Length;
            }

            MainWindow.LogMessage($"[INFO] Total Meshes to export: {MeshCount}");

            (Vector3 Min, Vector3 Max) BB = SharpGLTF.Runtime.MeshDecoder.EvaluateBoundingBox(gltf.Root.DefaultScene);
            BoundingBoxLowerLeftCorner1 = BB.Min; // Maybe with Animations???
            BoundingBoxUpperRightCorner1 = BB.Max; // Maybe with Animations???
            Meshes = new BattleforgeMesh[MeshCount];

            for (int m = 0; m < MeshCount; m++)
            {
                MainWindow.LogMessage($"[INFO] Creating Submesh #{m}");
                try
                {
                    Meshes[m] = new BattleforgeMesh(gltf, m);
                }
                catch (Exception ex)
                {
                    MainWindow.LogMessage($"[ERROR] {ex}");
                    return;
                }
                Meshes[m].BoundingBoxLowerLeftCorner2 = BoundingBoxLowerLeftCorner1;
                Meshes[m].BoundingBoxUpperRightCorner2 = BoundingBoxUpperRightCorner1;
            }

            SomePoints = new Vector4[3];
            SomePoints[0] = new Vector4(0, 0, 0, 1);
            SomePoints[1] = new Vector4(1, 1, 0, 0);
            SomePoints[2] = new Vector4(0, 0, 1, 1);
        }
        public int Size()
        {
            int Add = 0;

            for (int i = 0; i < MeshCount; i++)
            {
                Add += Meshes[i].Size();
            }

            return 84 + Add;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(Magic);
            bw.Write(Zero);
            bw.Write(MeshCount);
            bw.Write(BoundingBoxLowerLeftCorner1.X);
            bw.Write(BoundingBoxLowerLeftCorner1.Y);
            bw.Write(BoundingBoxLowerLeftCorner1.Z);
            bw.Write(BoundingBoxUpperRightCorner1.X);
            bw.Write(BoundingBoxUpperRightCorner1.Y);
            bw.Write(BoundingBoxUpperRightCorner1.Z);

            for (int i = 0; i < MeshCount; i++)
            {
                Meshes[i].Write(bw);
            }

            bw.Write(SomePoints[0].X);
            bw.Write(SomePoints[0].Y);
            bw.Write(SomePoints[0].Z);
            bw.Write(SomePoints[0].W);
            bw.Write(SomePoints[1].X);
            bw.Write(SomePoints[1].Y);
            bw.Write(SomePoints[1].Z);
            bw.Write(SomePoints[1].W);
            bw.Write(SomePoints[2].X);
            bw.Write(SomePoints[2].Y);
            bw.Write(SomePoints[2].Z);
            bw.Write(SomePoints[2].W);
        }
    }
}