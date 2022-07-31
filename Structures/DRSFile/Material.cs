using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Material
    {
        public int Identifier { get; }
        public float Smoothness { get; }
        public float Metalness { get; }
        public float Reflectivity { get; }
        public float Emissivity { get; }
        public float RefractionScale { get; }
        public float DistortionMeshScale { get; }
        public float Scratch { get; }
        public float SpecularScale { get; }
        public float WindResponse { get; }
        public float WindHeight { get; }
        public float DepthWriteThreshold { get; }
        public float Saturation { get; }
        public Material(FileWrapper file)
        {
            Identifier = file.ReadInt();

            switch (Identifier)
            {
                case 1668510769:
                    Smoothness = file.ReadFloat();
                    break;
                case 1668510770:
                    Metalness = file.ReadFloat();
                    break;
                case 1668510771:
                    Reflectivity = file.ReadFloat();
                    break;
                case 1668510772:
                    Emissivity = file.ReadFloat();
                    break;
                case 1668510773:
                    RefractionScale = file.ReadFloat();
                    break;
                case 1668510774:
                    DistortionMeshScale = file.ReadFloat();
                    break;
                case 1935897704:
                    Scratch = file.ReadFloat();
                    break;
                case 1668510775:
                    SpecularScale = file.ReadFloat();
                    break;
                case 1668510776:
                    WindResponse = file.ReadFloat();
                    break;
                case 1668510777:
                    WindHeight = file.ReadFloat();
                    break;
                case 1935893623:
                    DepthWriteThreshold = file.ReadFloat();
                    break;
                case 1668510785:
                    Saturation = file.ReadFloat();
                    break;
                default:
                    // Unknown Property, add to the list
                    file.ReadFloat();
                    break;
            }
        }
        public Material(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p, int i, int meshIndex)
        {
            switch (i)
            {
                case 0:
                    Identifier = 1668510769;
                    Smoothness = 0; // No use
                    break;
                case 1:
                    Identifier = 1668510770;
                    Metalness = 0; // No use
                    break;
                case 2:
                    Identifier = 1668510771;
                    Reflectivity = 0; // No use
                    break;
                case 3:
                    Identifier = 1668510772;
                    Emissivity = 0; // No use
                    break;
                case 4:
                    Identifier = 1668510773;
                    RefractionScale = 1; // default
                    break;
                case 5:
                    Identifier = 1668510774;
                    DistortionMeshScale = 0; // No use
                    break;
                case 6:
                    Identifier = 1935897704;
                    Scratch = 0; // No use
                    break;
                case 7:
                    Identifier = 1668510775;
                    SpecularScale = 1.5f; // looks like default
                    break;
                case 8:
                    Identifier = 1668510776;
                    WindResponse = 0; // No use
                    break;
                case 9:
                    Identifier = 1668510777;
                    WindHeight = 0; // No use
                    break;
                case 10:
                    Identifier = 1935893623;
                    DepthWriteThreshold = 0.5f; // looks like default
                    break;
                case 11:
                    Identifier = 1668510785;
                    Saturation = 1; // default
                    break;
                default:
                    break;
            }
        }
        internal void Write(BinaryWriter bw)
        {
            bw.Write(Identifier);

            switch (Identifier)
            {
                case 1668510769:
                    bw.Write(Smoothness);
                    break;
                case 1668510770:
                    bw.Write(Metalness);
                    break;
                case 1668510771:
                    bw.Write(Reflectivity);
                    break;
                case 1668510772:
                    bw.Write(Emissivity);
                    break;
                case 1668510773:
                    bw.Write(RefractionScale);
                    break;
                case 1668510774:
                    bw.Write(DistortionMeshScale);
                    break;
                case 1935897704:
                    bw.Write(Scratch);
                    break;
                case 1668510775:
                    bw.Write(SpecularScale);
                    break;
                case 1668510776:
                    bw.Write(WindResponse);
                    break;
                case 1668510777:
                    bw.Write(WindHeight);
                    break;
                case 1935893623:
                    bw.Write(DepthWriteThreshold);
                    break;
                case 1668510785:
                    bw.Write(Saturation);
                    break;
                default:
                    // Unknown Property, write 0.0f
                    bw.Write(0.0f);
                    break;
            }
        }
    }
}