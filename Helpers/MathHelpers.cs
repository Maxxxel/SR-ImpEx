using System;
using System.Numerics;

namespace SR_ImpEx.Structures.GLTFFile
{
    public class MathHelpers
    {
        public static Vector3 ExtractPosition(Matrix4x4 matrix)
        {
            Vector3 position;
            position.X = matrix.M14;
            position.Y = matrix.M24;
            position.Z = matrix.M34;
            return position;
        }
        public static Quaternion ExtractRotation(Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.X = matrix.M13;
            forward.Y = matrix.M23;
            forward.Z = matrix.M33;

            Vector3 upwards;
            upwards.X = matrix.M12;
            upwards.Y = matrix.M22;
            upwards.Z = matrix.M32;

            return LookRotation(forward, upwards);
        }
        private static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
        {
            forward = Vector3.Normalize(forward);

            Vector3 vector = Vector3.Normalize(forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(upwards, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.X;
            var m01 = vector2.Y;
            var m02 = vector2.Z;
            var m10 = vector3.X;
            var m11 = vector3.Y;
            var m12 = vector3.Z;
            var m20 = vector.X;
            var m21 = vector.Y;
            var m22 = vector.Z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;

            return quaternion;
        }
        public static Vector3 Transform(Vector3 value, Quaternion rotation)
        {
            Vector3 vector;
            float num12 = rotation.X + rotation.X;
            float num2 = rotation.Y + rotation.Y;
            float num = rotation.Z + rotation.Z;
            float num11 = rotation.W * num12;
            float num10 = rotation.W * num2;
            float num9 = rotation.W * num;
            float num8 = rotation.X * num12;
            float num7 = rotation.X * num2;
            float num6 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num4 = rotation.Y * num;
            float num3 = rotation.Z * num;
            float num15 = (value.X * (1f - num5 - num3)) + (value.Y * (num7 - num9)) + (value.Z * (num6 + num10));
            float num14 = (value.X * (num7 + num9)) + (value.Y * (1f - num8 - num3)) + (value.Z * (num4 - num11));
            float num13 = (value.X * (num6 - num10)) + (value.Y * (num4 + num11)) + (value.Z * (1f - num8 - num5));
            vector.X = num15;
            vector.Y = num14;
            vector.Z = num13;
            return vector;
        }
        public static Vector3 ExtractScale(Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.X = new Vector4(matrix.M11, matrix.M21, matrix.M31, matrix.M41).Length();
            scale.Y = new Vector4(matrix.M12, matrix.M22, matrix.M32, matrix.M42).Length();
            scale.Z = new Vector4(matrix.M13, matrix.M23, matrix.M33, matrix.M43).Length();
            return scale;
        }
    }
}