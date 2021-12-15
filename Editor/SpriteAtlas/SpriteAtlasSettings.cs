namespace UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas
{
    using System;
    using CoreModules.UniGame.GraphicsTools.Editor;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEngine;

    [Serializable]
    public class SpriteAtlasSettings
    {
        private const string AtlasSettingsGroup = "Sprite Atlas Settings";
        
        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup)]
#endif
        private SpriteAtlasType _type = SpriteAtlasType.Master;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _includeInBuild;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup), Title("Packing")]
#endif
        private bool _allowRotation;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup)]
#endif
        private bool _tightPacking;

        [SerializeField]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup), ValueDropdown(nameof(PaddingValues))]
#endif
        private int _padding = PaddingValues[1];

        
        [SerializeField]
        [Title("Default Importer Settings"), HideLabel]
#if ODIN_INSPECTOR
        [FoldoutGroup(AtlasSettingsGroup)]
#endif
        public TextureImporterSettingsAdapter ImportSettings = new TextureImporterSettingsAdapter();
        
        private static readonly int[] PaddingValues = {
            2,
            4,
            8
        };
        
        public SpriteAtlasType Type => _type;

        public bool IncludeInBuild => _includeInBuild;
        public bool AllowRotation => _allowRotation;
        public bool TightPacking => _tightPacking;

        public int Padding => _padding;
    }
}