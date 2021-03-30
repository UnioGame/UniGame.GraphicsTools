namespace UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas
{
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine;
    using UnityEngine.U2D;

    public static class SpriteAtlasExtensions
    {
        public static void ApplySettings(this SpriteAtlas atlas, SpriteAtlasSettings settings)
        {
            var atlasTextureSettings = new SpriteAtlasTextureSettings
            {
                generateMipMaps = settings.GenerateMipMaps,
                filterMode = settings.FilterMode,
                readable = settings.ReadWriteEnabled,
                sRGB = settings.SRgb
            };

            var atlasPackingSettings = new SpriteAtlasPackingSettings
            {
                padding = settings.Padding,
                enableRotation = settings.AllowRotation,
                enableTightPacking = settings.TightPacking
            };

            var platformSettings = new TextureImporterPlatformSettings
            {
                textureCompression = settings.Compression,
                maxTextureSize = settings.MaxTextureSize,
                format = (TextureImporterFormat)settings.Format,
                crunchedCompression = settings.UseCrunchCompression,
                compressionQuality = settings.CompressionQuality
            };

            atlas.SetTextureSettings(atlasTextureSettings);
            atlas.SetPackingSettings(atlasPackingSettings);
            atlas.SetPlatformSettings(platformSettings);

            atlas.SetIncludeInBuild(settings.IncludeInBuild);
            atlas.SetIsVariant(settings.Type == SpriteAtlasType.Variant);
        }

        public static void RemoveAt(this SpriteAtlas atlas, int index)
        {
            var methodInfo = typeof(UnityEditor.U2D.SpriteAtlasExtensions).GetMethod("RemoveAt", BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo?.Invoke(null, new object[] {atlas, index});
        }

        public static void Clear(this SpriteAtlas atlas)
        {
            var packedObjects = atlas.GetPackables();
            var removedCount  = 0;
            for (var i = 0; i < packedObjects.Length; i++)
            {
                atlas.RemoveAt(i - removedCount);
                removedCount++;
            }
        }

        public static void RemoveSprite(this SpriteAtlas atlas, string spritePath)
        {
            var packedAsset = new[] { AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath) };
            atlas.Remove(packedAsset);
            AssetDatabase.SaveAssets();
        }

        public static void RemoveSprites(this SpriteAtlas atlas, string[] spritePaths)
        {
            if (spritePaths.Length == 0)
            {
                return;
            }
            var sprites = spritePaths.Select(spritePath => AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath));
            var packedAsset = sprites.ToArray();
            atlas.Remove(packedAsset);
            AssetDatabase.SaveAssets();
        }

        public static bool CheckSettings(this SpriteAtlas atlas, SpriteAtlasSettings settings)
        {
            var textureSettings     = atlas.GetTextureSettings();
            var packingSettings     = atlas.GetPackingSettings();
            var platformSettings    = atlas.GetPlatformSettings("DefaultTexturePlatform");
            var isVariant           = atlas.isVariant;

            var sameSettings =
                textureSettings.generateMipMaps         == settings.GenerateMipMaps &&
                textureSettings.filterMode              == settings.FilterMode &&
                textureSettings.readable                == settings.ReadWriteEnabled &&
                textureSettings.sRGB                    == settings.SRgb &&
                packingSettings.padding                 == settings.Padding &&
                packingSettings.enableRotation          == settings.AllowRotation &&
                packingSettings.enableTightPacking      == settings.TightPacking &&
                platformSettings.textureCompression     == settings.Compression &&
                platformSettings.maxTextureSize         == settings.MaxTextureSize &&
                platformSettings.format                 == (TextureImporterFormat)settings.Format &&
                platformSettings.crunchedCompression    == settings.UseCrunchCompression &&
                platformSettings.compressionQuality     == settings.CompressionQuality &&
                isVariant                               == (settings.Type == SpriteAtlasType.Variant);

            return sameSettings;
        }
    }
}