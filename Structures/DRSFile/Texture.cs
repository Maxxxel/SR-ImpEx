using Assimp;
using DdsFileTypePlus;
using PaintDotNet;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SR_ImpEx.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Texture
    {
        public int Identifier { get; }
        public Bitmap RefractionMap { get; private set; }
        public int Length { get; }
        private ChannelBuilder Channel { get; }
        public string Name { get; }
        public int Spacer { get; }
        public MemoryImage Content { get; }
        private Bitmap EmissivityMap { get; set; }
        public Texture(FileWrapper file)
        {
            Identifier = file.ReadInt();
            Length = file.ReadInt();
            Name = file.ReadString(Length);
            Spacer = file.ReadInt();
        }
        public Texture(ChannelBuilder channel, int v, Bitmap additionalMap)
        {
            Channel = channel;
            Name = Channel.Texture.Name.Replace(".", "_");
            Length = Name.Length;
            Spacer = 0;
            Content = channel.Texture.PrimaryImage.Content; // Internal Value

            switch (v)
            {
                case 0:
                    Identifier = 1684432499; // ColorMap
                    EmissivityMap = additionalMap;
                    break;
                case 1:
                    Identifier = 1852992883; // NormalMap
                    break;
                case 2:
                    Identifier = 1936745324; // ParamMap
                    EmissivityMap = additionalMap;
                    break;
                case 3:
                    Identifier = 1919116143; // RefractionMap
                    RefractionMap = additionalMap;
                    Name = Name.Replace("_col", "_ref");
                    break;
                default:
                    MainWindow.LogMessage($"[WARN] Unsupported Texture Parameter => {v}!");
                    break;
            }
        }
        public Texture(string img_name, int v, Bitmap bitmap, Bitmap emissivityMap)
        {
            Name = img_name.Replace(".", "_");
            Length = Name.Length;
            Spacer = 0;
            Content = new MemoryImage(Helpers.ImageConverter.ImageToByte(bitmap)); // Internal Value

            switch (v)
            {
                case 0:
                    Identifier = 1684432499; // ColorMap
                    EmissivityMap = emissivityMap;
                    break;
                case 1:
                    Identifier = 1852992883; // NormalMap
                    break;
                case 2:
                    Identifier = 1936745324; // ParamMap
                    EmissivityMap = emissivityMap;
                    break;
                default:
                    MainWindow.LogMessage($"[WARN] Unsupported Texture Parameter => {v}!");
                    break;
            }
        }

        public Texture(Mesh currentMesh, int i)
        {
        }

        internal int Size()
        {
            return 12 + Length;
        }
        internal void Write(BinaryWriter bw)
        {
            string fileName = "";
            byte[] Image = new byte[0];

            if (Identifier == 1684432499 || Identifier == 1852992883 || Identifier == 1936745324 || Identifier == 1919116143)
            {
                if (Channel != null)
                {
                    fileName = Exporter.Folder + "/" + Name + ".dds";
                    Image = Channel.Texture.PrimaryImage.Content.Content.ToArray();
                }
                else
                {
                    fileName = Exporter.Folder + "/" + Name + ".dds";
                    Image = Content.Content.ToArray();
                }

                if (!File.Exists(fileName))
                {
                    MainWindow.LogMessage($"[INFO] File: {fileName} does not exist, writing to file...");
                    Bitmap bitmap = new Bitmap(new MemoryStream(Image));

                    if (Channel.Key == KnownChannel.BaseColor && EmissivityMap != null)
                    {
                        // We need to switch the colors from emissivity map to BaseColor
                        Bitmap temp = new Bitmap(EmissivityMap);
                        MainWindow.LogMessage("[INFO] Add Transparency to a copy of the Emission Map.");
                        temp = Helpers.ImageConverter.DropAlphaChannel(temp, true);
                        MainWindow.LogMessage("[INFO] Merging parts of Emissivity Map into BaseColor.");
                        bitmap = Helpers.ImageConverter.MergeImages(bitmap, temp);
                    }

                    if (Channel.Key == KnownChannel.MetallicRoughness)
                    {
                        // We need to exchange the red channel with blue channel
                        // cause GLTF uses metal on blue and AO on red
                        MainWindow.LogMessage("[INFO] Switching Red and Blue Channels of Parameter Map for Compatibility with GLTF 2.0 Standard.");
                        bitmap = Helpers.ImageConverter.SwapRedAndBlueChannels(bitmap);
                        // now we need to drop the blue map as its reserved for enviornment or fluid areas
                        MainWindow.LogMessage("[INFO] Dropping Blue Channel of Parameter Map for Compatibility cause of different use in Battleforge.");
                        bitmap = Helpers.ImageConverter.DropBlueChannel(bitmap);
                        // last step is droping the alpha channel when there is no emission texture
                        if (EmissivityMap != null)
                        {
                            MainWindow.LogMessage("[INFO] Applying Alpha Channel to Parameter Map for Compatibility.");
                            bitmap = Helpers.ImageConverter.ApplyAlphaChannel(bitmap, EmissivityMap);
                        }
                        else
                        {
                            MainWindow.LogMessage("[INFO] Dropping Alpha Channel of Parameter Map for Compatibility.");
                            bitmap = Helpers.ImageConverter.DropAlphaChannel(bitmap);
                        }
                    }

                    if (Channel.Key == KnownChannel.BaseColor && RefractionMap != null)
                    {
                        // We need to Create a new Map
                        MainWindow.LogMessage("[INFO] Applying Alpha Channel to Refraction Map for Compatibility.");
                        Bitmap temp = new Bitmap(bitmap);
                        //temp = Helpers.ImageConverter.ApplyAlphaChannel(temp);
                        bitmap = temp;
                    }

                    if (Channel.Key == KnownChannel.BaseColor && RefractionMap == null && EmissivityMap == null)
                    {
                        //bitmap = Helpers.ImageConverter.SetImageOpacity(bitmap, 1f);
                    }

                    try
                    {
                        Surface processedSurface = Surface.CopyFromBitmap(bitmap);
                        FileStream fileStream = File.OpenWrite(fileName);
                        DdsFile.Save(
                            fileStream,
                            DdsFileFormat.BC3,
                            DdsErrorMetric.Perceptual,
                            BC7CompressionMode.Fast,
                            cubeMap: true,
                            generateMipmaps: true,
                            ResamplingAlgorithm.SuperSampling,
                            processedSurface,
                            progressCallback
                        );

                        fileStream.Close();
                        MainWindow.LogMessage("[INFO] File saved.");
                    }
                    catch (Exception ex)
                    {
                        MainWindow.LogMessage($"[ERROR] {ex}");
                    }
                }
                else
                {
                    MainWindow.LogMessage($"[INFO] File: {fileName} already exists, skipping...");
                }
            }
            else
            {
                MainWindow.LogMessage($"[WARN] Unsupported Texture Parameter => {Identifier}!");
            }

            bw.Write(Identifier);
            bw.Write(Length);
            bw.Write(Name.ToCharArray());
            bw.Write(Spacer);
        }

        private void progressCallback(object sender, ProgressEventArgs e)
        {
            //progressBar1.Value = (int)Math.Round(e.Percent);
        }
    }
}