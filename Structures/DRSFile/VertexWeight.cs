using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class VertexWeight
    {
        public VertexWeight(FileWrapper file)
        {
            Weights = file.ReadFloats(-1, 4);
            BoneIndices = file.ReadInts(-1, 4);
        }

        public float[] Weights { get; }
        public int[] BoneIndices { get; }
    }
}
