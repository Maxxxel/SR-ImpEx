using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class ModeAnimationKey
    {
        private int uk;

        public ModeAnimationKey(FileWrapper file, int subType)
        {
            if (subType == 2)
            {
                Type = 2;
            }
            else
            {
                Type = file.ReadInt();
            }

            Length = file.ReadInt();
            Magic2 = file.ReadString(Length);
            Unknown11 = file.ReadInt();

            if (Type == 1)
            {
                Unknown12 = file.ReadInt();
                Unknown13 = file.ReadShort();
                Unknown14 = file.ReadInt();
                Unknown15 = file.ReadByte();
                Unknown16 = file.ReadInt();
                Unknown17 = file.ReadInt();
                Unknown18 = file.ReadInt();
                Unknown19 = file.ReadByte();
            }
            else if (Type == 2 || Type == 3 || Type == 4 || Type == 5)
            {
                Unknown20 = file.ReadInt();
                Unknown21 = file.ReadShort();
            }
            else if (Type == 6)
            {
                Unknown22 = file.ReadInt();
                Unknown23 = file.ReadShort();
                Unknown24 = file.ReadInt();
                Unknown25 = file.ReadShort();
            }

            VariantCount = file.ReadInt();
            Variants = new AnimationSetVariant[VariantCount];

            for (int u = 0; u < VariantCount; u++) Variants[u] = new AnimationSetVariant(file);

            if (Type == 2) uk = file.ReadInt();
        }

        public int Type { get; }
        public int Length { get; }
        public string Magic2 { get; }
        public int Unknown11 { get; }
        public int Unknown12 { get; }
        public short Unknown13 { get; }
        public int Unknown14 { get; }
        public byte Unknown15 { get; }
        public int Unknown16 { get; }
        public int Unknown17 { get; }
        public int Unknown18 { get; }
        public byte Unknown19 { get; }
        public int Unknown20 { get; }
        public short Unknown21 { get; }
        public int Unknown22 { get; }
        public short Unknown23 { get; }
        public int Unknown24 { get; }
        public short Unknown25 { get; }
        public int VariantCount { get; }
        public AnimationSetVariant[] Variants { get; }
    }
}
