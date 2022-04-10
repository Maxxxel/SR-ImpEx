using SR_ImpEx.Helpers;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.DRSFile;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class DRS
    {
        public int Magic { get; internal set; }
        public int NumberOfModels { get; internal set; }
        public string Location { get; internal set; }
        public int NodeInformationOffset { get; internal set; }
        public int NodeHierarchyOffset { get; internal set; }
        public int NodeCount { get; internal set; }
        public NodeInformation AnimationSetNodeInformation { get; internal set; }
        public NodeInformation CDspMeshFileNodeInformation { get; internal set; }
        public NodeInformation CGeoMeshFileNodeInformation { get; }
        public NodeInformation CSkSkinInfoFileNodeInformation { get; }
        public NodeInformation CSkSkeletonFileNodeInformation { get; }
        public CDspMeshFile CDspMeshFile { get; internal set; }
        public CGeoMesh CGeoMesh { get; private set; }
        public CSkSkinInfo CSkSkinInfo { get; private set; }
        public CSkSkeleton CSkSkeleton { get; private set; }
        public AnimationSet AnimationSet { get; }
        public DRS(FileWrapper File)
        {
            Magic = File.ReadInt();

            if (Magic == -981667554)
            {
                NumberOfModels = File.ReadInt();
                NodeInformationOffset = File.ReadInt();
                NodeHierarchyOffset = File.ReadInt();
                NodeCount = File.ReadInt();

                if (NodeCount != 0)
                {
                    // We dont need every information of the file, so we can skip many parts
                    // For Export we only need
                    // Static Models: CDspMeshFile
                    // Animated Models: CDspMeshFile, CGeoMesh, CSkSkinInfo, CSkSkeleton, AnimationSet
                    // Check if we have an animated Model --> AnimationSet != null, therefore we need to load the NodeInformations first
                    File.Seek(NodeInformationOffset + 32); // 32 = RootNodeSize

                    for (int i = 1; i < NodeCount; i++)
                    {
                        NodeInformation Info = new NodeInformation(File);

                        if (Info.Magic == -475734043) // AnimationSet
                        {
                            AnimationSetNodeInformation = Info;
                        }
                        else if (Info.Magic == -1900395636) // CDspMeshFile
                        {
                            CDspMeshFileNodeInformation = Info;
                        }
                        else if (Info.Magic == 100449016) // CGeoMesh
                        {
                            CGeoMeshFileNodeInformation = Info;
                        }
                        else if (Info.Magic == -761174227) // CSkSkinInfo
                        {
                            CSkSkinInfoFileNodeInformation = Info;
                        }
                        else if (Info.Magic == -2110567991) // CSkSkeleton
                        {
                            CSkSkeletonFileNodeInformation = Info;
                        }
                    }

                    if (AnimationSetNodeInformation != null)
                    {
                        File.Seek(CDspMeshFileNodeInformation.Offset);
                        CDspMeshFile = new CDspMeshFile(File);

                        File.Seek(AnimationSetNodeInformation.Offset);
                        AnimationSet = new AnimationSet(File);

                        File.Seek(CGeoMeshFileNodeInformation.Offset);
                        CGeoMesh = new CGeoMesh(File);

                        File.Seek(CSkSkinInfoFileNodeInformation.Offset);
                        CSkSkinInfo = new CSkSkinInfo(File);

                        File.Seek(CSkSkeletonFileNodeInformation.Offset);
                        CSkSkeleton = new CSkSkeleton(File);
                    }
                    else
                    {
                        // Static Object
                        // Now that we have offset and size, we can simply create our CDspMeshFile Structure with it
                        File.Seek(CDspMeshFileNodeInformation.Offset);
                        CDspMeshFile = new CDspMeshFile(File);
                    }
                }
                else
                {
                    // Unsupported NodeCount
                }
            }
            else
            {
                // Unsupported Magic
            }
        }
    }
}
