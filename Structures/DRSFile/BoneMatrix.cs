using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class BoneMatrix
    {
        public BoneMatrix(FileWrapper file)
        {
            BoneVertices = new BoneVertex[4];

            for (int bv = 0; bv < 4; bv++) BoneVertices[bv] = new BoneVertex(file);
        }

        public BoneVertex[] BoneVertices { get; }
    }
}
