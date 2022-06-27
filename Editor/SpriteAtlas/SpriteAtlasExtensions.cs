using UniModules.Editor;

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
            var textureSettings = settings.ImportSettings.GetTextureImporterSettings();
            
            var atlasTextureSettings = new SpriteAtlasTextureSettings
            {
                generateMipMaps = textureSettings.mipmapEnabled,
                filterMode = textureSettings.filterMode,
                readable = textureSettings.readable,
                sRGB = textureSettings.sRGBTexture
            };

            var atlasPackingSettings = new SpriteAtlasPackingSettings
            {
                padding = settings.Padding,
                enableRotation = settings.AllowRotation,
                enableTightPacking = settings.TightPacking
            };

            var platformSettings = settings.ImportSettings.GetTextureImporterPlatformSettings(EditorUserBuildSettings.selectedBuildTargetGroup);

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
            atlas.SetDirty();
            AssetDatabase.SaveAssets();
        }

        public static void RemoveSprites(this SpriteAtlas atlas, string[] spritePaths)
        {
            if (spritePaths.Length == 0)
            {
                return;
            }
            var sprites = spritePaths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>);
            var packedAsset = sprites.ToArray();
            atlas.Remove(packedAsset);
            atlas.SetDirty();
            AssetDatabase.SaveAssets();
        }

        public static bool CheckSettings(this SpriteAtlas atlas, SpriteAtlasSettings settings)
        {
            var atlasTextureSettings     = atlas.GetTextureSettings();
            var packingSettings     = atlas.GetPackingSettings();
            var isVariant           = atlas.isVariant;
            
            var textureSettings = settings.ImportSettings.GetTextureImporterSettings();
            var sameSettings =
                atlasTextureSettings.generateMipMaps         == textureSettings.mipmapEnabled &&
                atlasTextureSettings.filterMode              == textureSettings.filterMode &&
                atlasTextureSettings.readable                == textureSettings.readable &&
                atlasTextureSettings.sRGB                    == textureSettings.sRGBTexture &&
                packingSettings.padding                 == settings.Padding &&
                packingSettings.enableRotation          == settings.AllowRotation &&
                packingSettings.enableTightPacking      == settings.TightPacking &&
                isVariant                               == (settings.Type == SpriteAtlasType.Variant);

            return sameSettings && CheckPlatformSettings(atlas, settings, BuildTargetGroup.Unknown) && 
                   CheckPlatformSettings(atlas, settings, BuildTargetGroup.Standalone) && 
                   CheckPlatformSettings(atlas, settings, BuildTargetGroup.Android);
        }

        private static bool CheckPlatformSettings(SpriteAtlas atlas, SpriteAtlasSettings settings, BuildTargetGroup buildTargetGroup)
        {
            var atlasPlatformSettings = atlas.GetPlatformSettings(buildTargetGroup.ToString());
            var platformSettings = settings.ImportSettings.GetTextureImporterPlatformSettings(buildTargetGroup);

            return atlasPlatformSettings.textureCompression == platformSettings.textureCompression &&
                   atlasPlatformSettings.maxTextureSize == platformSettings.maxTextureSize &&
                   atlasPlatformSettings.format == platformSettings.format &&
                   atlasPlatformSettings.crunchedCompression == platformSettings.crunchedCompression &&
                   atlasPlatformSettings.compressionQuality == platformSettings.compressionQuality;
        }
    }
}