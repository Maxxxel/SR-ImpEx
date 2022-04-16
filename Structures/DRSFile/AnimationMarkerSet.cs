using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class AnimationMarkerSet
    {
        public AnimationMarkerSet(FileWrapper file)
        {
            AnimID = file.ReadInt();
            Length = file.ReadInt();
            FileName = file.ReadString(Length);
            Unknown43 = file.ReadInt();
            AnimationMarkerCount = file.ReadInt();
            AnimationMarkers = new AnimationMarker[AnimationMarkerCount];

            for (int u = 0; u < AnimationMarkerCount; u++) AnimationMarkers[u] = new AnimationMarker(file);
        }

        public int AnimID { get; }
        public int Length { get; }
        public string FileName { get; }
        public int Unknown43 { get; }
        public int AnimationMarkerCount { get; }
        public AnimationMarker[] AnimationMarkers { get; }
    }
}
