namespace UniModules.UniGame.GraphicsTools.Editor.SplitTexture.Abstract
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface ITextureSplitter
    {
        IEnumerable<Texture2D> SplitTexture(Texture2D source, Vector2Int maxSize);
    }
}