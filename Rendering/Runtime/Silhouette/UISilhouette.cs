namespace Taktika.Rendering.Runtime.Silhouette
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public sealed class UISilhouette : MonoBehaviour
    {
        private Image _image;

        private static readonly int RectPropertyID             = Shader.PropertyToID("_Rect");
        private static readonly int OutlineThicknessPropertyID = Shader.PropertyToID("_OutlineThickness");

        private Material _materialInstance;

        private float _startOutlineThickness;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            
            _startOutlineThickness = _image.material.GetFloat(OutlineThicknessPropertyID);
        }

        public void SetupMaterial(Sprite sprite)
        {
            _image.sprite = sprite;

            if (_materialInstance == null)
            {
                _materialInstance = Instantiate(_image.material);
            }

            var rect = new Vector4(sprite.textureRect.min.x/sprite.texture.width,
                sprite.textureRect.min.y/sprite.texture.height,
                sprite.textureRect.max.x/sprite.texture.width,
                sprite.textureRect.max.y/sprite.texture.height);

            var multiplier = sprite.textureRect.height / _image.rectTransform.rect.height;

            _materialInstance.SetFloat(OutlineThicknessPropertyID, multiplier * _startOutlineThickness);

            _materialInstance.SetVector(RectPropertyID, rect);

            if (_image.material != _materialInstance)
            {
                _image.material = _materialInstance;
            }
        }
    }
}