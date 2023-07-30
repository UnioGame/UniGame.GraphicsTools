namespace UniGame.Rendering.Editor.Water.Depth
{
    using Abstract;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public class EraserBrush : Brush
    {
        public override GUIContent GetContent()
        {
            return EditorGUIUtility.IconContent("Grid.EraserTool");
        }

        public override void DrawInspector()
        {
            SirenixEditorGUI.BeginBox(new GUIContent("Eraser Properties"));
            {
                DrawSizeInspector();
            }
            SirenixEditorGUI.EndBox();
        }
        
        public override void DrawHandle(Vector3 position)
        {
            var handleSize = HandleUtility.GetHandleSize(position);
            var normal     = Quaternion.identity * Vector3.forward;
            
            Handles.DrawWireDisc(position, normal, Size * handleSize);
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
                    
                    if (point1Index < textureBuffer.Length && point1Index >= 0)
                    {
                        brushBuffer[point1Index]   = Color.clear;
                        textureBuffer[point1Index] = Color.clear;
                    }
                    
                    if (point2Index < textureBuffer.Length && point2Index >= 0)
                    {
                        brushBuffer[point2Index]   = Color.clear;
                        textureBuffer[point2Index] = Color.clear;
                    }
                    
                    if (point3Index < textureBuffer.Length && point3Index >= 0)
                    {
                        brushBuffer[point3Index]   = Color.clear;
                        textureBuffer[point3Index] = Color.clear;
                    }
                    
                    if (point4Index < textureBuffer.Length && point4Index >= 0)
                    {
                        brushBuffer[point4Index]   = Color.clear;
                        textureBuffer[point4Index] = Color.clear;
                    }
                }
            }
        }
    }
}