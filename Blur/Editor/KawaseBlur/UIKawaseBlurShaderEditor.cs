namespace UniGame.Rendering.Editor.Blur.KawaseBlur
{
    using JetBrains.Annotations;
    using UnityEditor;
    using UnityEngine;

    [UsedImplicitly]
    public class UIKawaseBlurShaderEditor : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var material = materialEditor.target as Material;
            
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Tint Params", EditorStyles.boldLabel);

            var outlineColorProperty = FindProperty("_Color", properties);
            materialEditor.ColorProperty(outlineColorProperty, "Tint Color");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(material);
            }
        }
    }
}