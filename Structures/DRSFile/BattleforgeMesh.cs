using SR_ImpEx.Helpers;
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
    }
}