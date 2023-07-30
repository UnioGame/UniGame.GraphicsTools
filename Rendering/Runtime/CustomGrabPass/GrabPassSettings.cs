namespace UniGame.Rendering.Runtime.CustomGrabPass
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class GrabPassSettings
    {
        public string    TargetName = "_GrabPass";
        
        public LayerMask LayerMask;
        public LayerMask AfterLayerMask;

        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }
}