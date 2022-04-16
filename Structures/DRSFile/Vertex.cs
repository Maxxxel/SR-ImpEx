using SR_ImpEx.Helpers;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class Vertex
    {
        public Vector3 Position { get; }
        public Vector3 Normal { get; }
        public Vector2 Texture { get; }
        public Vertex(FileWrapper file)
        {
            Position = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            Normal = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            Texture = new Vector2(file.ReadFloat(), file.ReadFloat());
        }
    }
}