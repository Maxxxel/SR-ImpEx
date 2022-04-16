using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class JointGroup
    {
        public int JointCount { get; }
        public short[] JointIndices { get; }
        public JointGroup(GLTF gltf)
        {
            IReadOnlyList<SharpGLTF.Schema2.Node> n = gltf.Root.LogicalNodes;
            JointCount = 0;
            JointIndices = new short[n.Where(nn => nn.IsSkinJoint).Count()];

            for (int i = 0; i < n.Count; i++)
            {
                if (n[i].IsSkinJoint)
                {
                    JointIndices[JointCount] = (short)i;
                    JointCount++;
                }
            }
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(JointCount);

            for (int i = 0; i < JointCount; i++)
            {
                bw.Write(JointIndices[i]);
            }
        }
    }
}
