using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class UnknownStructure
    {
        public UnknownStructure(FileWrapper file)
        {
            Unknown36 = file.ReadInt();
            Unknown37 = file.ReadInt();
            Unknown38 = file.ReadInt();
        }

        public int Unknown36 { get; }
        public int Unknown37 { get; }
        public int Unknown38 { get; }
    }
}
