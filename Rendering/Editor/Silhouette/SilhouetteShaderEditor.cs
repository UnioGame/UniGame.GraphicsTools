namespace Taktika.Rendering.Editor.Silhouette
{
    using UnityEditor;
    using UnityEngine;

    public class SilhouetteShaderEditor : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var material = materialEditor.target as Material;
            
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Outline Params", EditorStyles.boldLabel);

            var outlineColorProperty = FindProperty("_OutlineColor", properties);
            materialEditor.ColorProperty(outlineColorProperty, "Outline Color");

            var outlineThicknessProperty = FindProperty("_OutlineThickness", properties);
            materialEditor.RangeProperty(outlineThicknessProperty, "Thickness");
            
            var outlineAccuracyProperty = FindProperty("_OutlineAccuracy", properties);
            materialEditor.RangeProperty(outlineAccuracyProperty, "Accuracy");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(material);
            }
        }
    }
}