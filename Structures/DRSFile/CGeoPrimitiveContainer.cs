﻿using Assimp;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.IO;

namespace SR_ImpEx.Helpers
{
    public class CGeoPrimitiveContainer
    {
        public CGeoPrimitiveContainer(GLTF gltf)
        {
        }

        public CGeoPrimitiveContainer(Scene model)
        {
        }

        internal int Size()
        {
            return 0;
        }

        internal void Write(BinaryWriter bw)
        {
        }
    }
}