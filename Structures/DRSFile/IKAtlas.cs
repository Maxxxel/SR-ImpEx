using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class IKAtlas
    {
        public IKAtlas(FileWrapper file)
        {
            Identifier = file.ReadInt();
            Version = file.ReadShort();

            if (Version >= 1)
            {
                Axis = file.ReadInt();
                ChainOrder = file.ReadInt();
                Constraints = new Constraint[3];

                for (int u = 0; u < 3; u++) Constraints[u] = new Constraint(file);
            }

            if (Version >= 2)
            {
                PurposeFlags = file.ReadShort();
            }
        }

        public int Identifier { get; }
        public short Version { get; }
        public int Axis { get; }
        public int ChainOrder { get; }
        public Constraint[] Constraints { get; }
        public short PurposeFlags { get; }
    }
}
