using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.GLTFFile
{
    public class GLTF
    {
        public StaticMesh staticMesh { get; set; }
        public ModelRoot Root { get; }
        public bool SkinnedModel { get; }
        public bool CollisionShaped { get; internal set; }
        public float ModelSize { get; }
        private bool HasEnoughTriangles(Scene scene)
        {
            int TriangleCount = Toolkit.EvaluateTriangles(scene).ToList().Count;

            return TriangleCount <= short.MaxValue;
        }
        private bool IsSkinnedModel(ModelRoot root)
        {
            return root.LogicalSkins.Count > 0;
        }
        public GLTF(string filePath)
        {
            Root = ModelRoot.Load(filePath);
            
            var BB = SharpGLTF.Runtime.MeshDecoder.EvaluateBoundingBox(Root.DefaultScene);
            // Set MainWindow.Model_Size to the size of the model (for collision detection) calculated from the bounding box BB, display as meters.
            ModelSize = BB.Max.Y - BB.Min.Y; // (Blender Z)

            /*
             * .bmg file as entrance
             * _module1_s0, _module1_s2
             * 
             * 
             * 
             * 
            */

            if (!HasEnoughTriangles(Root.DefaultScene))
            {
                MainWindow.LogMessage("Model has too many triangles. Please reduce the number of triangles.");
                return;
            }

            if (IsSkinnedModel(Root))
            {
                // Unsupported, so we will make it a static model.
                // Write that to the User
                MainWindow.LogMessage("[WARN] Skinned model detected. This is not supported. Converting to static model.");
            }

            // If its static it has a Collision Class
            CollisionShaped = true;
        }

        public float GetModelSize()
        {
            var BB = SharpGLTF.Runtime.MeshDecoder.EvaluateBoundingBox(Root.DefaultScene);
            float Y = BB.Max.Y - BB.Min.Y; // (Blender Z)

            return Y;
        }
    }
}
