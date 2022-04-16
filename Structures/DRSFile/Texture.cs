using CSharpImageLibrary;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Texture
    {
        public int Identifier { get; }
        public int Length { get; }
        private ChannelBuilder Channel { get; }
        public string Name { get; }
        public int Spacer { get; }
        public MemoryImage Content { get; }

        public Texture(FileWrapper file)
        {
            Identifier = file.ReadInt();
            Length = file.ReadInt();
            Name = file.ReadString(Length);
            Spacer = file.ReadInt();
        }

        public Texture(ChannelBuilder channel, int v)
        {
            Channel = channel;
            string N = 
            Name = Channel.Texture.Name + ".dds";
            Length = Name.Length;
            Spacer = 0;
            Content = channel.Texture.PrimaryImage.Content; // Internal Value

            switch (v)
            {
                case 0:
                    Identifier = 1684432499; // ColorMap
                    break;
                case 1:
                    Identifier = 1852992883; // NormalMap
                    break;
                default:
                    MainWindow.LogMessage($"[WARN] Unsupported Texture Parameter => {v}!");
                    break;
            }
        }

        internal int Size()
        {
            return 12 + Length;
        }

        internal void Write(BinaryWriter bw)
        {
            string tempFileName = "";
            string fileName = "";

            string CurrentPath = AppContext.BaseDirectory;
            if (!Directory.Exists(Path.Combine(CurrentPath, "DRS_Exports"))) Directory.CreateDirectory(Path.Combine(CurrentPath, "DRS_Exports"));
            CurrentPath = Path.Combine(CurrentPath, "DRS_Exports");

            switch (Identifier)
            {
                case 1684432499:
                    tempFileName = CurrentPath + "/_temporary_" + Channel.Texture.Name + "." + Channel.Texture.PrimaryImage.Content.FileExtension;
                    fileName = CurrentPath + "/" + Channel.Texture.Name + ".dds";

                    if (!File.Exists(fileName))
                    {
                        Channel.Texture.PrimaryImage.Content.SaveToFile(tempFileName);
                        ImageEngineImage img1 = new ImageEngineImage(tempFileName, 1024);
                        img1.Save(fileName, new ImageFormats.ImageEngineFormatDetails(ImageEngineFormat.DDS_DXT5), MipHandling.Default);
                    }
                    break;
                case 1852992883:
                    tempFileName = CurrentPath + "/_temporary_" + Channel.Texture.Name + "." + Channel.Texture.PrimaryImage.Content.FileExtension;
                    fileName = CurrentPath + "/" + Channel.Texture.Name + ".dds";

                    if (!File.Exists(fileName))
                    {
                        Channel.Texture.PrimaryImage.Content.SaveToFile(tempFileName);
                        ImageEngineImage img1 = new ImageEngineImage(tempFileName, 1024);
                        img1.Save(fileName, new ImageFormats.ImageEngineFormatDetails(ImageEngineFormat.DDS_DXT5), MipHandling.Default);
                    }
                    break;
                default:
                    MainWindow.LogMessage($"[WARN] Unsupported Texture Parameter => {Identifier}!");
                    break;
            }

            bw.Write(Identifier);
            bw.Write(Length);
            bw.Write(Name.ToCharArray());
            bw.Write(Spacer);
        }
    }
}