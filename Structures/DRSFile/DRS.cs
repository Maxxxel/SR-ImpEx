using SharpGLTF.Scenes;
using SR_ImpEx.Helpers;
using SR_ImpEx.Structures.DRSFile;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class DRS
    {
        public float ModelSize { get; set; }
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
        public CGeoMesh CGeoMesh { get; internal set; }
        public CGeoOBBTree CGeoOBBTree { get; internal set; }
        public CSkSkinInfo CSkSkinInfo { get; private set; }
        public CSkSkeleton CSkSkeleton { get; private set; }
        public AnimationSet AnimationSet { get; }
        public CDspJointMap CDspJointMap { get; internal set; }
        public DrwResourceMeta DrwResourceMeta { get; internal set; }
        public CGeoPrimitiveContainer CGeoPrimitiveContainer { get; internal set; }
        public CollisionShape CollisionShape { get; internal set; }
        public RootNode RootNode { get; internal set; }
        public Node CGeoMeshNode { get; internal set; }
        public Node CGeoOBBTreeNode { get; internal set; }
        public Node JointMapNode { get; internal set; }
        public Node MeshNode { get; internal set; }
        public Node DrwResourceMetaNode { get; internal set; }
        public Node CGeoPrimitiveContainerNode { get; internal set; }
        public Node CollisionShapeNode { get; internal set; }
        public RootNodeInformation RootNodeInformation { get; internal set; }
        public NodeInformation CGeoMeshInformation { get; internal set; }
        public NodeInformation CGeoOBBTreeInformation { get; internal set; }
        public NodeInformation JointNodeInformation { get; internal set; }
        public NodeInformation MeshNodeInformation { get; internal set; }
        public NodeInformation DrwResourceMetaInformation { get; internal set; }
        public NodeInformation CollisionShapeInformation { get; internal set; }
        public NodeInformation CGeoPrimitiveContainerInformation { get; internal set; }
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
        public DRS(GLTF gltf)
        {
            Magic = -981667554;
            NumberOfModels = 1;
            NodeHierarchyOffset = 20;
            NodeCount = 1;

            MainWindow.LogMessage("[INFO] Creating CGeoMesh.");
            CGeoMesh = new CGeoMesh(gltf); NodeCount++;
            MainWindow.LogMessage("[INFO] Creating CGeoOBBTree.");
            CGeoOBBTree = new CGeoOBBTree(gltf); NodeCount++;
            MainWindow.LogMessage("[INFO] Creating CDspJointMap.");
            CDspJointMap = new CDspJointMap(gltf); NodeCount++;
            MainWindow.LogMessage("[INFO] Creating CDspMeshFile.");
            CDspMeshFile = new CDspMeshFile(gltf); NodeCount++;
            MainWindow.LogMessage("[INFO] Creating DrwResourceMeta.");
            DrwResourceMeta = new DrwResourceMeta(gltf); NodeCount++;

            if (gltf.SkinnedModel)
            {
                // WIP
            }
            else if (gltf.CollisionShaped)
            {
                CGeoPrimitiveContainer = new CGeoPrimitiveContainer(gltf); NodeCount++;
                CollisionShape = new CollisionShape(gltf); NodeCount++;
            }

            // We need to get the Sizes of our Classes now
            int GeoSize = CGeoMesh.Size(); MainWindow.LogMessage($"[INFO] Geo Size {GeoSize}");
            int OBBSize = CGeoOBBTree.Size(); MainWindow.LogMessage($"[INFO] OBBSize {OBBSize}");
            int JointSize = CDspJointMap.Size(); MainWindow.LogMessage($"[INFO] JointSize {JointSize}");
            int MeshSize = CDspMeshFile.Size(); MainWindow.LogMessage($"[INFO] MeshSize {MeshSize}");
            int DrwResSize = DrwResourceMeta.Size(); MainWindow.LogMessage($"[INFO] DrwResSize {DrwResSize}");
            int PrimitiveSize = 0;
            int CollisionSize = 0;

            if (gltf.SkinnedModel)
            {
                // WIP
            }
            else if (gltf.CollisionShaped)
            {
                PrimitiveSize = CGeoPrimitiveContainer.Size();
                CollisionSize = CollisionShape.Size();
            }

            int InfoIndex = 0;
            int InformationOffset = 20;

            RootNode = new RootNode(); InformationOffset += RootNode.Size(); InfoIndex++;

            int GeoIndex = InfoIndex;
            CGeoMeshNode = new Node("CGeoMesh", InfoIndex); InformationOffset += CGeoMeshNode.Size(); InfoIndex++;
            int OBBIndex = InfoIndex;
            CGeoOBBTreeNode = new Node("CGeoOBBTree", InfoIndex); InformationOffset += CGeoOBBTreeNode.Size(); InfoIndex++;
            int JointIndex = InfoIndex;
            JointMapNode = new Node("CDspJointMap", InfoIndex); InformationOffset += JointMapNode.Size(); InfoIndex++;
            int MeshIndex = InfoIndex;
            MeshNode = new Node("CDspMeshFile", InfoIndex); InformationOffset += MeshNode.Size(); InfoIndex++;
            int DrwResIndex = InfoIndex;
            DrwResourceMetaNode = new Node("DrwResourceMeta", InfoIndex); InformationOffset += DrwResourceMetaNode.Size(); InfoIndex++;

            int CollisionIndex = 0;
            int PrimitiveIndex = 0;

            if (gltf.SkinnedModel)
            {
                // WIP
            }
            else if (gltf.CollisionShaped)
            {
                PrimitiveIndex = InfoIndex;
                CGeoPrimitiveContainerNode = new Node("CGeoPrimitiveContainer", InfoIndex); InformationOffset += CGeoPrimitiveContainerNode.Size(); InfoIndex++;
                CollisionIndex = InfoIndex;
                CollisionShapeNode = new Node("collisionShape", InfoIndex); InformationOffset += CollisionShapeNode.Size(); InfoIndex++;
            }

            int DataOffset = InformationOffset + (NodeCount - 1) * 32;
            RootNodeInformation = new RootNodeInformation(NodeCount - 1);
            CGeoMeshInformation = new NodeInformation("CGeoMesh", GeoIndex, DataOffset, GeoSize); DataOffset += GeoSize;
            CGeoOBBTreeInformation = new NodeInformation("CGeoOBBTree", OBBIndex, DataOffset, OBBSize); DataOffset += OBBSize;
            JointNodeInformation = new NodeInformation("CDspJointMap", JointIndex, DataOffset, JointSize); DataOffset += JointSize;
            MeshNodeInformation = new NodeInformation("CDspMeshFile", MeshIndex, DataOffset, MeshSize); DataOffset += MeshSize;
            DrwResourceMetaInformation = new NodeInformation("DrwResourceMeta", DrwResIndex, DataOffset, DrwResSize); DataOffset += DrwResSize;

            if (gltf.CollisionShaped)
            {
                CGeoPrimitiveContainerInformation = new NodeInformation("CGeoPrimitiveContainer", PrimitiveIndex, DataOffset, PrimitiveSize); DataOffset += PrimitiveSize;
                CollisionShapeInformation = new NodeInformation("collisionShape", CollisionIndex, DataOffset, CollisionSize); DataOffset += CollisionSize;
            }

            // Update at last
            NodeInformationOffset = InformationOffset;
        }
        internal void Write(BinaryWriter bw)
        {
            // Write Header
            bw.Write(Magic);
            bw.Write(NumberOfModels);
            bw.Write(NodeInformationOffset);
            bw.Write(NodeHierarchyOffset);
            bw.Write(NodeCount);
            // Write Node Hierarchy
            RootNode.Write(bw);
            CGeoMeshNode.Write(bw);
            CGeoOBBTreeNode.Write(bw);
            JointMapNode.Write(bw);
            MeshNode.Write(bw);
            DrwResourceMetaNode.Write(bw);
            if (CGeoPrimitiveContainerNode != null) CGeoPrimitiveContainerNode.Write(bw);
            if (CollisionShapeNode != null) CollisionShapeNode.Write(bw);
            // Write Node Information
            RootNodeInformation.Write(bw); // Reads First, then NodeInformation(s)
            CGeoMeshInformation.Write(bw);
            CGeoOBBTreeInformation.Write(bw);
            JointNodeInformation.Write(bw);
            MeshNodeInformation.Write(bw);
            DrwResourceMetaInformation.Write(bw);
            if (CGeoPrimitiveContainerInformation != null) CGeoPrimitiveContainerInformation.Write(bw);
            if (CollisionShapeInformation != null) CollisionShapeInformation.Write(bw);
            // Node Data
            CGeoMesh.Write(bw);
            CGeoOBBTree.Write(bw);
            CDspJointMap.Write(bw);
            CDspMeshFile.Write(bw);
            DrwResourceMeta.Write(bw);
            if (CGeoPrimitiveContainer != null) CGeoPrimitiveContainer.Write(bw);
            if (CollisionShape != null) CollisionShape.Write(bw);
        }
    }
}
