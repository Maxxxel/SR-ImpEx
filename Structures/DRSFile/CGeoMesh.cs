using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class CGeoMesh
    {
        public int Magic { get; }
        public int IndexCount { get; }
        public Triangle[] Triangles { get; }
        public int VertexCount { get; }
        public Vector4[] Vertices { get; }
        public float MaxX { get; private set; }
        public float MinX { get; private set; }
        public float MaxY { get; private set; }
        public float MinY { get; private set; }
        public float MaxZ { get; private set; }
        public float MaxW { get; private set; }
        public float MinZ { get; private set; }
        public float MinW { get; private set; }

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
                {
                    var X = file.ReadFloat();
                    var Y = file.ReadFloat();
                    var Z = file.ReadFloat();
                    var W = file.ReadFloat();
                    Vertices[v] = new Vector4(X, Y, Z, W);

                    // Get the highest and lowest values for X, Y, Z, W
                    if (X > MaxX) MaxX = X;
                    if (X < MinX) MinX = X;
                    if (Y > MaxY) MaxY = Y;
                    if (Y < MinY) MinY = Y;
                    if (Z > MaxZ) MaxZ = Z;
                    if (Z < MinZ) MinZ = Z;
                    if (W > MaxW) MaxW = W;
                    if (W < MinW) MinW = W;
                }
            }

            // Print the highest and lowest values for X, Y, Z, W using MainWindow.LogMessage in one Line
            MainWindow.LogMessage(
                $"MaxX: {MaxX}, MinX: {MinX}, MaxY: {MaxY}, MinY: {MinY}, MaxZ: {MaxZ}, MinZ: {MinZ}, MaxW: {MaxW}, MinW: {MinW}");
        }
        public CGeoMesh(GLTF gltf)
        {
            Magic = 1;

            foreach (var P in gltf.staticMesh.UnifiedMesh.Primitives) // Only 1!
            {
                IReadOnlyList<(int, int, int)> Tris = P.Triangles; // e. g. 100
                IReadOnlyList<VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty>> Vs = P.Vertices;

                IndexCount = Tris.Count * 3; // e. g. 300
                Triangles = new Triangle[IndexCount / 3]; // e. g. 100

                for (int i = 0; i < IndexCount / 3; i++)
                {
                    (int, int, int) Tri = Tris[i];
                    Triangles[i] = new Triangle(Tri);
                }

                VertexCount = Vs.ToList().Count;
                Vertices = new Vector4[VertexCount];

                for (int i = 0; i < VertexCount; i++)
                {
                    VertexBuilder<VertexPosition, VertexEmpty, VertexEmpty> Vertex = Vs[i];
                    Vector3 V = Vertex.Geometry.Position;
                    Vertices[i] = new Vector4(V.X, V.Y, V.Z, 1);
                }
            }
        }
        internal int Size()
        {
            return 12 + (IndexCount * 2) + (VertexCount * 16);
        }
        internal void Write(BinaryWriter bw)
        {
            bw.Write(Magic);
            bw.Write(IndexCount);

            for (int i = 0; i < IndexCount / 3; i++)
            {
                Triangle T = Triangles[i];

                for (int j = 0; j < T.Indices.Length; j++)
                {
                    bw.Write(T.Indices[j]);
                }
            }

            bw.Write(VertexCount);

            for (int i = 0; i < VertexCount; i++)
            {
                Vector4 V = Vertices[i];

                bw.Write(V.X);
                bw.Write(V.Y);
                bw.Write(V.Z);
                bw.Write(V.W);
            }
        }
    }
}