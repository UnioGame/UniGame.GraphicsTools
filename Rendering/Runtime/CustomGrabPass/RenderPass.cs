namespace UniGame.Rendering.Runtime.CustomGrabPass
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class RenderPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> _shaderTagIds = new List<ShaderTagId>();

        private FilteringSettings _filteringSettings;
        private RenderStateBlock  _renderStateBlock;

        public RenderPass(RenderPassEvent passEvent, LayerMask layerMask)
        {
            renderPassEvent = passEvent;
            
            _shaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));
            _shaderTagIds.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIds.Add(new ShaderTagId("LightweightForward"));

            _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            _renderStateBlock  = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("GrabRenderPass");

            var drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonTransparent);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings, ref _renderStateBlock);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }
    }
}