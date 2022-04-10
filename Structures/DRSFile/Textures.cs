using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class Textures
    {
        public int Length { get; }
        public Texture[] Texture { get; }
        public Textures(FileWrapper file)
        {
            Length = file.ReadInt();
            Texture = new Texture[Length];
            for (int i = 0; i < Length; i++) Texture[i] = new Texture(file);
        }
    }
}