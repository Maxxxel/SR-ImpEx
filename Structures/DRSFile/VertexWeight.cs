using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class VertexWeight
    {
        public VertexWeight(FileWrapper file)
        {
            Weights = file.ReadFloats(-1, 4);
            BoneIndices = file.ReadInts(-1, 4);
        }

        public float[] Weights { get; }
        public int[] BoneIndices { get; }
    }
}
