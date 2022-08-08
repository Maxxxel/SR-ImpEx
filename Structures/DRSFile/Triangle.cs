﻿using Assimp;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Triangle
    {
        public short[] Indices { get; }

        public Triangle(FileWrapper file)
        {
            Indices = file.ReadShorts(-1, 3);
        }

        public Triangle((int, int, int) tri)
        {
            Indices = new short[] { (short)tri.Item1, (short)tri.Item2, (short)tri.Item3 };
        }

        public Triangle(Face face)
        {
            Indices = new short[] { (short)face.Indices[0], (short)face.Indices[1], (short)face.Indices[2] };
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Indices[0]);
            bw.Write(Indices[1]);
            bw.Write(Indices[2]);
        }
    }
}