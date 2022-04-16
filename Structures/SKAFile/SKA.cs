using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.SKAFile
{
    public class SKA
    {
        public SKA(FileWrapper file, string name)
        {
            Name = name;
            Magic = file.ReadInt();
            Type = file.ReadUInt();
            Length = file.ReadInt();
            Headers = new Header[Length];

            for (int h = 0; h < Length; h++) Headers[h] = new Header(file);

            KeyframeCount = file.ReadInt();
            Times = file.ReadFloats(-1, KeyframeCount);
            Keyframes = new Keyframe[KeyframeCount];

            for (int q = 0; q < KeyframeCount; q++) Keyframes[q] = new Keyframe(file);

            if (Type == 6)
            {
                TotalTime = file.ReadFloat(); // Guess
                Uk1 = file.ReadInt();
                Uk2 = file.ReadInt();
                Zeroes = file.ReadInts(-1, 3);
            }
            else if (Type == 7)
            {
                TotalTime = file.ReadFloat(); // Guess
                Uk1 = file.ReadInt();
                Uk2 = file.ReadInt();
                Zero = file.ReadInt();
                Zeroes = file.ReadInts(-1, 3);
            }
        }

        public string Name { get; }
        public int Magic { get; }
        public uint Type { get; }
        public int Length { get; }
        public Header[] Headers { get; }
        public int KeyframeCount { get; }
        public float[] Times { get; }
        public Keyframe[] Keyframes { get; }
        public float TotalTime { get; }
        public int Uk1 { get; }
        public int Uk2 { get; }
        public int Zero { get; }
        public int[] Zeroes { get; }
    }
}
