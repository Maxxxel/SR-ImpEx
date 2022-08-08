using Assimp;
using SR_ImpEx.Structures.GLTFFile;
using System;
using System.IO;

namespace SR_ImpEx.Helpers
{
    public class CollisionShape
    {
        public byte Version { get; }
        public int NumberBoxes { get; }
        public CollisionBox[] Boxes { get; }
        public int NumberSpheres { get; }
        public CollisionSphere[] Spheres { get; }
        public int NumberCylinders { get; }
        public CollisionCylinder[] Cylinders { get; }
        public CollisionShape(GLTF gltf)
        {
            Version = 1;
            NumberBoxes = 0; // WIP
            Boxes = new CollisionBox[NumberBoxes];

            if (NumberBoxes > 0)
            {
                for (int i = 0; i < NumberBoxes; i++)
                {
                    // ...
                }
            }

            NumberSpheres = 0; // WIP
            Spheres = new CollisionSphere[NumberSpheres];

            if (NumberSpheres > 0)
            {
                for (int i = 0; i < NumberSpheres; i++)
                {
                    // ...
                }
            }

            NumberCylinders = 0; // WIP
            Cylinders = new CollisionCylinder[NumberCylinders];

            if (NumberCylinders > 0)
            {
                for (int i = 0; i < NumberCylinders; i++)
                {
                    // ...
                }
            }
        }

        public CollisionShape(Scene model)
        {
            Version = 1;
            NumberBoxes = 0; // WIP
            Boxes = new CollisionBox[NumberBoxes];

            if (NumberBoxes > 0)
            {
                for (int i = 0; i < NumberBoxes; i++)
                {
                    // ...
                }
            }

            NumberSpheres = 0; // WIP
            Spheres = new CollisionSphere[NumberSpheres];

            if (NumberSpheres > 0)
            {
                for (int i = 0; i < NumberSpheres; i++)
                {
                    // ...
                }
            }

            NumberCylinders = 0; // WIP
            Cylinders = new CollisionCylinder[NumberCylinders];

            if (NumberCylinders > 0)
            {
                for (int i = 0; i < NumberCylinders; i++)
                {
                    // ...
                }
            }
        }

        internal int Size()
        {
            return 13 + (NumberCylinders * 44) + (NumberSpheres * 64) + (NumberBoxes * 72);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Version);
            bw.Write(NumberBoxes);
            if (NumberBoxes > 0)
            {
                for (int i = 0; i < NumberBoxes; i++)
                {
                    // ...
                }
            }
            bw.Write(NumberSpheres);
            if (NumberSpheres > 0)
            {
                for (int i = 0; i < NumberSpheres; i++)
                {
                    // ...
                }
            }
            bw.Write(NumberCylinders);
            if (NumberCylinders > 0)
            {
                for (int i = 0; i < NumberCylinders; i++)
                {
                    // ...
                }
            }
        }
    }
}