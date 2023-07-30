namespace Taktika.Rendering.Runtime.Blur.KawaseBlur
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class KawaseBlurSettings
    {
        [SerializeField]
        private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        
        [SerializeField]
        private Material _material;

        [Range(2, 15)]
        [SerializeField]
        private int _passes = 2;

        [Range(1, 4)]
        [SerializeField]
        private int _downSample = 1;

        [SerializeField]
        private bool   _copyToFrameBuffer;
        [SerializeField]
        private string _targetName = "_blurTexture";

        public RenderPassEvent RenderPassEvent => _renderPassEvent;

        public Material Material => _material;

        public int Passes => _passes;
        public int DownSample => _downSample;

        public bool   CopyToFrameBuffer => _copyToFrameBuffer;
        public string TargetName        => _targetName;
    }
}