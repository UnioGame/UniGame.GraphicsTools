namespace UniGame.Rendering.Runtime.Blur.KawaseBlur
{
    public static class KawaseBlurGlobalSettings
    {
        public static bool IsEnable { get; private set; }

        public static void EnableBlur()
        {
            IsEnable = true;
        }

        public static void DisableBlur()
        {
            IsEnable = false;
        }
    }
}