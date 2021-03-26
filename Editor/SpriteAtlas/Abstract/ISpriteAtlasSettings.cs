namespace UniModules.UniGame.GraphicsTools.Editor.SpriteAtlas.Abstract
{
    using UnityEditor;
    using UnityEngine;

    public interface ISpriteAtlasSettings
    {
        SpriteAtlasType Type { get; }

        bool IncludeInBuild { get; }
        bool AllowRotation  { get; }
        bool TightPacking   { get; }

        int Padding { get; }

        bool ReadWriteEnabled { get; }
        bool GenerateMipMaps  { get; }
        bool SRgb             { get; }

        FilterMode FilterMode { get; }

        int MaxTextureSize { get; }

        AtlasTextureFormat         Format      { get; }
        TextureImporterCompression Compression { get; }

        bool UseCrunchCompression { get; }
        
        int CompressionQuality { get; }
    }
}