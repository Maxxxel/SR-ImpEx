using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.GLTFFile
{
    public class GLTF
    {
        public readonly StaticMesh staticMesh;

        public ModelRoot Root { get; }
        public bool SkinnedModel { get; }
        public bool CollisionShaped { get; internal set; }

        public GLTF(string filePath)
        {
            Root = ModelRoot.Load(filePath);

            /*
             * .bmg file as entrance
             * _module1_s0, _module1_s2
             * 
             * 
             * 
             * 
            */

            // Check Triangle Count
            int TriangleCount = Toolkit.EvaluateTriangles(Root.DefaultScene).ToList().Count;

            if (TriangleCount > short.MaxValue)
            {
                MainWindow.LogMessage($"[ERRR] Model has more than 32.768 Triangles => {TriangleCount}! Please try to reduce them.");
                return;
            }

            // Check what type of Object we got
            if (Root.LogicalSkins.Count > 0) SkinnedModel = true;

            if (SkinnedModel)
            {
                MainWindow.LogMessage("[ERRO] Loading skinned model still not supported.");
                return;
            }
            else
            {
                CollisionShaped = true;
                MainWindow.LogMessage("[INFO] Loading static model.");
                staticMesh = new StaticMesh(Root);
            }
        }
    }
}
