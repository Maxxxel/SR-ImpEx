using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.IO;

namespace SR_ImpEx.Structures
{
    public class Textures
    {
        public int Length { get; }
        public Texture[] Texture { get; }
        public Textures(FileWrapper file)
        {
            Length = file.ReadInt();
            Texture = new Texture[Length];
            for (int i = 0; i < Length; i++) Texture[i] = new Texture(file);
        }

        public Textures(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p, int m)
        {
            foreach (ChannelBuilder channel in p.Material.Channels)
            {
                if (channel.Texture != null)
                {
                    switch (channel.Key)
                    {
                        case KnownChannel.BaseColor:
                            Length++;
                            break;
                        case KnownChannel.Normal:
                            Length++;
                            break;
                        default:
                            MainWindow.LogMessage($"[WARN] Unsupported Texture => {channel.Key}! Skipped!");
                            break;
                    }
                }
            }

            Texture = new Texture[Length];
            int Counter = 0;

            foreach (ChannelBuilder channel in p.Material.Channels)
            {
                if (channel.Texture != null)
                {
                    string img_name = "";

                    switch (channel.Key)
                    {
                        case KnownChannel.BaseColor:
                            img_name = channel.Texture.Name;

                            if (img_name == null || img_name.Length == 0)
                            {
                                if (channel.Texture.PrimaryImage.Name == null || channel.Texture.PrimaryImage.Name.Length == 0)
                                {
                                    if (channel.Texture.PrimaryImage.Content.SourcePath != null)
                                    {
                                        img_name = Path.GetFileName(channel.Texture.PrimaryImage.Content.SourcePath).Replace("." + channel.Texture.PrimaryImage.Content.FileExtension, "_col");
                                    }
                                }
                            }

                            // In case we cant find a name we use the hashcode
                            if (img_name == null || img_name.Length == 0)
                            {
                                string newName = channel.Texture.PrimaryImage.Content.GetHashCode().ToString() + "_custom_col";
                                img_name = newName;
                            }

                            channel.Texture.Name = img_name;
                            Texture[Counter] = new Texture(channel, 0);
                            Counter++;
                            break;
                        case KnownChannel.Normal:
                            img_name = channel.Texture.Name;

                            if (img_name == null || img_name.Length == 0)
                            {
                                if (channel.Texture.PrimaryImage.Name == null || channel.Texture.PrimaryImage.Name.Length == 0)
                                {
                                    if (channel.Texture.PrimaryImage.Content.SourcePath != null)
                                    {
                                        img_name = Path.GetFileName(channel.Texture.PrimaryImage.Content.SourcePath).Replace("." + channel.Texture.PrimaryImage.Content.FileExtension, "_nor");
                                    }
                                }
                            }

                            // In case we cant find a name we use the hashcode
                            if (img_name == null || img_name.Length == 0)
                            {
                                string newName = channel.Texture.PrimaryImage.Content.GetHashCode().ToString() + "_custom_nor";
                                img_name = newName;
                            }

                            channel.Texture.Name = img_name;
                            Texture[Counter] = new Texture(channel, 1);
                            Counter++;
                            break;
                        default:
                            MainWindow.LogMessage($"[WARN] Unsupported Texture => {channel.Key}, Skipping!");
                            break;
                    }
                }
            }
        }

        internal int Size()
        {
            int Add = 0;

            for (int i = 0; i < Length; i++)
            {
                Add += Texture[i].Size();
            }

            return 4 + Add;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);

            for (int i = 0; i < Length; i++)
            {
                Texture[i].Write(bw);
            }
        }
    }
}