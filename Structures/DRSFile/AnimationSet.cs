using SR_ImpEx.Helpers;

namespace SR_ImpEx.Structures.DRSFile
{
    public class AnimationSet
    {
        public float fly_bank_scale { get; }
        public float fly_accel_scale { get; }
        public float fly_hit_scale { get; }
        public int uk { get; }
        public int Length { get; }
        public string Version { get; }
        public int Count { get; }
        public float DefaultRunSpeed { get; }
        public float DefaultWalkSpeed { get; }
        public int ModeAnimationKeyCount { get; }
        public int Revision { get; }
        public byte ModeChangeType { get; }
        public byte HoveringGround { get; }
        public byte AlignToTerrain { get; }
        public ModeAnimationKey[] ModeAnimationKeys { get; }
        public short HasAtlas { get; }
        public int AtlasCount { get; }
        public IKAtlas[] IKAtlas { get; }
        public int UnknownStructureLength { get; }
        public UnknownStructure[] UnknownStructures { get; }
        public int AnimationMarkerCount { get; }
        public AnimationMarkerSet[] AnimationMarkerSets { get; }
        public int Unknown53 { get; }
        public int UnknownStructureLength2 { get; }
        public UnknownStructure2[] UnknownStructures2 { get; }
        public AnimationSet(FileWrapper file)
        {
            Length = file.ReadInt();
            Version = file.ReadString(Length); // Battleforge
            Count = file.ReadInt();
            DefaultRunSpeed = file.ReadFloat();
            DefaultWalkSpeed = file.ReadFloat();

            if (Count == 2)
            {
                ModeAnimationKeyCount = file.ReadInt();
            }
            else
            {
                Revision = file.ReadInt();
            }

            if (Count == 6)
            {
                if (Revision >= 2)
                {
                    ModeChangeType = file.ReadByte();
                    HoveringGround = file.ReadByte();
                }

                if (Revision >= 5)
                {
                    fly_bank_scale = file.ReadFloat();
                    fly_accel_scale = file.ReadFloat();
                    fly_hit_scale = file.ReadFloat();
                }

                if (Revision >= 6)
                    AlignToTerrain = file.ReadByte();
            }

            if (Count == 2)
            {
                uk = file.ReadInt();
            }
            else
            {
                uk = 0;
                ModeAnimationKeyCount = file.ReadInt();
            }

            ModeAnimationKeys = new ModeAnimationKey[ModeAnimationKeyCount];

            for (int a = 0; a < ModeAnimationKeyCount; a++) ModeAnimationKeys[a] = new ModeAnimationKey(file, uk);

            if (Count >= 3)
            {
                HasAtlas = file.ReadShort();

                if (HasAtlas >= 1)
                {
                    AtlasCount = file.ReadInt();
                    IKAtlas = new IKAtlas[AtlasCount];

                    for (int u = 0; u < AtlasCount; u++) IKAtlas[u] = new IKAtlas(file);
                }

                if (HasAtlas >= 2)
                {
                    UnknownStructureLength = file.ReadInt();
                    UnknownStructures = new UnknownStructure[UnknownStructureLength];

                    for (int uu = 0; uu < UnknownStructureLength; uu++) UnknownStructures[uu] = new UnknownStructure(file);
                }
            }
            if (Count >= 4)
            {
                Revision = file.ReadShort();

                if (Revision == 2)
                {
                    AnimationMarkerCount = file.ReadInt();
                    AnimationMarkerSets = new AnimationMarkerSet[AnimationMarkerCount];

                    for (int u = 0; u < AnimationMarkerCount; u++) AnimationMarkerSets[u] = new AnimationMarkerSet(file);
                }
                else if (Revision == 1)
                {
                    UnknownStructureLength2 = file.ReadInt();
                    UnknownStructures2 = new UnknownStructure2[UnknownStructureLength2];

                    for (int u = 0; u < UnknownStructureLength2; u++) UnknownStructures2[u] = new UnknownStructure2(file);
                }
            }
        }
    }
}