using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SR_ImpEx.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;

namespace SR_ImpEx.Structures
{
    public class Textures
    {
        public int Length { get; }
        public Texture[] Texture { get; }
        public bool HasParameterMap { get; private set; }
        public bool HasNormalMap { get; private set; }
        public bool HasRefractionMap { get; private set; }
        public Textures(FileWrapper file)
        {
            Length = file.ReadInt();
            Texture = new Texture[Length];
            for (int i = 0; i < Length; i++) Texture[i] = new Texture(file);
        }
        public Textures(PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexTexture1, VertexEmpty> p, int m)
        {
            MainWindow.LogMessage($"[INFO] Expected Textures: {p.Material.Channels.Count}");
            ChannelBuilder[] TextureChannels = new ChannelBuilder[4]; // We only support 4 Textures so far
            ChannelBuilder[] ColorChannels = new ChannelBuilder[1]; // We only support 1 Plain Color
            int ColorCount = 0;
            int EmissivityCount = 0;
            Bitmap EmissivityMap = null;
            Bitmap RefractionMap = null;

            // Count the total number of Textures
            foreach (ChannelBuilder channel in p.Material.Channels)
            {
                if (channel.Texture != null && (channel.Key == KnownChannel.BaseColor || channel.Key == KnownChannel.Normal || channel.Key == KnownChannel.MetallicRoughness))
                {
                    // Real Texture
                    TextureChannels[Length] = channel;
                    Length++;

                    if (p.Material.AlphaMode == AlphaMode.BLEND && channel.Key == KnownChannel.BaseColor)
                    {
                        // Real Texture
                        RefractionMap = new Bitmap(new MemoryStream(channel.Texture.PrimaryImage.Content.Content.ToArray(), false));
                        TextureChannels[Length] = channel;
                        Length++;
                        HasRefractionMap = true;
                    }
                }
                else if (channel.Texture == null && channel.Key == KnownChannel.BaseColor)
                {
                    // Plain Color we need to create a Texture from
                    ColorChannels[ColorCount] = channel;
                    ColorCount++;
                    Length++;
                }
                else if (channel.Texture != null && channel.Key == KnownChannel.Emissive)
                {
                    // We need the emissivity Mapß internally to apply it on the alpha channel later
                    EmissivityMap = new Bitmap(new MemoryStream(channel.Texture.PrimaryImage.Content.Content.ToArray(), false));
                    EmissivityCount++;
                    HasParameterMap = true;
                }
                else if (channel.Parameters.Count > 1) { } // Just to hide the Warining on this one
                else
                {
                    MainWindow.LogMessage($"[WARN] Unsupported Texture => {channel.Key}({channel.Texture}/{channel.Parameters.Count})! Skipped!");
                }
            }

            MainWindow.LogMessage($"[INFO] Found {Length} Textures, {ColorCount} Colors, {EmissivityCount} Emissive Maps.");

            Texture = new Texture[Length];
            int Counter = 0;
            bool usedBaseMap = false;

            // Get the Texture from the file
            for (int i = 0; i < TextureChannels.Length; i++)
            {
                ChannelBuilder channel = TextureChannels[i];

                if (channel != null)
                {
                    string img_name = channel.Texture.Name;
                    string surfix = channel.Key == KnownChannel.BaseColor ? "_col" : channel.Key == KnownChannel.Normal ? "_nor" : channel.Key == KnownChannel.MetallicRoughness ? "_par" : "_unk";

                    if (surfix == "_col" && usedBaseMap) surfix = "_ref";
                    if (surfix == "_par") HasParameterMap = true;
                    if (surfix == "_nor") HasNormalMap = true;

                    int type = 
                        (channel.Key == KnownChannel.BaseColor && !usedBaseMap) ? 0 : 
                        channel.Key == KnownChannel.Normal ? 1 : 
                        channel.Key == KnownChannel.MetallicRoughness ? 2 : 
                        (channel.Key == KnownChannel.BaseColor && usedBaseMap) ? 3 : 4;
                    // Check if img_name is null, if so generate a new one
                    if (img_name == null || img_name.Length == 0)
                    { 
                        if (channel.Texture.PrimaryImage.Name == null || channel.Texture.PrimaryImage.Name.Length == 0)
                        {
                            if (channel.Texture.PrimaryImage.Content.SourcePath != null)
                            {
                                img_name = Path.GetFileName(channel.Texture.PrimaryImage.Content.SourcePath).Replace("." + channel.Texture.PrimaryImage.Content.FileExtension, surfix);
                            }
                            else
                            {
                                // Use the Hash instead
                                img_name = channel.Texture.PrimaryImage.Content.GetHashCode().ToString() + "_custom" + surfix;
                            }
                        }
                        else
                        {
                            img_name = channel.Texture.PrimaryImage.Name.Replace(".", "_") + surfix;
                        }
                    }

                    channel.Texture.Name = img_name;
                    Texture[Counter] = new Texture(channel, type, surfix == "_ref" ? RefractionMap : EmissivityMap);
                    Counter++;

                    if (surfix == "_col") usedBaseMap = true;
                }
            }
            // Create the texture from Plain Color
            for (int i = 0; i < ColorChannels.Length; i++)
            {
                ChannelBuilder channel = ColorChannels[i];

                if (channel != null)
                {
                    Color color = new Color();

                    foreach (KeyValuePair<KnownProperty, MaterialValue> x in channel.Parameters)
                    {
                        Vector4 RGBA = (Vector4)x.Value;
                        color = Color.FromArgb((int)(RGBA.W * 255), (int)(RGBA.X * 255), (int)(RGBA.Y * 255), (int)(RGBA.Z * 255));
                    }

                    Bitmap bitmap = new Bitmap(50, 50);
                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.Clear(color);
                    Texture[Counter] = new Texture($"{color.GetHashCode() + 999999}_custom_col", 0, bitmap, EmissivityMap);
                    Counter++;
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