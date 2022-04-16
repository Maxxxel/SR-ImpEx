using System;
using System.IO;

namespace SR_ImpEx.Helpers
{
    public class Node
    {
        public int InfoIndex { get; }
        public int Length { get; }
        public string Name { get; }
        public int Zero { get; }

        public Node(string v, int infoIndex)
        {
            InfoIndex = infoIndex;
            Length = v.Length;
            Name = v;
            Zero = 0;
        }

        internal int Size()
        {
            return 12 + Length;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(InfoIndex);
            bw.Write(Length);
            bw.Write(Name.ToCharArray());
            bw.Write(Zero);
        }
    }
}