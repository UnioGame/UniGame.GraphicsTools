namespace UniModules.UniGame.GraphicsTools.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    public static class TextureExtensions
    {
        private static readonly Dictionary<byte[], Func<BinaryReader, Vector2Int>> ImageFormatDecoders = new Dictionary<byte[], Func<BinaryReader, Vector2Int>>
        {
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, DecodePNG }
        };
        
        public static int ReadLittleEndianInt32(this BinaryReader binaryReader)
        {
            var bytes = new byte[sizeof(int)];
            for (var i = 0; i < sizeof(int); i++)
            {
                bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
            }

            return BitConverter.ToInt32(bytes, 0);
        }

        public static Vector2Int GetTextureSize(this byte[] source)
        {
            using (var binaryReader = new BinaryReader(new MemoryStream(source)))
            {
                var maxMagicBytesLength = ImageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;
                var magicBytes          = new byte[maxMagicBytesLength];

                for (var i = 0; i < maxMagicBytesLength; i++)
                {
                    magicBytes[i] = binaryReader.ReadByte();

                    foreach (var imageFormatDecoder in ImageFormatDecoders)
                    {
                        if (magicBytes.StartsWith(imageFormatDecoder.Key))
                        {
                            return imageFormatDecoder.Value.Invoke(binaryReader);
                        }
                    }
                }
            }
            
            return default;
        }

        public static bool StartsWith(this byte[] source, byte[] target)
        {
            for (var i = 0; i < target.Length; i++)
            {
                if (target[i] != source[i])
                    return false;
            }

            return true;
        }

        private static Vector2Int DecodePNG(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            
            var width  = binaryReader.ReadLittleEndianInt32();
            var height = binaryReader.ReadLittleEndianInt32();
            
            return new Vector2Int(width, height);
        }

        public static Texture2D GetTexture(this Texture2D source, Rect rect, TextureFormat format)
        {
            var texture = new Texture2D(Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height), format, false);
            var rawData = source.GetRawTextureData(rect);

            texture.LoadRawTextureData(rawData);
            texture.Apply();

            return texture;
        }

        public static byte[] GetRawTextureData(this Texture2D texture, Rect rect)
        {
            return GetByteArrayByRect(Mathf.FloorToInt(rect.x), Mathf.FloorToInt(rect.y), Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height), 
                texture.width, texture.GetRawTextureData(), texture.format);
        }

        public static byte[] GetByteArrayByRect(int x, int y, int width, int height, int sourceWidth, byte[] sourceArray, TextureFormat sourceFormat)
        {
            var sizeInBytes = sourceFormat.GetSizeOf();

            var offset= (x + y * sourceWidth) * sizeInBytes;

            var destinationArray = new byte[width * height * sizeInBytes];
            var length           = width * sizeInBytes;
            var sourceLength     = sourceWidth * sizeInBytes;
            
            for (var i = 0; i < height; i++)
            {
                var startIndex = offset + sourceLength * i;
                var endIndex   = length * i;

                Array.Copy(sourceArray, startIndex, destinationArray, endIndex, length);
            }

            return destinationArray;
        }

        public static int GetSizeOf(this TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.Alpha8:
                    return 1;
                case TextureFormat.ARGB4444:
                    return 2;
                case TextureFormat.RGB24:
                    return 3;
                case TextureFormat.RGBA32:
                    return 4;
                case TextureFormat.ARGB32:
                    return 4;
                case TextureFormat.RGB565:
                    return 2;
                case TextureFormat.R16:
                    return 2;
                case TextureFormat.DXT1:
                    return 1;
                case TextureFormat.DXT5:
                    return 1;
                case TextureFormat.RGBA4444:
                    return 2;
                case TextureFormat.BGRA32:
                    return 4;
                case TextureFormat.RHalf:
                    return 2;
                case TextureFormat.RGHalf:
                    return 4;
                case TextureFormat.RGBAHalf:
                    return 8;
                case TextureFormat.RFloat:
                    return 4;
                case TextureFormat.RGFloat:
                    return 8;
                case TextureFormat.RGBAFloat:
                    return 16;
                case TextureFormat.RGB9e5Float:
                    return 4;
                case TextureFormat.BC6H:
                    return 8;
                case TextureFormat.BC7:
                    return 1;
                case TextureFormat.BC4:
                    return 1;
                case TextureFormat.BC5:
                    return 1;
                case TextureFormat.DXT1Crunched:
                    return 1;
                case TextureFormat.DXT5Crunched:
                    return 1;
                case TextureFormat.PVRTC_RGB2:
                    return 1;
                case TextureFormat.PVRTC_RGBA2:
                    return 1;
                case TextureFormat.PVRTC_RGB4:
                    return 1;
                case TextureFormat.PVRTC_RGBA4:
                    return 1;
                case TextureFormat.ETC_RGB4:
                    return 1;
                case TextureFormat.EAC_R:
                    return 1;
                case TextureFormat.EAC_R_SIGNED:
                    return 1;
                case TextureFormat.EAC_RG:
                    return 1;
                case TextureFormat.EAC_RG_SIGNED:
                    return 1;
                case TextureFormat.ETC2_RGB:
                    return 1;
                case TextureFormat.ETC2_RGBA1:
                    return 1;
                case TextureFormat.ETC2_RGBA8:
                    return 1;
                case TextureFormat.ASTC_4x4:
                    return 16;
                case TextureFormat.ASTC_5x5:
                    return 16;
                case TextureFormat.ASTC_6x6:
                    return 16;
                case TextureFormat.ASTC_8x8:
                    return 16;
                case TextureFormat.ASTC_10x10:
                    return 16;
                case TextureFormat.ASTC_12x12:
                    return 16;
                case TextureFormat.RG16:
                    return 2;
                case TextureFormat.R8:
                    return 1;
                case TextureFormat.ETC_RGB4Crunched:
                    return 1;
                case TextureFormat.ETC2_RGBA8Crunched:
                    return 1;
                case TextureFormat.ASTC_HDR_4x4:
                    return 16;
                case TextureFormat.ASTC_HDR_5x5:
                    return 16;
                case TextureFormat.ASTC_HDR_6x6:
                    return 16;
                case TextureFormat.ASTC_HDR_8x8:
                    return 16;
                case TextureFormat.ASTC_HDR_10x10:
                    return 16;
                case TextureFormat.ASTC_HDR_12x12:
                    return 16;
                default:
                    return 4;
            }
        }
    }
}