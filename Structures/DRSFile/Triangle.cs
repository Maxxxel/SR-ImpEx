using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures
{
    public class Triangle
    {
        public short[] Indices { get; }

        public Triangle(FileWrapper file)
        {
            Indices = file.ReadShorts(-1, 3);
        }
    }
}