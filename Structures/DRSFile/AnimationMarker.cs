﻿using SR_ImpEx.Helpers;
using System.Numerics;

namespace SR_ImpEx.Structures.DRSFile
{
    public class AnimationMarker
    {
        public AnimationMarker(FileWrapper file)
        {
            Class = file.ReadInt();
            Time = file.ReadFloat();
            Direction = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
            Position = new Vector3(file.ReadFloat(), file.ReadFloat(), file.ReadFloat());
        }

        public int Class { get; }
        public float Time { get; }
        public Vector3 Direction { get; }
        public Vector3 Position { get; }
    }
}
