using SR_ImpEx.Helpers;
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
    }
}