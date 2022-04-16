using SharpGLTF.Scenes;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

            string CurrentDir = Directory.GetCurrentDirectory();
            SharpGLTF.Schema2.ModelRoot GLTF = DRS_Scene.ToGltf2();
            GLTF.SaveGLB(CurrentDir + "/GLTF_Exports/" + fileName.Replace(".drs", "") + ".glb");

            string[] tmpList = Directory.GetFiles(CurrentDir + "/GLTF_Exports/", "_temporary_*");
            foreach (string f in tmpList)
            {
                File.Delete(f);
            }

            MainWindow.LogMessage("[INFO] File has been exported.");
        }
    }
}
