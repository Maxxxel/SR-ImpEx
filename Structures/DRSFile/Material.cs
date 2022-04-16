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
        public int Length { get; }
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
        public float UnknownProperty { get; }
        public Material(FileWrapper file)
        {
            Length = file.ReadInt();

            for (int i = 0; i < Length; i++)
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
                        UnknownProperty = file.ReadFloat();
                        break;
                    default:
                        // Unknown Property, add to the list
                        file.ReadFloat();
                        break;
                }
            }
        }

        public Material(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p)
        {
            Length = 0;

            if (Length != 0) // WIP
            {
                // ... Console Window or from the file itself? We need to test these things ingame if they work
            }
        }

        internal int Size()
        {
            return 4 + (Length * 8);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);
        }
    }
}