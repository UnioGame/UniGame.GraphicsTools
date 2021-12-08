namespace UniModules.UniGame.CoreModules.UniGame.GraphicsTools.Editor
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class TextureImporterSettingsAdapter
    {
        private const string StandaloneCompressionQualityCondition = "@this.FormatStandalone == TextureImporterFormat.BC7 || this.FormatStandalone == TextureImporterFormat.BC6H"; 
        private const string StandaloneCrunchCompressionQualityCondition = "@this.FormatStandalone == TextureImporterFormat.DXT5Crunched || this.FormatStandalone == TextureImporterFormat.DXT1Crunched";

        public enum SettingsApplicationMode
        {
            AlwaysOverwrite,
            OnlyApplyOnCreation
        };

        private static readonly int[] MaxSizeValues = {
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

        [Tooltip("Determines whether importer settings of an existing sprite will be overwritten if the same sprite is unpacked from PSB")]
        public SettingsApplicationMode ApplicationMode = SettingsApplicationMode.AlwaysOverwrite;

        [ReadOnly]
        public TextureImporterType TextureType = TextureImporterType.Sprite;

        public SpriteImportMode SpriteMode = SpriteImportMode.Single;
        [Indent]
        public int SpritePixelsPerUnit = 200;
        [Indent]
        public SpriteMeshType SpriteMeshType = SpriteMeshType.Tight;
        [Range(0, 32)]
        [Indent]
        public uint SpriteExtrude = 1;
        [Indent]
        public SpriteAlignment SpritePivot = SpriteAlignment.Center;
        [Indent]
        [HideLabel]
        [ShowIf("SpritePivot", SpriteAlignment.Custom)]
        public Vector2 CustomPivot = new Vector2(0.5f, 0.5f);
        [Indent]
        public bool SpriteGeneratePhysicsShape = true;

        [TitleGroup("Advanced", horizontalLine: false, boldTitle: false)]
        [Indent]
        [LabelText("sRGB (Color Texture)")]
        public bool sRGBTexture = true;
        [TitleGroup("Advanced")]
        [Indent]
        public TextureImporterAlphaSource AlphaSource = TextureImporterAlphaSource.FromInput;
        [TitleGroup("Advanced")]
        [Indent]
        public bool AlphaIsTransparency = true;
        [TitleGroup("Advanced")]
        [Indent]
        [LabelText("Read/Write Enabled")]
        public bool Readable = true;
        [TitleGroup("Advanced")]
        [Indent]
        [LabelText("Generate Mip Maps")]
        public bool mipmapEnabled = false;

        public TextureWrapMode WrapMode = TextureWrapMode.Clamp;
        public FilterMode FilterMode = FilterMode.Bilinear;
        [Range(1, 9)]
        public int Aniso = 2;

        public bool CreateLocatorWrappers;

        [TabGroup("Default")]
        [ValueDropdown("MaxSizeValues")]
        [LabelText("Max Size")]
        public int MaxTextureSize = MaxSizeValues[6];
        [TabGroup("Default")]
        public TextureResizeAlgorithm ResizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        [TabGroup("Default")]
        public TextureImporterFormat Format = TextureImporterFormat.Automatic;
        [TabGroup("Default")]
        [LabelText("Compression")]
        public TextureImporterCompression TextureCompression = TextureImporterCompression.Compressed;
        [TabGroup("Default")]
        [LabelText("Use Crunch Compression")]
        public bool CrunchedCompression = true;
        [TabGroup("Default")]
        [LabelText("Compressior Quality")]
        [Range(0, 100), ShowIf("CrunchedCompression")]
        public int CompressionQuality = 90;

        [TabGroup("PC, Mac & Linux")]
        [LabelText("Override for PC, Mac & Linux Standalone")]
        [ToggleLeft]
        public bool OverrideStandalone = false;
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [ValueDropdown("MaxSizeValues")]
        [LabelText("Max Size")]
        public int MaxTextureSizeStandalone = MaxSizeValues[6];
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [LabelText("Resize Algorithm")]
        public TextureResizeAlgorithm ResizeAlgorithmStandalone = TextureResizeAlgorithm.Mitchell;
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [LabelText("Format")]
        public TextureImporterFormat FormatStandalone = TextureImporterFormat.DXT5Crunched;
        [TabGroup("PC, Mac & Linux")]
        [LabelText("Compressior Quality")]
        [ShowIf(StandaloneCompressionQualityCondition)]
        public TextureImporterCompression TextureCompressionStandalone = TextureImporterCompression.Compressed;
        [TabGroup("PC, Mac & Linux")]
        [EnableIf("OverrideStandalone")]
        [LabelText("Compressior Quality")]
        [Range(0, 100), ShowIf(StandaloneCrunchCompressionQualityCondition)]
        public int CompressionQualityStandalone = 90;

        [TabGroup("Android")]
        [LabelText("Override for Android")]
        [ToggleLeft]
        public bool OverrideAndroid = false;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        [ValueDropdown("MaxSizeValues")]
        [LabelText("Max Size")]
        public int MaxTextureSizeAndroid = MaxSizeValues[6];
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        public TextureResizeAlgorithm ResizeAlgorithmAndroid = TextureResizeAlgorithm.Mitchell;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        public TextureImporterFormat FormatAndroid = TextureImporterFormat.ETC2_RGBA8Crunched;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        [LabelText("Compressior Quality")]
        public TextureImporterCompression TextureCompressionAndroid = TextureImporterCompression.Compressed;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        [LabelText("Compressior Quality")]
        [Range(0, 100)]
        public int CompressionQualityAndroid = 90;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        public bool SplitAlphaChannel = false;
        [TabGroup("Android")]
        [EnableIf("OverrideAndroid")]
        [LabelText("Override ETC2 fallback")]
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
        public TextureImporterPlatformSettings GetTextureImporterPlatformSettings(BuildTargetGroup target = BuildTargetGroup.Unknown)
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