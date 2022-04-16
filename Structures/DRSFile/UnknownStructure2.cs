using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class UnknownStructure2
    {
        public UnknownStructure2(FileWrapper file)
        {
            Unknown54 = file.ReadInt();
            Length = file.ReadInt();
            Name = file.ReadString(Length);
            Unknown56 = file.ReadInt();
            UnknownStructureLength = file.ReadInt();
            UnknownStructures = new UnknownStructure3[UnknownStructureLength];

            for (int u = 0; u < UnknownStructureLength; u++) UnknownStructures[u] = new UnknownStructure3(file);
        }

        public int Unknown54 { get; }
        public int Length { get; }
        public string Name { get; }
        public int Unknown56 { get; }
        public int UnknownStructureLength { get; }
        public UnknownStructure3[] UnknownStructures { get; }
    }
}
