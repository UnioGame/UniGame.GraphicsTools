namespace Taktika.Rendering.Runtime.Water
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class Water : MonoBehaviour
    {
        private static readonly int DepthMaskPropertyId = Shader.PropertyToID("_DepthMask");

        private static readonly int ViewPos              = Shader.PropertyToID("_ViewPos");
        private static readonly int WaterGradientTexture = Shader.PropertyToID("_WaterGradientTexture");

        private const int PixelsPerUnit = 10;
        
        private static readonly Color   DefaultColor   = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        private static readonly Color32 DefaultColor32 = new Color32(0, 0, 0, 0);

        [SerializeField]
        private Vector3 _viewPositionOffset = new Vector3(0.5f, 0.1f, 0.0f);
        [SerializeField]
        private Gradient _waterGradient = new Gradient();

        [SerializeField]
        private WaterMesh _waterMesh;

        [SerializeField]
        private Texture2D _depthMask;
        
        private MeshRenderer _meshRenderer;
        private MeshFilter   _meshFilter;
        
        private MaterialPropertyBlock _materialPropertyBlock;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField]
        private Bounds _previousBounds;

        private MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter == null)
                    _meshFilter = GetComponent<MeshFilter>();

                return _meshFilter;
            }
        }

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                    _meshRenderer = GetComponent<MeshRenderer>();

                return _meshRenderer;
            }
        }

        public WaterMesh WaterMesh
        {
            get => _waterMesh;
            set
            {
                _waterMesh = value;
                
                MeshFilter.mesh = _waterMesh.Mesh;
            }
        }

        public Gradient WaterGradient => _waterGradient;

        public Rect DepthMaskRect => new Rect(0.0f, 0.0f, DepthMask.width, DepthMask.height);

        public Bounds Bounds => WaterMesh.IsNull ? new Bounds() : WaterMesh.Mesh.bounds;

        private Texture2D DepthMask
        {
            get
            {
                if (_depthMask == null)
                {
                    _previousBounds = Bounds;
                    _depthMask      = CreateDepthMask(GetMaskSize());
                }

                return _depthMask;
            }
        }
        
        private MaterialPropertyBlock MaterialPropertyBlock
        {
            get { return _materialPropertyBlock ??= new MaterialPropertyBlock(); }
        }

        public void ResizeDepthMask()
        {
            if (_previousBounds == Bounds)
                return;

            var size = GetMaskSize();
            ResizeDepthMask(size, WaterMeshUtility.GetResizeDirection(_previousBounds, Bounds));
            _previousBounds = Bounds;
        }

        public void SetDepthMask(Color[] depths)
        {
            if(DepthMask == null || depths.Length != DepthMask.width * DepthMask.height)
                return;
            
            DepthMask.SetPixels(depths);
            DepthMask.Apply();
        }

        public void SetDepthMask(Rect rect, Color[] depths)
        {
            if(DepthMask == null)
                return;
            
            DepthMask.SetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, depths);
            DepthMask.Apply();
        }

        public Color[] GetDepthMask(Rect rect)
        {
            return DepthMask == null ? null : DepthMask.GetPixels((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
        }
        
        public Color[] GetDepthMask()
        {
            return DepthMask == null ? null : DepthMask.GetPixels();
        }

        public Vector2Int GetMaskSize(int pixelsPerUnit = PixelsPerUnit)
        {
            var bounds = WaterMesh.Mesh.bounds;
            var xSize  = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
            var ySize  = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);
            
            return new Vector2Int(xSize, ySize);
        }

        public static Color[] GetDefaultDepths(Vector2Int size)
        {
            var depths = new Color[size.x * size.y];
            for (var i = 0; i < depths.Length; i++)
            {
                depths[i] = DefaultColor;
            }

            return depths;
        }
        
        public void SetGradient(Texture2D gradientTexture)
        {
            MeshRenderer.sharedMaterial.SetTexture(WaterGradientTexture, gradientTexture);
        }
        
        // TODO: пока отключаем, так как маска рисуется вручную в Adobe Photoshop
        private void UpdateMaterial()
        {
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetTexture(DepthMaskPropertyId, _depthMask);
            
            MeshRenderer.SetPropertyBlock(propertyBlock);
        }

        private void ResizeDepthMask(Vector2Int size, ResizeDirection resizeDirection)
        {
            if(_depthMask == null)
                return;
            
            var newPixels = WaterMeshUtility.ResizeCanvas(_depthMask.GetPixels32(), _depthMask.width, _depthMask.height, size.x, size.y, DefaultColor32, resizeDirection);
            _depthMask.Reinitialize(size.x, size.y, TextureFormat.Alpha8, false);
            _depthMask.SetPixels32(newPixels);
            _depthMask.Apply();
        }
        
        private void InitMaterial(Camera drawingCamera)
        {
            SetViewPosition(drawingCamera, MaterialPropertyBlock);

            MeshRenderer.SetPropertyBlock(MaterialPropertyBlock);
        }

        private void SetViewPosition(Camera drawingCamera, MaterialPropertyBlock propertyBlock)
        {
            var cameraPosition       = drawingCamera.transform.position;
            var cameraObjectPosition = transform.InverseTransformPoint(cameraPosition);
            cameraObjectPosition.z = 1.0f;
            
            cameraObjectPosition += _viewPositionOffset;

            propertyBlock.SetVector(ViewPos, cameraObjectPosition);
        }

        private static Texture2D CreateDepthMask(Vector2Int size)
        {
            var depthMask        = new Texture2D(size.x, size.y, TextureFormat.Alpha8, false);
            var depthArray = GetDefaultDepths(size);
            
            depthMask.SetPixels(depthArray, 0);
            depthMask.Apply();

            return depthMask;
        }
        
        private void OnBeginCameraRendering(ScriptableRenderContext renderContext, Camera drawingCamera)
        {
            InitMaterial(drawingCamera);
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }
        
        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
    }
}