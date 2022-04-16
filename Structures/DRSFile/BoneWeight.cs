using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class BoneWeight
    {
        public BoneWeight(int _boneIndex0, int _boneIndex1, int _boneIndex2, int _boneIndex3, float _weight0, float _weight1, float _weight2, float _weight3)
        {
            boneIndex0 = _boneIndex0;
            boneIndex1 = _boneIndex1;
            boneIndex2 = _boneIndex2;
            boneIndex3 = _boneIndex3;
            weight0 = _weight0;
            weight1 = _weight1;
            weight2 = _weight2;
            weight3 = _weight3;
        }

        public int boneIndex0 { get; }
        public int boneIndex1 { get; }
        public int boneIndex2 { get; }
        public int boneIndex3 { get; }
        public float weight0 { get; }
        public float weight1 { get; }
        public float weight2 { get; }
        public float weight3 { get; }
    }
}
