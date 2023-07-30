namespace UniGame.Rendering.Editor.Blur.KawaseBlur
{
    using JetBrains.Annotations;
    using UnityEditor;
    using UnityEngine;

    [UsedImplicitly]
    public class KawaseBlurShaderEditor : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var material = materialEditor.target as Material;
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Label("There is no parameters to set.");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(material);
            }
        }
    }
}