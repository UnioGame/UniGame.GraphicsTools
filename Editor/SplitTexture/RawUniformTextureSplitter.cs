namespace UniModules.UniGame.GraphicsTools.Editor.SplitTexture
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Editor;
    using UnityEngine;

    public class RawUniformTextureSplitter : ITextureSplitter
    {
        private const string TexturePartNameTemplate = "{0}_part_{1}";
        
        public IEnumerable<Texture2D> SplitTexture(Texture2D source, Vector2Int maxSize)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            
            if (source.width > maxSize.x && source.height > maxSize.y)
            {
                var counter = 0;
                foreach (var texture2D in SplitVerticalAndHorizontal(source, maxSize))
                {
                    texture2D.name = string.Format(TexturePartNameTemplate, source.name, counter);
                    
                    yield return texture2D;

                    counter++;
                }
            }
            else if (source.width > maxSize.x) 
            {
                var counter = 0;
                foreach (var texture2D in SplitHorizontal(source, maxSize.x))
                {
                    texture2D.name = string.Format(TexturePartNameTemplate, source.name, counter);
                    
                    yield return texture2D;
                    
                    counter++;
                }
            }
            else if (source.height > maxSize.y) 
            {
                var counter = 0;
                foreach (var texture2D in SplitVertical(source, maxSize.y))
                {
                    texture2D.name = string.Format(TexturePartNameTemplate, source.name, counter);
                    
                    yield return texture2D;
                    
                    counter++;
                }
            }
            else
            {
                yield return source;
            }
        }

        private IEnumerable<Texture2D> SplitHorizontal(Texture2D source, int maxWidth)
        {
            var splitCount = Mathf.CeilToInt((float)source.width / maxWidth);
            if (splitCount > 1) 
            {
                var partWidth = Mathf.FloorToInt((float)source.width / splitCount);
                for (var i = 0; i < splitCount; i++) 
                {
                    var rect = new Rect(i * partWidth, 0.0f, partWidth, source.height);
                    
                    if (i == splitCount - 1) 
                    {
                        var commonSplitWidth = partWidth * i;
                        var newWidth = source.width - commonSplitWidth;
                        
                        rect = new Rect(i * partWidth, 0.0f, newWidth, source.height);
                    }

                    yield return source.GetTexture(rect, source.format);
                }
            }
        }

        private IEnumerable<Texture2D> SplitVertical(Texture2D source, int maxHeight)
        {
            var splitCount = Mathf.CeilToInt((float) source.height / maxHeight);
            if (splitCount > 1) 
            {
                var partHeight = Mathf.FloorToInt((float)source.height / splitCount);
                for (var i = 0; i < splitCount; i++) 
                {
                    var rect = new Rect(0.0f, i * partHeight, source.width, partHeight);
                    
                    if (i == splitCount - 1) 
                    {
                        var commonSplitHeight = partHeight * i;
                        var newHeight = source.height - commonSplitHeight;
                        
                        rect = new Rect(0.0f, i * partHeight, source.width, newHeight);
                    }

                    yield return source.GetTexture(rect, source.format);
                }
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
                    if(splittedByHeight != null)
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
    }
}