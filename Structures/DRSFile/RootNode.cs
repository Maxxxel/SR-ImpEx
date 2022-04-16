using System;
using System.IO;

namespace SR_ImpEx.Helpers
{
    public class RootNode
    {
        public int RootIdentifier { get; }
        public int Unknown { get; }
        public int Length { get; }
        public string NodeName { get; }
        public RootNode()
        {
            RootIdentifier = 0;
            Unknown = 0;
            Length = 9;
            NodeName = "root node";
        }

        internal int Size()
        {
            return 21;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(RootIdentifier);
            bw.Write(Unknown);
            bw.Write(Length);
            bw.Write(NodeName.ToCharArray());
        }
    }
}