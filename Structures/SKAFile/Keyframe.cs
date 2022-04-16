using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.Structures.SKAFile
{
    public class Keyframe
    {
        public Keyframe(FileWrapper file)
        {
            X = file.ReadFloat();       //float valX 
            Y = file.ReadFloat();       //float valY 
            Z = file.ReadFloat();       //float valZ 
            W = file.ReadFloat();       //float valW //for type==0 valW==1 
            TanX = file.ReadFloat();    //float tanX //tangents for animation curve 
            TanY = file.ReadFloat();    //float tanY 
            TanZ = file.ReadFloat();    //float tanZ 
            TanW = file.ReadFloat();	//float tanW //for type==0 tanW==0 
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float W { get; }
        public float TanX { get; }
        public float TanY { get; }
        public float TanZ { get; }
        public float TanW { get; }
    }
}
