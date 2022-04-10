using SharpGLTF.Scenes;
using SR_ImpEx.Structures;
using SR_ImpEx.Structures.DRSFile;
using System;
using System.Linq;
using System.Numerics;

namespace SR_ImpEx.Structures.GLTFFile
{
    public class GLTFSkeleton
    {
        private int count { get; set; }
        public NodeBuilder[] Binds { get; }
        public NodeBuilder Root { get; }
        public GLTFSkeleton(CSkSkeleton skeleton)
        {
            Binds = new NodeBuilder[skeleton.BoneCount];
            Bone SkeletonRoot = skeleton.Bones.Where(b => b.Identifier == 0).Single();
            Root = new NodeBuilder(SkeletonRoot.Name);
            BoneVertex[] BV = skeleton.BoneMatrices[0].BoneVertices;
            Vector4 vec1 = new Vector4(BV[0].X, BV[0].Y, BV[0].Z, BV[0].ParentReference);
            Vector4 vec2 = new Vector4(BV[1].X, BV[1].Y, BV[1].Z, BV[1].ParentReference);
            Vector4 vec3 = new Vector4(BV[2].X, BV[2].Y, BV[2].Z, BV[2].ParentReference);
            Vector4 vec4 = new Vector4(BV[3].X, BV[3].Y, BV[3].Z, BV[3].ParentReference);

            Matrix4x4 matrix = new Matrix4x4(
                vec1.X, vec1.Y, vec1.Z, -vec4.X,
                vec2.X, vec2.Y, vec2.Z, -vec4.Y,
                vec3.X, vec3.Y, vec3.Z, -vec4.Z,
                0, 0, 0, 1
            );

            Quaternion Rotation = MathHelpers.ExtractRotation(matrix);
            Vector3 Position = MathHelpers.ExtractPosition(matrix);
            Position = MathHelpers.Transform(Position, Rotation);
            Vector3 Scale = MathHelpers.ExtractScale(matrix);
            Matrix4x4.Invert(Root.WorldMatrix, out Matrix4x4 inverse);

            Root
                .WithLocalTranslation(Position)
                .WithLocalRotation(Rotation)
                .WithLocalScale(Scale);

            matrix = Root.LocalMatrix * inverse;
            if (matrix.M44 != 1.0f) matrix.M44 = 1.0f;
            Root.LocalMatrix = matrix;

            Binds[count] = Root;
            count++;

            BuildSkeleton(SkeletonRoot, Root, skeleton);
        }

        private void BuildSkeleton(Bone parent, NodeBuilder parentNode, CSkSkeleton skeleton)
        {
            if (parent.ChildCount > 0) Array.Sort(parent.Children);

            for (int i = 0; i < parent.ChildCount; i++)
            {
                Bone Child = skeleton.Bones.Where(b => b.Identifier == parent.Children[i]).Single();
                NodeBuilder child = parentNode.CreateNode(Child.Name);
                BoneVertex[] BV = skeleton.BoneMatrices[parent.Children[i]].BoneVertices;

                Vector4 vec1 = new Vector4(BV[0].X, BV[0].Y, BV[0].Z, BV[0].ParentReference);
                Vector4 vec2 = new Vector4(BV[1].X, BV[1].Y, BV[1].Z, BV[1].ParentReference);
                Vector4 vec3 = new Vector4(BV[2].X, BV[2].Y, BV[2].Z, BV[2].ParentReference);
                Vector4 vec4 = new Vector4(BV[3].X, BV[3].Y, BV[3].Z, BV[3].ParentReference);

                Matrix4x4 matrix = new Matrix4x4(
                    vec1.X, vec1.Y, vec1.Z, -vec4.X,
                    vec2.X, vec2.Y, vec2.Z, -vec4.Y,
                    vec3.X, vec3.Y, vec3.Z, -vec4.Z,
                    0, 0, 0, 1
                );

                Quaternion Rotation = MathHelpers.ExtractRotation(matrix);
                Vector3 Position = MathHelpers.ExtractPosition(matrix);
                Position = MathHelpers.Transform(Position, Rotation);
                Vector3 Scale = MathHelpers.ExtractScale(matrix);
                Matrix4x4.Invert(child.WorldMatrix, out Matrix4x4 inverse);

                child
                    .WithLocalTranslation(Position)
                    .WithLocalRotation(Rotation)
                    .WithLocalScale(Scale);

                matrix = child.LocalMatrix * inverse;
                if (matrix.M44 != 1.0f) matrix.M44 = 1.0f;
                child.LocalMatrix = matrix;

                Binds[count] = child;
                count++;
                BuildSkeleton(Child, child, skeleton);
            }
        }
    }
}