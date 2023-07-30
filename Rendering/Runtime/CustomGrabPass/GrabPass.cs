namespace Taktika.Rendering.Runtime.CustomGrabPass
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class GrabPass : ScriptableRenderPass
    {
        private RenderTargetHandle _tempColorHandle;
        
        private readonly GrabPassSettings   _settings;

        private RenderTargetIdentifier _cameraTargetIdentifier;

        public GrabPass(GrabPassSettings settings)
        {
            _settings       = settings;
            renderPassEvent = settings.RenderPassEvent;
            
            _tempColorHandle.Init(settings.TargetName);
        }

        public void Setup(RenderTargetIdentifier cameraTarget)
        {
            _cameraTargetIdentifier = cameraTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(_tempColorHandle.id, cameraTextureDescriptor);
            cmd.SetGlobalTexture(_settings.TargetName, _tempColorHandle.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("GrabPass");

            Blit(cmd, _cameraTargetIdentifier, _tempColorHandle.Identifier());
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempColorHandle.id);
        }
    }
}