namespace UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas
{
    using System;
    using Abstract;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class SpriteAtlasSettings : ISpriteAtlasSettings
    {
        private const string AtlasSettingsGroup = "Sprite Atlas Settings";
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private SpriteAtlasType _type = SpriteAtlasType.Master;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _includeInBuild;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup), Sirenix.OdinInspector.Title("Packing")]
#endif
        private bool _allowRotation;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _tightPacking;

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup), Sirenix.OdinInspector.ValueDropdown(nameof(PaddingValues))]
#endif
        private int _padding = PaddingValues[1];

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Title("Texture")]
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _readWriteEnabled = true;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _generateMipMaps;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _sRGB = true;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private FilterMode _filterMode = FilterMode.Bilinear;

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Title("Default Build Platform")]
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup), Sirenix.OdinInspector.ValueDropdown(nameof(MaxSizeValues))]
#endif
        private int _maxTextureSize = MaxSizeValues[6];

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private AtlasTextureFormat _format = AtlasTextureFormat.Automatic;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private TextureImporterCompression _compression = TextureImporterCompression.Compressed;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _useCrunchCompression = true;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(AtlasSettingsGroup)]
#endif
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