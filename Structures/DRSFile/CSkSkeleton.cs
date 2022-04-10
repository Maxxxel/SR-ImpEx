using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.DRSFile;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class CSkSkeleton
    {
        public CSkSkeleton(FileWrapper file)
        {
            Magic = file.ReadInt();
            Version = file.ReadInt(); // 3
            BoneMatrixCount = file.ReadInt();
            BoneMatrices = new BoneMatrix[BoneMatrixCount];

            for (int bm = 0; bm < BoneMatrixCount; bm++) BoneMatrices[bm] = new BoneMatrix(file);

            BoneCount = file.ReadInt();
            Bones = new Bone[BoneCount];

            for (int bc = 0; bc < BoneCount; bc++) Bones[bc] = new Bone(file);

            SuperParent = new Vector4[4];

            for (int i = 0; i < 4; i++) SuperParent[i] = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
        }

        public int Magic { get; }
        public int Version { get; }
        public int BoneMatrixCount { get; }
        public BoneMatrix[] BoneMatrices { get; }
        public int BoneCount { get; }
        public Bone[] Bones { get; }
        public Vector4[] SuperParent { get; }
    }
}