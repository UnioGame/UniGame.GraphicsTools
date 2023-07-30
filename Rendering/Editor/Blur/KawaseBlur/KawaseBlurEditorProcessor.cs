namespace UniGame.Rendering.Editor.Blur.KawaseBlur
{
    using Runtime.Blur.KawaseBlur;
    using UnityEditor;

    [InitializeOnLoad]
    public static class KawaseBlurEditorProcessor
    {
        static KawaseBlurEditorProcessor()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                KawaseBlurGlobalSettings.DisableBlur();
            }
        }
    }
}