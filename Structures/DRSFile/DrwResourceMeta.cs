using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class DrwResourceMeta
    {
        public int[] UnknownInts { get; }
        public int Length { get; }
        public string UnknownHash { get; }

        public DrwResourceMeta(GLTF gltf)
        {
            UnknownInts = new int[2];
            Length = 0;
            UnknownHash = ""; // ???
        }

        internal int Size()
        {
            return 12 + Length;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(UnknownInts[0]);
            bw.Write(UnknownInts[1]);
            bw.Write(Length);
            if (Length != 0) bw.Write(UnknownHash);
        }
    }
}
