using SR_ImpEx.Structures;
using SR_ImpEx.Structures.DRSFile;
using System.Collections.Generic;

namespace SR_ImpEx.Helpers
{
    public class AnimationFinder
    {
        public HashSet<string> FindModeAnimationKeys(DRS drs)
        {
            HashSet<string> ModeAnimationKeys = new HashSet<string>();
            AnimationSet AnimationSet = drs.AnimationSet;

            for (int AnimationMarkerCount = 0; AnimationMarkerCount < AnimationSet.AnimationMarkerCount; AnimationMarkerCount++)
            {
                ModeAnimationKeys.Add(AnimationSet.AnimationMarkerSets[AnimationMarkerCount].FileName);
            }

            for (int ModeAnimationKeyCount = 0; ModeAnimationKeyCount < AnimationSet.ModeAnimationKeyCount; ModeAnimationKeyCount++)
            {
                for (int VariantCount = 0; VariantCount < AnimationSet.ModeAnimationKeys[ModeAnimationKeyCount].VariantCount; VariantCount++)
                {
                    ModeAnimationKeys.Add(AnimationSet.ModeAnimationKeys[ModeAnimationKeyCount].Variants[VariantCount].VariantName);
                }
            }

            return ModeAnimationKeys;
        }
    }
}
