using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class Texture
    {
        public int Identifier { get; }
        public int Length { get; }
        public string Name { get; }
        public int Spacer { get; }

        public Texture(FileWrapper file)
        {
            Identifier = file.ReadInt();
            Length = file.ReadInt();
            Name = file.ReadString(Length);
            Spacer = file.ReadInt();
        }
    }
}