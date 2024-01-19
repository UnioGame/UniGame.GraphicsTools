namespace UniGame.Rendering.Editor.Water
{
    using System.IO;
    using Depth;
    using Mesh;
    using Runtime.Utils;
    using Runtime.Water;
    
    using UniModules.Editor;
    using UnityEditor;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.Utilities.Editor;
#endif
    
    [CustomEditor(typeof(Water))]
    public class WaterEditor : Editor
    {
        private Water _waterTarget;
        private bool  _meshEditingEnabled;
        private bool  _maskEditingEnabled;

        private DepthPainter _depthPainter;
        private MeshEditor   _meshEditor;

        private SerializedProperty _viewPositionOffsetProperty;
        private SerializedProperty _gradientProperty;

        private bool _gradientChanged;

        private Texture2D _gradientTexture;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
#if ODIN_INSPECTOR
            SirenixEditorGUI.BeginBox(new GUIContent("Water Settings"));
            {
                EditorGUILayout.PropertyField(_viewPositionOffsetProperty, new GUIContent("View Position Offset"));
                
                EditorGUI.BeginChangeCheck();

                var gradientChangedMark = _gradientChanged ? "*" : string.Empty;
                EditorGUILayout.PropertyField(_gradientProperty, new GUIContent($"Water Gradient{gradientChangedMark}"));
                
                if(EditorGUI.EndChangeCheck())
                {
                    _gradientChanged = true;
                    
                    _gradientTexture = RenderingUtils.CreateGradientTexture(_waterTarget.WaterGradient);
                    
                    _waterTarget.SetGradient(_gradientTexture);
                }

                GUI.enabled = _gradientChanged;
                {
                    if (GUILayout.Button("Save Gradient"))
                    {
                        var path = EditorUtility.SaveFilePanel("Save Water Gradient Texture", "Assets", "Water Gradient", "png");

                        SaveGradient(path);

                        _gradientChanged = false;
                    }
                }
                GUI.enabled = true;
            }
            SirenixEditorGUI.EndBox();
#endif
            
            _meshEditingEnabled = _meshEditor.DrawMeshPanel();
            if (_meshEditingEnabled)
            {
                _depthPainter.ReleaseBrush();
            }

            GUI.enabled         = false;
            _maskEditingEnabled = _depthPainter.DrawBrushPanel();
            if (_maskEditingEnabled)
            {
                _meshEditor.ReleaseEditing();
            }

            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }

        private void SaveGradient(string path)
        {
            var textureBytes = _gradientTexture.EncodeToPNG();
                        
            File.WriteAllBytes(path, textureBytes);
                        
            AssetDatabase.Refresh();

            var localGradientTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path.ToProjectPath());
            if (localGradientTexture != null)
            {
                _waterTarget.SetGradient(localGradientTexture);
            }
        }

        private void OnSceneGUI()
        {
            _meshEditor.HandleMeshManipulators();
            _depthPainter.HandleMouse(Event.current);
        }

        private void OnEnable()
        {
            _waterTarget = target as Water;
            if(_waterTarget == null)
                return;

            _viewPositionOffsetProperty = serializedObject.FindProperty("_viewPositionOffset");
            _gradientProperty           = serializedObject.FindProperty("_waterGradient");
            
            _depthPainter = new DepthPainter(_waterTarget);
            _depthPainter.RegisterBrush(new DepthBrush());
            _depthPainter.RegisterBrush(new EraserBrush());
            
            _meshEditor = new MeshEditor(_waterTarget, Repaint);
            _meshEditor.RegisterManipulator(new SelectVertexManipulator(_waterTarget));
            _meshEditor.RegisterManipulator(new MoveVertexManipulator(_waterTarget));
            _meshEditor.RegisterManipulator(new AddVertexManipulator(_waterTarget));
            _meshEditor.RegisterManipulator(new RemoveVertexManipulator(_waterTarget));
            _meshEditor.RegisterManipulator(new AddHoleManipulator(_waterTarget));
        }
    }
}