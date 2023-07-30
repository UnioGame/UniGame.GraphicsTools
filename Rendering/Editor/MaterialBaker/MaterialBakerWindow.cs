namespace Taktika.Rendering.Editor.ShaderBaker
{
    using System.IO;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    public class MaterialBakerWindow : OdinEditorWindow
    {
        private Material   _materialToBake;
        
        private Vector2Int _size = new Vector2Int(512, 512);
        
        private string     _pathToSave;
        private bool       _sRGB;

        private bool _isBakeProcessing;

        private TextureFormat _textureFormat = TextureFormat.RGBA32;

        [MenuItem("Rendering/Material Baker")]
        private static void OpenWindow()
        {
            var window = GetWindow<MaterialBakerWindow>();
            
            window.minSize      = new Vector2(256f, 256f);
            window.titleContent = new GUIContent("Material Baker");

            window.Show();
        }

        protected override void OnGUI()
        {
            _materialToBake = (Material)EditorGUILayout.ObjectField(new GUIContent("Material to Bake"), _materialToBake, typeof(Material), false);
            _sRGB           = EditorGUILayout.Toggle(new GUIContent("sRGB"), _sRGB);
            _size           = EditorGUILayout.Vector2IntField(new GUIContent("Texture Size"), _size);
            _textureFormat  = (TextureFormat)EditorGUILayout.EnumPopup(new GUIContent("Texture Format"), _textureFormat);
            
            GUILayout.BeginHorizontal();
            {
                _pathToSave = SirenixEditorFields.FilePathField(new GUIContent("Path to save"), _pathToSave, "Assets", ".png", true, true);
            }
            GUILayout.EndHorizontal();

            GUI.enabled = _materialToBake != null && !string.IsNullOrEmpty(_pathToSave) || _isBakeProcessing;
            if (GUILayout.Button("Bake"))
            {
                _isBakeProcessing = true;
            }
            GUI.enabled = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
        }

        private void OnEndFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            if (_isBakeProcessing)
            {
                var renderTexture = RenderTexture.GetTemporary(_size.x, _size.y, 0);
                
                Graphics.SetRenderTarget(renderTexture);
                Graphics.Blit(null, renderTexture, _materialToBake);

                var result = new Texture2D(renderTexture.width, renderTexture.height, _textureFormat, false, _sRGB);
                var oldRT  = RenderTexture.active;
                RenderTexture.active = renderTexture;
                
                result.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height),0, 0);
                result.Apply();

                RenderTexture.active = oldRT;

                var bytes = result.EncodeToPNG();

                File.WriteAllBytes(_pathToSave, bytes);

                AssetDatabase.Refresh();

                renderTexture.Release();
                _isBakeProcessing = false;
            }
        }
    }
}