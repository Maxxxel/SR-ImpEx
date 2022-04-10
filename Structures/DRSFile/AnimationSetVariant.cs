using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class AnimationSetVariant
    {
        public AnimationSetVariant(FileWrapper file)
        {
            Unknown25 = file.ReadInt();
            Weight = file.ReadInt();
            Length = file.ReadInt();
            VariantName = file.ReadString(Length);

            if (Unknown25 >= 4)
            {
                Start = file.ReadFloat();
                End = file.ReadFloat();
            }

            if (Unknown25 >= 5)
            {
                AllowsIK = file.ReadByte();
            }

            if (Unknown25 >= 7)
            {
                Unknown31 = file.ReadByte();
            }
        }

        public int Unknown25 { get; }
        public int Weight { get; }
        public int Length { get; }
        public string VariantName { get; }
        public float Start { get; }
        public float End { get; }
        public byte AllowsIK { get; }
        public byte Unknown31 { get; }
    }
}
