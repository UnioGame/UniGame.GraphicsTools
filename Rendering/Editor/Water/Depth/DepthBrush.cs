namespace UniGame.Rendering.Editor.Water.Depth
{
    using Abstract;
    using UnityEditor;
    using UnityEngine;

    public class DepthBrush : Brush
    {
        public override GUIContent GetContent()
        {
            return EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat");
        }

        public override void Paint(Rect sourceRect, Rect paintRect, PaintMode paintMode, Color paintColor, Color[] sourceBuffer, ref Color[] brushBuffer, ref Color[] textureBuffer)
        {
            var brushCenter = new Vector2Int((int) (paintRect.x + paintRect.size.x * 0.5f), (int) (paintRect.y + paintRect.size.y * 0.5f));
            var radius      = (int) (paintRect.width * 0.5f);

            var cx = brushCenter.x;
            var cy = brushCenter.y;
            
            for (var x = 0; x <= radius; x++)
            {
                var delta = (int)Mathf.Ceil(Mathf.Sqrt(radius * radius - x * x));
                for (var y = 0; y < delta; y++)
                {
                    var px = cx + x;
                    var nx = cx - x;
                    var py = cy + y;
                    var ny = cy - y;

                    var point1Index = py * (int)sourceRect.width + px;
                    var point2Index = py * (int)sourceRect.width + nx;
                    var point3Index = ny * (int)sourceRect.width + px;
                    var point4Index = ny * (int)sourceRect.width + nx;
                    
                    // рисуем сразу в 4-х четвертях круга одновременно
                    // но индексы могут совпадать или выходить за rect текстуры
                    if (point1Index < textureBuffer.Length && point1Index >= 0 && point1Index != point2Index && point1Index != point3Index && point1Index != point4Index)
                    {
                        var brushColor = GetBrushColor(px, py, radius, paintColor, brushCenter, brushBuffer[point1Index]);
                        brushBuffer[point1Index] = brushColor;

                        textureBuffer[point1Index] = ProcessBrushColor(paintMode, brushColor, paintColor, sourceBuffer, point1Index);
                    }
                    if(point2Index < textureBuffer.Length && point2Index >= 0 && point2Index != point3Index && point2Index != point4Index)
                    {
                        var brushColor = GetBrushColor(nx, py, radius, paintColor, brushCenter, brushBuffer[point2Index]);
                        brushBuffer[point2Index] = brushColor;
                        
                        textureBuffer[point2Index] = ProcessBrushColor(paintMode, brushColor, paintColor, sourceBuffer, point2Index);
                    }
                    if(point3Index < textureBuffer.Length && point3Index >= 0 && point3Index != point4Index)
                    {
                        var brushColor = GetBrushColor(px, ny, radius, paintColor, brushCenter, brushBuffer[point3Index]);
                        brushBuffer[point3Index] = brushColor;
                        
                        textureBuffer[point3Index] = ProcessBrushColor(paintMode, brushColor, paintColor, sourceBuffer, point3Index);
                    }
                    if (point4Index < textureBuffer.Length && point4Index >= 0)
                    {
                        var brushColor = GetBrushColor(nx, ny, radius, paintColor, brushCenter, brushBuffer[point4Index]);
                        brushBuffer[point4Index] = brushColor;
                        
                        textureBuffer[point4Index] = ProcessBrushColor(paintMode, brushColor, paintColor, sourceBuffer, point4Index);
                    }
                }
            }
        }

        private static Color ProcessBrushColor(PaintMode paintMode, Color brushColor, Color paintColor, Color[] sourceBuffer, int pixelIndex)
        {
            return paintMode switch
            {
                PaintMode.Normal => ProcessNormalMode(brushColor, paintColor, sourceBuffer, pixelIndex),
                PaintMode.Additive => ProcessAdditiveMode(brushColor, sourceBuffer, pixelIndex),
                PaintMode.Subtract => ProcessSubtractMode(brushColor, sourceBuffer, pixelIndex),
                _ => Color.clear
            };
        }

        private static Color ProcessNormalMode(Color brushColor, Color paintColor, Color[] sourceBuffer, int pixelIndex)
        {
            var sourceColor = sourceBuffer[pixelIndex];
            var delta = paintColor.a - sourceColor.a;

            return new Color(0.0f, 0.0f, 0.0f, sourceColor.a + delta * (brushColor.a / paintColor.a));
        }

        private static Color ProcessAdditiveMode(Color brushColor, Color[] sourceBuffer, int pixelIndex)
        {
            return sourceBuffer[pixelIndex] + brushColor;
        }

        private static Color ProcessSubtractMode(Color brushColor, Color[] sourceBuffer, int pixelIndex)
        {
            return sourceBuffer[pixelIndex] - brushColor;
        }

        private Color GetBrushColor(int x, int y, float radius, Color paintColor, Vector2Int brushCenter, Color brushColor)
        {
            var distance = Vector2.Distance(brushCenter, new Vector2(x, y));
            var color    = paintColor;
            if (distance > radius * Strength)
            {
                color *= 1.0f - distance / radius;
            }

            brushColor += color;
            if (brushColor.a > paintColor.a)
            {
                brushColor = paintColor;
            }

            return brushColor;
        }
    }
}