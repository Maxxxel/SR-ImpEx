using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.DRSFile
{
    public class CDspJointMap
    {
        public int Version { get; }
        public int JointGroupCount { get; }
        public JointGroup[] JointGroups { get; }
        public CDspJointMap(GLTF gltf)
        {
            Version = 1; // Verify
            JointGroupCount = !gltf.SkinnedModel ? 0 : 1;

            if (JointGroupCount > 0) // WIP
            {
                //JointGroups = new JointGroup[JointGroupCount];
                //JointGroups[0] = new JointGroup(gltf);
            }
        }
        internal int Size()
        {
            int add = 0;

            for (int i = 0; i < JointGroupCount; i++)
            {
                add += 4 + JointGroups[i].JointCount * 2;
            }

            return 8 + add;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Version);
            bw.Write(JointGroupCount);

            for (int i = 0; i < JointGroupCount; i++)
            {
                JointGroups[i].Write(bw);
            }
        }
    }
}