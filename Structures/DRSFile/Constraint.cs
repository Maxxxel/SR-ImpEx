using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class Constraint
    {
        public Constraint(FileWrapper file)
        {
            Revision = file.ReadShort();

            if (Revision == 1)
            {
                LeftAngle = file.ReadFloat();
                RightAngle = file.ReadFloat();
                LeftDampStart = file.ReadFloat();
                RightDampStart = file.ReadFloat();
                DampRatio = file.ReadFloat();
            }
        }

        public short Revision { get; }
        public float LeftAngle { get; }
        public float RightAngle { get; }
        public float LeftDampStart { get; }
        public float RightDampStart { get; }
        public float DampRatio { get; }
    }
}
