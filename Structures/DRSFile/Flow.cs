using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class Flow
    {
        public int Length { get; }
        public int Identifier1 { get; }
        public Vector4 MaxFlowSpeed { get; }
        public int Identifier2 { get; }
        public Vector4 MinFlowSpeed { get; }
        public int Identifier3 { get; }
        public Vector4 FlowSpeedChange { get; }
        public int Identifier4 { get; }
        public Vector4 FlowScale { get; }
        public Flow(FileWrapper file)
        {
            Length = file.ReadInt();

            if (Length == 4)
            {
                Identifier1 = file.ReadInt();
                MaxFlowSpeed = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
                Identifier2 = file.ReadInt();
                MinFlowSpeed = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
                Identifier3 = file.ReadInt();
                FlowSpeedChange = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
                Identifier4 = file.ReadInt();
                FlowScale = new Vector4(file.ReadFloat(), file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            }
        }

        public Flow(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p)
        {
            Length = 0; // WIP: 4 if we have flowing Textures

            if (Length == 4)
            {
                Identifier1 = 1668707377;
                MaxFlowSpeed = new Vector4();
                Identifier2 = 1668707378;
                MinFlowSpeed = new Vector4();
                Identifier3 = 1668707379;
                FlowSpeedChange = new Vector4();
                Identifier4 = 1668707380;
                FlowScale = new Vector4();
            }
        }

        internal int Size()
        {
            return 4 + (Length * 20);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);

            if (Length == 4)
            {
                bw.Write(Identifier1);
                bw.Write(MaxFlowSpeed.X);
                bw.Write(MaxFlowSpeed.Y);
                bw.Write(MaxFlowSpeed.Z);
                bw.Write(MaxFlowSpeed.W);
                bw.Write(Identifier2);
                bw.Write(MinFlowSpeed.X);
                bw.Write(MinFlowSpeed.Y);
                bw.Write(MinFlowSpeed.Z);
                bw.Write(MinFlowSpeed.W);
                bw.Write(Identifier3);
                bw.Write(FlowSpeedChange.X);
                bw.Write(FlowSpeedChange.Y);
                bw.Write(FlowSpeedChange.Z);
                bw.Write(FlowSpeedChange.W);
                bw.Write(Identifier4);
                bw.Write(FlowScale.X);
                bw.Write(FlowScale.Y);
                bw.Write(FlowScale.Z);
                bw.Write(FlowScale.W);
            }
        }
    }
}