namespace UniModules.UniGame.GraphicsTools.Editor.TextureComparer
{
    using System;
    using System.Security.Cryptography;
    using Abstract;
    using UnityEngine;

    public class Texture2DComparer : ITextureComparer
    {
        private readonly MD5CryptoServiceProvider _cryptoServiceProvider;

        public Texture2DComparer()
        {
            _cryptoServiceProvider = new MD5CryptoServiceProvider();
        }

        public bool Compare(Texture2D first, Texture2D second)
        {
            if (first.width != second.width || first.height != second.height)
            {
                return false;
            }
            
            var firstRawData  = first.GetRawTextureData();
            var secondRawData = second.GetRawTextureData();

            var firstHash  = ComputeHash(firstRawData);
            var secondHash = ComputeHash(secondRawData);

            return firstHash.Equals(secondHash);
        }

        private string ComputeHash(byte[] byteArray)
        {
            return Convert.ToBase64String(_cryptoServiceProvider.ComputeHash(byteArray));
        }

        public void Dispose()
        {
            _cryptoServiceProvider.Dispose();
        }
    }
}