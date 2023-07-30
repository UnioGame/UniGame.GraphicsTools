namespace Taktika.Rendering.Runtime.Blur.KawaseBlur
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class KawaseBlurRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private KawaseBlurSettings _settings = new KawaseBlurSettings();
        
        private KawaseBlurRenderPass _kawaseBlurRenderPass;
        
        public override void Create()
        {
            _kawaseBlurRenderPass = new KawaseBlurRenderPass("KawaseBlur")
            {
                Material      = _settings.Material,
                Passes            = _settings.Passes,
                DownSample        = _settings.DownSample,
                CopyToFrameBuffer = _settings.CopyToFrameBuffer,
                TargetName        = _settings.TargetName,
                renderPassEvent   = _settings.RenderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var src = renderer.cameraColorTarget;
            _kawaseBlurRenderPass.Setup(src);
            renderer.EnqueuePass(_kawaseBlurRenderPass);
        }
    }
}