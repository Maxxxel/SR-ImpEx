using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class Bone
    {
        public Bone(FileWrapper file)
        {
            Version = file.ReadInt();
            Identifier = file.ReadInt();
            Length = file.ReadInt();
            Name = file.ReadString(Length);
            ChildCount = file.ReadInt();

            if (ChildCount > 0) Children = file.ReadInts(-1, ChildCount);
        }

        public int Version { get; }
        public int Identifier { get; }
        public int Length { get; }
        public string Name { get; }
        public int ChildCount { get; }
        public int[] Children { get; }
    }
}
