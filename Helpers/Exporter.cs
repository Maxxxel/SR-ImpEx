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
        public static string Folder { get; private set; }

        public void ExportToGLTF(string fileName, DRS drs, HashSet<string> animations)
        {
            // Create Folder Structure
            string CurrentDir = AppContext.BaseDirectory;
            // Create Export Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports"));
            // Create DRS_Exports folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//GLTF_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//GLTF_Exports"));
            // Create Model Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//GTLF_Exports//" + fileName))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//GLTF_Exports//" + fileName));
            // Set the Model Save Folder
            Folder = Path.Combine(CurrentDir, "Exports//GLTF_Exports//" + fileName);
            
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

            DRS_Scene.ApplyBasisTransform(MainWindow.Scale);

            SharpGLTF.Schema2.ModelRoot GLTF = DRS_Scene.ToGltf2();
            GLTF.SaveGLB(Folder + "/" + fileName.Replace(".drs", "") + ".glb");

            MainWindow.LogMessage("[INFO] File has been exported.");
        }
        public void ExportToDRS(string fileName, GLTF gltf)
        {
            // Create the Classes every models uses first
            // All use: Root, CGeo, OBB, Joint, Mesh, DrwRes ==> 6 Nodes
            // Static Objects need: CGeoPrimitive, collisionShape ==> + 2 Nodes
            // Skinned Objects need: Skin, Skeleton, CDrwLocator, AnimationSet, AnimationTiming, EffectSet ==> + 6 Nodes
            // Buildings need: WIP

            // Create Folder Structure
            string CurrentDir = AppContext.BaseDirectory;
            // Create Export Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports"));
            // Create DRS_Exports folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//DRS_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//DRS_Exports"));
            // Create Model Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName));
            // Set the Model Save Folder
            Folder = Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName);

            MainWindow.LogMessage("[INFO] Creating new DRS File.");
            DRS newDRSFile = new DRS(gltf);

            MainWindow.LogMessage("[INFO] Writting...");
            using (FileStream fs = new FileStream(Folder + "/object_frostland_lava_tree_l_001.drs", FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII))
                {
                    try
                    {
                        newDRSFile.Write(bw);
                    }
                    catch (Exception ex)
                    {
                        MainWindow.LogMessage($"[ERROR] {ex}");
                    }
                }
            }

            MainWindow.LogMessage($"[INFO] File has been exported.");
        }

        internal void ExportToDRS(string fileName, Assimp.Scene model)
        {
            // Create the Classes every models uses first
            // All use: Root, CGeo, OBB, Joint, Mesh, DrwRes ==> 6 Nodes
            // Static Objects need: CGeoPrimitive, collisionShape ==> + 2 Nodes
            // Skinned Objects need: Skin, Skeleton, CDrwLocator, AnimationSet, AnimationTiming, EffectSet ==> + 6 Nodes
            // Buildings need: WIP

            // Create Folder Structure
            string CurrentDir = AppContext.BaseDirectory;
            // Create Export Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports"));
            // Create DRS_Exports folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//DRS_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//DRS_Exports"));
            // Create Model Folder
            if (!Directory.Exists(Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName))) Directory.CreateDirectory(Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName));
            // Set the Model Save Folder
            Folder = Path.Combine(CurrentDir, "Exports//DRS_Exports//" + fileName);

            MainWindow.LogMessage("[INFO] Creating new DRS File.");
            DRS newDRSFile = new DRS(model);

            MainWindow.LogMessage("[INFO] Writting...");
            using (FileStream fs = new FileStream(Folder + "/object_frostland_lava_tree_l_001.drs", FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII))
                {
                    try
                    {
                        newDRSFile.Write(bw);
                    }
                    catch (Exception ex)
                    {
                        MainWindow.LogMessage($"[ERROR] {ex}");
                    }
                }
            }

            MainWindow.LogMessage($"[INFO] File has been exported.");
        }
    }
}