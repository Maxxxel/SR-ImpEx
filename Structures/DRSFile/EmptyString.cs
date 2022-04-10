using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class EmptyString
    {
        public int Length { get; }
        public byte[] Uk { get; }
        public EmptyString(FileWrapper file)
        {
            Length = file.ReadInt();
            Uk = file.ReadBytes(-1, Length * 2);
        }
    }
}