using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.SKAFile
{
    public class Header
    {
        public Header(FileWrapper file)
        {
            StartFrame = file.ReadInt();
            Length = file.ReadInt();
            Type = file.ReadInt(); // 0: Position, 1: Rotation
            BoneVersion = file.ReadInt();
        }

        public int StartFrame { get; }
        public int Length { get; }
        public int Type { get; }
        public int BoneVersion { get; }
    }
}
