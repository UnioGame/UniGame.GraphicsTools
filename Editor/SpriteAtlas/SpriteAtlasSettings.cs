namespace UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas
{
    using System;
    using Abstract;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class SpriteAtlasSettings : ISpriteAtlasSettings
    {
        private const string AtlasSettingsGroup = "Sprite Atlas Settings";
        
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private SpriteAtlasType _type = SpriteAtlasType.Master;
        
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _includeInBuild;
        
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup), Title("Packing")]
        private bool _allowRotation;
        
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _tightPacking;

        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup), ValueDropdown(nameof(PaddingValues))]
        private int _padding = PaddingValues[1];

        [SerializeField]
        [Title("Texture")]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _readWriteEnabled = true;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _generateMipMaps;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _sRGB = true;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private FilterMode _filterMode = FilterMode.Bilinear;

        [SerializeField]
        [Title("Default Build Platform")]
        [FoldoutGroup(AtlasSettingsGroup), ValueDropdown(nameof(MaxSizeValues))]
        private int _maxTextureSize = MaxSizeValues[6];

        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private AtlasTextureFormat _format = AtlasTextureFormat.Automatic;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private TextureImporterCompression _compression = TextureImporterCompression.Compressed;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        private bool _useCrunchCompression = true;
        [SerializeField]
        [FoldoutGroup(AtlasSettingsGroup)]
        [Range(0,100)]
        private int _compressionQuality = 100;
        
        private static readonly int[] PaddingValues = {
            2,
            4,
            8
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

        public SpriteAtlasType Type => _type;

        public bool IncludeInBuild => _includeInBuild;
        public bool AllowRotation => _allowRotation;
        public bool TightPacking => _tightPacking;

        public int Padding => _padding;

        public bool ReadWriteEnabled => _readWriteEnabled;
        public bool GenerateMipMaps => _generateMipMaps;
        public bool SRgb => _sRGB;

        public FilterMode FilterMode => _filterMode;

        public int MaxTextureSize => _maxTextureSize;

        public AtlasTextureFormat Format => _format;
        public TextureImporterCompression Compression => _compression;

        public bool UseCrunchCompression => _useCrunchCompression;

        public int CompressionQuality => _compressionQuality;
    }
}