namespace UniGame.Rendering.Editor.Water.Depth
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Runtime.Water;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    
    public class DepthPainter
    {
        private const int MaxHistory = 5;
        
        private readonly Water       _water;
        private readonly List<Brush> _registeredBrushes = new List<Brush>();

        private float     _depthValue = 1.0f;
        private PaintMode _paintMode  = PaintMode.Normal;
        
        private int _selectedBrushIndex = -1;

        private Color[] _sourceBuffer;
        private Color[] _brushBuffer;
        private Color[] _textureBuffer;
        
        private readonly LinkedList<Color[]> _history = new LinkedList<Color[]>();

        public DepthPainter(Water water)
        {
            _water = water;
        }

        public bool DrawBrushPanel()
        {
            SirenixEditorGUI.BeginBox(new GUIContent("Depth Painter"));
            {
                _depthValue = EditorGUILayout.Slider(new GUIContent("Depth"), _depthValue, 0.0f, 1.0f);
                _paintMode  = (PaintMode) EditorGUILayout.EnumPopup(new GUIContent("Mode"), _paintMode);

                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                for (var i = 0; i < _registeredBrushes.Count; i++)
                {
                    var enabled = GUILayout.Toggle(_selectedBrushIndex == i, _registeredBrushes[i].GetContent(), EditorStyles.miniButton);

                    _selectedBrushIndex = enabled ? i : _selectedBrushIndex == i ? -1 : _selectedBrushIndex;
                }

                GUILayout.EndHorizontal();

                if (_selectedBrushIndex != -1)
                {
                    var selectedBrush = _registeredBrushes[_selectedBrushIndex];
                    selectedBrush.DrawInspector();
                }
                
                if (GUILayout.Button("Clear Depth"))
                {
                    var depths = Water.GetDefaultDepths(_water.GetMaskSize());
                    _water.SetDepthMask(depths);
                }

                GUI.enabled = _history.Count > 0;
                if (GUILayout.Button("Undo"))
                {
                    var depths = _history.Last.Value;
                    _water.SetDepthMask(depths);
                    _history.RemoveLast();
                }

                GUI.enabled = true;
            }
            SirenixEditorGUI.EndBox();

            return _selectedBrushIndex != -1;
        }

        public void HandleMouse(Event currentEvent)
        {
            if(_selectedBrushIndex == -1)
                return;
            
            var mousePosition       = currentEvent.mousePosition;
            var mouseWorldPosition  = (Vector2) HandleUtility.GUIPointToWorldRay(mousePosition).origin;
            var mouseObjectPosition = _water.WorldToObject(mouseWorldPosition);

            if(!_water.IsPointInsideMesh(mouseObjectPosition))
                return;
            
            var selectedBrush = _registeredBrushes[_selectedBrushIndex];
            selectedBrush.DrawHandle(mouseWorldPosition);

            if(currentEvent.button != 0)
                return;
            
            var paintRect = GetPaintRect(mouseObjectPosition, selectedBrush, _water.Bounds, HandleUtility.GetHandleSize(mouseWorldPosition), _water.GetMaskSize());
            
            switch (currentEvent.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    break;
                case EventType.MouseDown:
                    currentEvent.Use();

                    InitializePaint();
                    ProcessPaint(selectedBrush, _water.DepthMaskRect, paintRect);
                    
                    break;
                case EventType.MouseDrag:
                    currentEvent.Use();
                    
                    ProcessPaint(selectedBrush, _water.DepthMaskRect, paintRect);
                    break;
                case EventType.MouseUp:
                    currentEvent.Use();

                    ReleasePaint();
                    break;
            }
        }

        public void RegisterBrush(Brush brush)
        {
            if(_registeredBrushes.Contains(brush))
                return;
            
            _registeredBrushes.Add(brush);
        }

        public void UnRegisterBrush(Brush brush)
        {
            if(!_registeredBrushes.Contains(brush))
                return;

            _registeredBrushes.Remove(brush);
        }

        public void ReleaseBrush()
        {
            _selectedBrushIndex = -1;
        }

        private void InitializePaint()
        {
            _sourceBuffer = _water.GetDepthMask();
            
            _brushBuffer = new Color[_sourceBuffer.Length];
            _textureBuffer = new Color[_sourceBuffer.Length];
            
            Array.Copy(_sourceBuffer, _textureBuffer, _sourceBuffer.Length);

            if (_history.Count >= MaxHistory)
            {
                _history.RemoveFirst();
            }

            _history.AddLast(_sourceBuffer);
        }

        private void ReleasePaint()
        {
            _sourceBuffer  = null;
            _brushBuffer   = null;
            _textureBuffer = null;
        }

        private void ProcessPaint(Brush selectedBrush, Rect sourceRect, Rect paintRect)
        {
            selectedBrush.Paint(sourceRect, paintRect, _paintMode, new Color(0.0f, 0.0f, 0.0f, _depthValue), _sourceBuffer, ref _brushBuffer, ref _textureBuffer);
            
            _water.SetDepthMask(_textureBuffer);
        }

        private static Rect GetPaintRect(Vector3 objectPosition, Brush selectedBrush, Bounds bounds, float handleSize, Vector2Int textureSize)
        {
            var cornerOffset = selectedBrush.Size * handleSize;
            
            var worldMinX = objectPosition.x - cornerOffset;
            var worldMaxX = objectPosition.x + cornerOffset;
            var worldMinY = objectPosition.y - cornerOffset;
            var worldMaxY = objectPosition.y + cornerOffset;
            
            var textureMinX = Mathf.CeilToInt(textureSize.x * WaterMeshUtility.ReMap(worldMinX, bounds.min.x, bounds.max.x));
            var textureMaxX = Mathf.CeilToInt(textureSize.x * WaterMeshUtility.ReMap(worldMaxX, bounds.min.x, bounds.max.x));
            var textureMinY = Mathf.CeilToInt(textureSize.y * WaterMeshUtility.ReMap(worldMinY, bounds.min.y, bounds.max.y));
            var textureMaxY = Mathf.CeilToInt(textureSize.y * WaterMeshUtility.ReMap(worldMaxY, bounds.min.y, bounds.max.y));
            
            var paintPosition = new Vector2(textureMinX, textureMinY);
            var paintSize     = new Vector2(textureMaxX - textureMinX, textureMaxY - textureMinY);
                
            return new Rect(paintPosition, paintSize);
        }
    }
}