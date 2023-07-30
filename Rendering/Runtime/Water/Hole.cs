namespace Taktika.Rendering.Runtime.Water
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Hole
    {
        [SerializeField]
        private List<int> _indexes = new List<int>();

        public List<int> Indexes => _indexes;
    }
}