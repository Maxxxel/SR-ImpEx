using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class BoneVertex
    {
        public BoneVertex(FileWrapper file)
        {
            X = file.ReadFloat();
            Y = file.ReadFloat();
            Z = file.ReadFloat();
            ParentReference = file.ReadInt();
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public int ParentReference { get; }
    }
}