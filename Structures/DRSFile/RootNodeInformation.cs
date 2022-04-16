using System;
using System.IO;

namespace SR_ImpEx.Helpers
{
    public class RootNodeInformation
    {
        public byte[] Zeros { get; }
        public int NegOne { get; }
        public int One { get; }
        public int NodeInformationCount { get; }
        public int Zero { get; }

        public RootNodeInformation(int nodeCount)
        {
            Zeros = new byte[16];
            NegOne = -1;
            One = 1;
            NodeInformationCount = nodeCount;
            Zero = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Zeros);
            bw.Write(NegOne);
            bw.Write(One);
            bw.Write(NodeInformationCount);
            bw.Write(Zero);
        }
    }
}