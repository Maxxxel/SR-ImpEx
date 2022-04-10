using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.DRSFile;

namespace SR_ImpEx.Structures
{
    public class CSkSkinInfo
    {
        public CSkSkinInfo(FileWrapper file)
        {
            Version = file.ReadInt();
            VertexCount = file.ReadInt();
            VertexWeights = new VertexWeight[VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                VertexWeights[i] = new VertexWeight(file);
            }
        }

        public int Version { get; }
        public int VertexCount { get; }
        public VertexWeight[] VertexWeights { get; }
    }
}