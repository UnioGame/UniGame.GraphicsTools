namespace UniModules.UniGame.GraphicsTools.Editor.TextureComparer
{
    using System;
    using System.Security.Cryptography;
    using Abstract;
    using UnityEngine;

    public class BytesSpriteComparer : ITextureComparer
    {
        private readonly MD5CryptoServiceProvider _hashAlgorithm = new MD5CryptoServiceProvider();

        public bool Compare(byte[] first, Rect firstRect, Sprite second)
        {
            if(first == null)
                throw new ArgumentNullException(nameof(first));
            if(second == null)
                throw new ArgumentNullException(nameof(second));
            
            if (Mathf.CeilToInt(firstRect.width) != Mathf.CeilToInt(second.textureRect.width) || Mathf.CeilToInt(firstRect.height) != Mathf.CeilToInt(second.textureRect.height))
            {
                return false;
            }
            
            var secondTexture = second.texture.GetTexture(second.textureRect, second.texture.format);
            var secondPixels  = secondTexture.EncodeToPNG();

            var firstHash  = ComputeHash(first);
            var secondHash = ComputeHash(secondPixels);
            
            return firstHash.Equals(secondHash);
        }

        private string ComputeHash(byte[] byteArray)
        {
            var hash       = _hashAlgorithm.ComputeHash(byteArray);
            var stringHash = Convert.ToBase64String(hash);

            return stringHash;
        }

        public void Dispose()
        {
            _hashAlgorithm?.Dispose();
        }
    }
}