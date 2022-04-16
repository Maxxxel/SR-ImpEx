using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class Refraction
    {
        public int Length { get; }
        public int Identifier { get; }
        public float R { get; }
        public float G { get; }
        public float B { get; }
        public Refraction(FileWrapper file)
        {
            Length = file.ReadInt(); // 0 or 1

            if (Length == 1)
            {
                Identifier = file.ReadInt(); // 1668510769
                R = file.ReadFloat();
                G = file.ReadFloat();
                B = file.ReadFloat();
            }
        }
    }
}