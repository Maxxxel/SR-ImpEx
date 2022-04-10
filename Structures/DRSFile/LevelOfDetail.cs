using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class LevelOfDetail
    {
        public int Length { get; }
        public int LODLevel { get; }
        public LevelOfDetail(FileWrapper file)
        {
            Length = file.ReadInt();

            if (Length == 1)
            {
                LODLevel = file.ReadInt();
            }
        }
    }
}