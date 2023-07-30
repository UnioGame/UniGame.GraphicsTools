namespace UniGame.Rendering.Runtime.Blur.KawaseBlur
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class KawaseBlurRenderPass : ScriptableRenderPass
    {
        private const string PropertyName1 = "tmpBlurRT1";
        private const string PropertyName2 = "tmpBlurRT2";

        private const string OffsetPropertyName = "_offset";
            
        public Material Material;
        public int      Passes;
        public int      DownSample;
        public bool     CopyToFrameBuffer;
        public string   TargetName;
            
        private readonly string _profilerTag;

        private int _tempId1;
        private int _tempId2;

        private RenderTargetIdentifier _tempRenderTarget1;
        private RenderTargetIdentifier _tempRenderTarget2;

        private RenderTargetIdentifier _source;

        public void Setup(RenderTargetIdentifier source)
        {
            _source = source;
        }

        public KawaseBlurRenderPass(string profilerTag)
        {
            _profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width  = cameraTextureDescriptor.width / DownSample;
            var height = cameraTextureDescriptor.height / DownSample;

            _tempId1 = Shader.PropertyToID(PropertyName1);
            _tempId2 = Shader.PropertyToID(PropertyName2);
            cmd.GetTemporaryRT(_tempId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(_tempId2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            _tempRenderTarget1 = new RenderTargetIdentifier(_tempId1);
            _tempRenderTarget2 = new RenderTargetIdentifier(_tempId2);

            ConfigureTarget(_tempRenderTarget1);
            ConfigureTarget(_tempRenderTarget2);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(!KawaseBlurGlobalSettings.IsEnable)
                return;
                
            var cmd = CommandBufferPool.Get(_profilerTag);

            var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
                
            cmd.SetGlobalFloat(OffsetPropertyName, 1.5f);
            cmd.Blit(_source, _tempRenderTarget1, Material);

            for (var i = 1; i < Passes - 1; i++)
            {
                cmd.SetGlobalFloat(OffsetPropertyName, 0.5f + i);
                cmd.Blit(_tempRenderTarget1, _tempRenderTarget2, Material);
                    
                var temp = _tempRenderTarget1;
                _tempRenderTarget1 = _tempRenderTarget2;
                _tempRenderTarget2 = temp;
            }
                
            cmd.SetGlobalFloat(OffsetPropertyName, 0.5f + Passes - 1f);
            if (CopyToFrameBuffer)
            {
                cmd.Blit(_tempRenderTarget1, _source, Material);
            }
            else
            {
                cmd.Blit(_tempRenderTarget1, _tempRenderTarget2, Material);
                cmd.SetGlobalTexture(TargetName, _tempRenderTarget2);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }
}