using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class CGeoOBBTree
    {
        public int Magic { get; }
        public int Version { get; }
        public int MatrixCount { get; }
        public Matrix4x4[] Matrices { get; }
        public int TriangleCount { get; }
        public Triangle[] Triangles { get; }

        public CGeoOBBTree(GLTF gltf)
        {
            Magic = 1845540702;
            Version = 3;
            MatrixCount = 0; // We need to wait for 100 % Decryption
            Matrices = new Matrix4x4[MatrixCount];

            foreach (var P in gltf.staticMesh.UnifiedMesh.Primitives) // Only 1!
            {
                IReadOnlyList<(int, int, int)> Tris = P.Triangles; // e. g. 100
                TriangleCount = Tris.ToList().Count; // e. g. 100
                Triangles = new Triangle[TriangleCount]; // e. g. 100

                for (int i = 0; i < TriangleCount; i++)
                {
                    (int, int, int) Tri = Tris[i];
                    Triangles[i] = new Triangle(Tri);
                }
            };
        }

        internal int Size()
        {
            return 16 + (MatrixCount * 64) + (TriangleCount * 6);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Magic);
            bw.Write(Version);
            bw.Write(MatrixCount);

            for (int i = 0; i < MatrixCount; i++)
            {
                Matrix4x4 F = Matrices[i];

                //for (int j = 0; j < F.Indices.Length; j++)
                //{
                //    bw.Write(F.Indices[j]);
                //}
            }

            bw.Write(TriangleCount);

            for (int i = 0; i < TriangleCount; i++)
            {
                Triangle T = Triangles[i];

                for (int j = 0; j < T.Indices.Length; j++)
                {
                    bw.Write(T.Indices[j]);
                }
            }
        }
    }
}
