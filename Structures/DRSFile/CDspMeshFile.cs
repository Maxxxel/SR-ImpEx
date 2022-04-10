using SR_ImpEx.Helpers;
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
    }
}