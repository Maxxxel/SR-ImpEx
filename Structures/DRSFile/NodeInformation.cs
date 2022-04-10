using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class NodeInformation
    {
        public int Magic { get; }
        public int Identifier { get; }
        public int Offset { get; }
        public int Size { get; }
        public byte[] Spacer { get; }
        public NodeInformation(FileWrapper file)
        {
            Magic = file.ReadInt();
            Identifier = file.ReadInt();
            Offset = file.ReadInt();
            Size = file.ReadInt();
            Spacer = file.ReadBytes(-1, 16);
        }
    }
}
