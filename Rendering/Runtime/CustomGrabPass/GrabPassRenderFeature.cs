namespace Taktika.Rendering.Runtime.CustomGrabPass
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class GrabPassRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private GrabPassSettings _settings;
        
        private GrabPass   _grabPass;
        private RenderPass _renderPass;
        private RenderPass _endRenderPass;

        public override void Create()
        {
            _grabPass = new GrabPass(_settings);
            _renderPass = new RenderPass(_settings.RenderPassEvent + 1, _settings.LayerMask);
            _endRenderPass = new RenderPass(_settings.RenderPassEvent + 2, _settings.AfterLayerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _grabPass.Setup(renderer.cameraColorTarget);
            
            renderer.EnqueuePass(_grabPass);
            renderer.EnqueuePass(_renderPass);
            renderer.EnqueuePass(_endRenderPass);
        }
    }
}