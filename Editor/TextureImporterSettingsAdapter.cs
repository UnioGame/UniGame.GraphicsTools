namespace UniModules.UniGame.CoreModules.UniGame.GraphicsTools.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [Serializable]
    public class TextureImporterSettingsAdapter
    {
        private const string StandaloneCompressionQualityCondition =
            "@this.FormatStandalone == TextureImporterFormat.BC7 || this.FormatStandalone == TextureImporterFormat.BC6H";

        private const string StandaloneCrunchCompressionQualityCondition =
            "@this.FormatStandalone == TextureImporterFormat.DXT5Crunched || this.FormatStandalone == TextureImporterFormat.DXT1Crunched";

        public enum SettingsApplicationMode
        {
            AlwaysOverwrite,
            OnlyApplyOnCreation
        };

        private static readonly int[] MaxSizeValues =
        {
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192
        };

        [Tooltip(
            "Determines whether importer settings of an existing sprite will be overwritten if the same sprite is unpacked from PSB")]
        public SettingsApplicationMode ApplicationMode = SettingsApplicationMode.AlwaysOverwrite;

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        public TextureImporterType TextureType = TextureImporterType.Sprite;

        public SpriteImportMode SpriteMode = SpriteImportMode.Single;
#if ODIN_INSPECTOR
        [Indent]
#endif
        public int SpritePixelsPerUnit = 200;

#if ODIN_INSPECTOR
        [Indent]
#endif
        public SpriteMeshType SpriteMeshType = SpriteMeshType.Tight;

        [Range(0, 32)]
#if ODIN_INSPECTOR
        [Indent]
#endif
        public uint SpriteExtrude = 1;
#if ODIN_INSPECTOR
        [Indent]
#endif
        public SpriteAlignment SpritePivot = SpriteAlignment.Center;

#if ODIN_INSPECTOR
        [Indent] [HideLabel] [ShowIf("SpritePivot", SpriteAlignment.Custom)]
#endif
        public Vector2 CustomPivot = new Vector2(0.5f, 0.5f);

#if ODIN_INSPECTOR
        [Indent]
#endif
        public bool SpriteGeneratePhysicsShape = true;

#if ODIN_INSPECTOR
        [TitleGroup("Advanced", horizontalLine: false, boldTitle: false)] [Indent] [LabelText("sRGB (Color Texture)")]
#endif
        public bool sRGBTexture = true;

#if ODIN_INSPECTOR
        [TitleGroup("Advanced")] [Indent]
#endif
        public TextureImporterAlphaSource AlphaSource = TextureImporterAlphaSource.FromInput;

#if ODIN_INSPECTOR
        [TitleGroup("Advanced")] [Indent]
#endif
        public bool AlphaIsTransparency = true;

#if ODIN_INSPECTOR
        [TitleGroup("Advanced")] [Indent] [LabelText("Read/Write Enabled")]
#endif
        public bool Readable = true;

#if ODIN_INSPECTOR
        [TitleGroup("Advanced")] [Indent] [LabelText("Generate Mip Maps")]
#endif
        public bool mipmapEnabled = false;

        public TextureWrapMode WrapMode = TextureWrapMode.Clamp;
        public FilterMode FilterMode = FilterMode.Bilinear;

#if ODIN_INSPECTOR
        [Range(1, 9)]
#endif
        public int Aniso = 2;

        public bool CreateLocatorWrappers;

#if ODIN_INSPECTOR
        [TabGroup("Default")] [ValueDropdown("MaxSizeValues")] [LabelText("Max Size")]
#endif
        public int MaxTextureSize = MaxSizeValues[6];

#if ODIN_INSPECTOR
        [TabGroup("Default")]
#endif
        public TextureResizeAlgorithm ResizeAlgorithm = TextureResizeAlgorithm.Mitchell;

#if ODIN_INSPECTOR
        [TabGroup("Default")]
#endif
        public TextureImporterFormat Format = TextureImporterFormat.Automatic;

#if ODIN_INSPECTOR
        [TabGroup("Default")] [LabelText("Compression")]
#endif
        public TextureImporterCompression TextureCompression = TextureImporterCompression.Compressed;

#if ODIN_INSPECTOR
        [TabGroup("Default")] [LabelText("Use Crunch Compression")]
#endif
        public bool CrunchedCompression = true;

#if ODIN_INSPECTOR
        [TabGroup("Default")] [LabelText("Compressior Quality")] [Range(0, 100), ShowIf("CrunchedCompression")]
#endif
        public int CompressionQuality = 90;

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")] [LabelText("Override for PC, Mac & Linux Standalone")] [ToggleLeft]
#endif
        public bool OverrideStandalone = false;

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [ValueDropdown("MaxSizeValues")]
        [LabelText("Max Size")]
#endif
        public int MaxTextureSizeStandalone = MaxSizeValues[6];

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")] [EnableIf("OverrideStandalone")] [LabelText("Resize Algorithm")]
#endif
        public TextureResizeAlgorithm ResizeAlgorithmStandalone = TextureResizeAlgorithm.Mitchell;

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")] [EnableIf("OverrideStandalone")] [LabelText("Format")]
#endif
        public TextureImporterFormat FormatStandalone = TextureImporterFormat.DXT5Crunched;

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")] [LabelText("Compressior Quality")] [ShowIf(StandaloneCompressionQualityCondition)]
#endif
        public TextureImporterCompression TextureCompressionStandalone = TextureImporterCompression.Compressed;

#if ODIN_INSPECTOR
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [LabelText("Compressior Quality")]
        [Range(0, 100), ShowIf(StandaloneCrunchCompressionQualityCondition)]
#endif
        public int CompressionQualityStandalone = 90;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [LabelText("Override for Android")] [ToggleLeft]
#endif
        public bool OverrideAndroid = false;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")] [ValueDropdown("MaxSizeValues")] [LabelText("Max Size")]
#endif
        public int MaxTextureSizeAndroid = MaxSizeValues[6];

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")]
#endif
        public TextureResizeAlgorithm ResizeAlgorithmAndroid = TextureResizeAlgorithm.Mitchell;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")]
#endif
        public TextureImporterFormat FormatAndroid = TextureImporterFormat.ETC2_RGBA8Crunched;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")] [LabelText("Compressior Quality")]
#endif
        public TextureImporterCompression TextureCompressionAndroid = TextureImporterCompression.Compressed;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")] [LabelText("Compressior Quality")] [Range(0, 100)]
#endif
        public int CompressionQualityAndroid = 90;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")]
#endif
        public bool SplitAlphaChannel = false;

#if ODIN_INSPECTOR
        [TabGroup("Android")] [EnableIf("OverrideAndroid")] [LabelText("Override ETC2 fallback")]
#endif
        public AndroidETC2FallbackOverride androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;

        public TextureImporterSettings GetTextureImporterSettings()
        {
            var settings = new TextureImporterSettings
            {
                textureType = TextureType,

                spriteMode = (int)SpriteMode,
                spritePixelsPerUnit = SpritePixelsPerUnit,
                spriteMeshType = SpriteMeshType,
                spriteExtrude = SpriteExtrude,
                spriteAlignment = (int)SpritePivot,
                spritePivot = CustomPivot,
                spriteGenerateFallbackPhysicsShape = SpriteGeneratePhysicsShape,

                sRGBTexture = sRGBTexture,
                alphaSource = AlphaSource,
                alphaIsTransparency = AlphaIsTransparency,
                readable = Readable,
                mipmapEnabled = mipmapEnabled,

                wrapMode = WrapMode,
                filterMode = FilterMode,
                aniso = Aniso
            };

            return settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">Unknown is Default Texture Platform for now.</param>
        /// <returns></returns>
        public TextureImporterPlatformSettings GetTextureImporterPlatformSettings(
            BuildTargetGroup target = BuildTargetGroup.Unknown)
        {
            TextureImporterPlatformSettings settings = null;

            switch (target)
            {
                case BuildTargetGroup.Unknown:
                    settings = new TextureImporterPlatformSettings
                    {
                        name = "DefaultTexturePlatform",
                        maxTextureSize = MaxTextureSize,
                        resizeAlgorithm = ResizeAlgorithm,
                        format = Format,
                        textureCompression = TextureCompression,
                        crunchedCompression = CrunchedCompression,
                        compressionQuality = CompressionQuality
                    };
                    break;
                case BuildTargetGroup.Standalone:
                    settings = new TextureImporterPlatformSettings
                    {
                        name = target.ToString(),
                        overridden = OverrideStandalone,
                        maxTextureSize = MaxTextureSizeStandalone,
                        resizeAlgorithm = ResizeAlgorithmStandalone,
                        format = FormatStandalone,
                        textureCompression = TextureCompressionStandalone,
                        compressionQuality = CompressionQualityStandalone
                    };
                    break;
                case BuildTargetGroup.Android:
                    settings = new TextureImporterPlatformSettings
                    {
                        name = target.ToString(),
                        overridden = OverrideAndroid,
                        maxTextureSize = MaxTextureSizeAndroid,
                        resizeAlgorithm = ResizeAlgorithmAndroid,
                        format = FormatAndroid,
                        textureCompression = TextureCompressionAndroid,
                        compressionQuality = CompressionQualityAndroid,
                        allowsAlphaSplitting = SplitAlphaChannel,
                        androidETC2FallbackOverride = androidETC2FallbackOverride
                    };
                    break;
            }

            return settings;
        }
    }
}