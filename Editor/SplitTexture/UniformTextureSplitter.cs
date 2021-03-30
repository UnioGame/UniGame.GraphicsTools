namespace UniModules.UniGame.GraphicsTools.Editor.SplitTexture
{
    using System.Collections.Generic;
    using Abstract;
    using UnityEngine;

    public sealed class UniformTextureSplitter : ITextureSplitter
    {
        public IEnumerable<Texture2D> SplitTexture(Texture2D source, Vector2Int maxSize)
        {
            if (source == null) 
            {
                yield break;
            }
            
            if (source.width > maxSize.x && source.height > maxSize.y) 
            {
                var counter = 0;
                foreach (var texture2D in SplitVerticalAndHorizontal(source, maxSize))
                {
                    texture2D.name = SplitHelper.GetSplittedTextureName(counter, texture2D.name);
                    
                    yield return texture2D;
                    
                    counter++;
                }
            }
            else if (source.width > maxSize.x) 
            {
                foreach (var texture2D in SplitHorizontal(source, maxSize.x))
                {
                    yield return texture2D;
                }
            }
            else if (source.height > maxSize.y) 
            {
                foreach (var texture2D in SplitVertical(source, maxSize.y))
                {
                    yield return texture2D;
                }
            }
            else
            {
                yield return source;
            }
        }

        private IEnumerable<Texture2D> SplitHorizontal(Texture2D source, int maxSize)
        {
            var splitCount = SplitHelper.GetSplitCount(source.width, maxSize);
            if (splitCount > 1) 
            {
                var partWidth = Mathf.FloorToInt((float)source.width / splitCount);
                
                for (var i = 0; i < splitCount; i++) 
                {
                    var rect = new Rect(i * partWidth, 0.0f, partWidth, source.height);
                    if (i == splitCount - 1) 
                    {
                        var commonSplitWidth = partWidth * i;
                        partWidth = source.width - commonSplitWidth;
                        rect = new Rect(commonSplitWidth, 0.0f, partWidth, source.height);
                    }
                    
                    var texture = GetTextureByRect(source, rect);
                    texture.name = SplitHelper.GetSplittedTextureName(i, source.name);
                    yield return texture;
                }
            }
            else
            {
                yield return source;
            }
        }

        private IEnumerable<Texture2D> SplitVertical(Texture2D source, int maxSize)
        {
            var splitCount = SplitHelper.GetSplitCount(source.height, maxSize);
            if (splitCount > 1) {
                var partHeight = Mathf.FloorToInt((float)source.height / splitCount);
                
                for (var i = 0; i < splitCount; i++) {
                    if (i == splitCount - 1) {
                        var commonSplitHeight = partHeight * i;
                        partHeight = source.height - commonSplitHeight;
                    }

                    var rect = new Rect(0.0f, i * partHeight, source.width, partHeight);
                    var texture = GetTextureByRect(source, rect);
                    texture.name = SplitHelper.GetSplittedTextureName(i, source.name);
                    yield return texture;
                }
            }
            else
            {
                yield return source;
            }
        }

        private IEnumerable<Texture2D> SplitVerticalAndHorizontal(Texture2D source, Vector2Int maxSize)
        {
            var splittedByWidth = SplitHorizontal(source, maxSize.x);
            if (splittedByWidth != null)
            {
                foreach (var texture in splittedByWidth)
                {
                    var splittedByHeight = SplitVertical(texture, maxSize.y);
                    if (splittedByHeight != null)
                    {
                        foreach (var texture2D in splittedByHeight)
                        {
                            yield return texture2D;
                        }
                    }
                    else
                    {
                        yield return texture;
                    }
                }
            }
        }

        private Texture2D GetTextureByRect(Texture2D source, Rect rect)
        {
            var resultTexture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            var pixels = source.GetPixels((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
            
            resultTexture.SetPixels(pixels);
            resultTexture.Apply();

            return resultTexture;
        }
    }
}