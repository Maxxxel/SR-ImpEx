using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class UnknownStructure3
    {
        public UnknownStructure3(FileWrapper file)
        {
            Unknown58 = file.ReadInt();
            Unknown59 = file.ReadInt();
            Unknown60 = file.ReadInt();
            Unknown61 = file.ReadInt();
            Unknown62 = file.ReadInt();
        }

        public int Unknown58 { get; }
        public int Unknown59 { get; }
        public int Unknown60 { get; }
        public int Unknown61 { get; }
        public int Unknown62 { get; }
    }
}
