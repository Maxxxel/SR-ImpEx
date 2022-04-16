using SharpGLTF.Scenes;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.DRSFile;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace SR_ImpEx.Helpers
{
    public class Exporter
    {
        public void ExportToGLTF(string fileName, DRS drs, HashSet<string> animations)
        {
            SceneBuilder DRS_Scene = new SceneBuilder("DRS_SCENE");

            if (drs.AnimationSetNodeInformation != null)
            {
                // Skinned Mesh
                SkinnedMesh SkinnedMesh = new SkinnedMesh(drs.CGeoMesh, drs.CDspMeshFile, drs.CSkSkinInfo, drs.CSkSkeleton, animations, drs.Location);
                DRS_Scene.AddSkinnedMesh(SkinnedMesh.Mesh, Matrix4x4.Identity, SkinnedMesh.Skeleton.Binds);
            }
            else
            {
                // Static Mesh
                StaticMesh StaticMeshes = new StaticMesh(drs.CDspMeshFile, drs.Location);
                for (int i = 0; i < StaticMeshes.Meshes.Length; i++) DRS_Scene.AddRigidMesh(StaticMeshes.Meshes[i], Matrix4x4.Identity);
            }

            SharpGLTF.Schema2.ModelRoot GLTF = DRS_Scene.ToGltf2();
            GLTF.SaveGLB(AppContext.BaseDirectory + "/GLTF_Exports/" + fileName.Replace(".drs", "") + ".glb");

            string[] tmpList = Directory.GetFiles(AppContext.BaseDirectory + "/GLTF_Exports/", "_temporary_*");
            foreach (string f in tmpList)
            {
                File.Delete(f);
            }

            MainWindow.LogMessage("[INFO] File has been exported.");
        }

        public void ExportToDRS(string fileName, GLTF gltf)
        {
            // Create the Classes every models uses first
            // All use: Root, CGeo, OBB, Joint, Mesh, DrwRes ==> 6 Nodes
            // Static Objects need: CGeoPrimitive, collisionShape ==> + 2 Nodes
            // Skinned Objects need: Skin, Skeleton, CDrwLocator, AnimationSet, AnimationTiming, EffectSet ==> + 6 Nodes
            // Buildnings need: WIP
            DRS newDRSFile = new DRS(gltf);

            Directory.CreateDirectory(AppContext.BaseDirectory + "/DRS_Exports");
            string CurrentDir = AppContext.BaseDirectory + "/DRS_Exports/";

            using (FileStream fs = new FileStream(CurrentDir + fileName.Replace(".glb", ".drs").Replace(".gltf", ".drs"), FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII))
                {
                    newDRSFile.Write(bw);
                }
            }

            string[] tmpList = Directory.GetFiles(AppContext.BaseDirectory + "/DRS_Exports/", "_temporary_*");
            foreach (string f in tmpList)
            {
                File.Delete(f);
            }

            MainWindow.LogMessage($"File has been exported.");
        }
    }
}