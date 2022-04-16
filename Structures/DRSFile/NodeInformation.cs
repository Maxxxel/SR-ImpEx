using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class NodeInformation
    {
        public int Magic { get; }
        public int Identifier { get; }
        public int Offset { get; }
        public int NodeSize { get; }
        public byte[] Spacer { get; }
        public NodeInformation(FileWrapper file)
        {
            Magic = file.ReadInt();
            Identifier = file.ReadInt();
            Offset = file.ReadInt();
            NodeSize = file.ReadInt();
            Spacer = file.ReadBytes(-1, 16);
        }

        public NodeInformation(string name, int identifier, int offset, int s)
        {
			switch (name)
			{
				case "CDspJointMap":
					Magic = -1340635850;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "CGeoMesh":
					Magic = 100449016;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "CGeoOBBTree":
					Magic = -933519637;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "CSkSkinInfo":
					Magic = -761174227;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "CDspMeshFile":
					Magic = -1900395636;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "DrwResourceMeta":
					Magic = -183033339;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "collisionShape":
					Magic = 268607026;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				case "CGeoPrimitiveContainer":
					Magic = 1396683476;
					Identifier = identifier;
					Offset = offset + Size();
					NodeSize = s;
					Spacer = new byte[16];
					break;
				default:
					MainWindow.LogMessage($"[WARN] Unsupported NodeInformation => {name}!");
					break;
			}
		}
		public int Size()
		{
			return 32;
		}

        internal void Write(BinaryWriter bw)
        {
			bw.Write(Magic);
			bw.Write(Identifier);
			bw.Write(Offset);
			bw.Write(NodeSize);
			bw.Write(Spacer);
		}
    }
}
