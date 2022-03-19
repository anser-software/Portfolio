using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{
    [System.Serializable]
    public class Structure2D
    {

        public int width;
        public int height;

        public bool biomeDependent = false;

        /// <summary>
        /// Indices of biomes where this structure will spawn.
        /// </summary>
        public List<int> biomeIndices = new List<int>();

        public int spawnChance;
        public bool spawnAnywhere = true;

        public bool spawnAboveGround = true;
        public bool spawnBelowGround = true;
        public bool spawnOnlyLeft;
        public bool spawnOnlyUp;
        public bool spawnOnlyRight;
        public bool spawnOnlyDown;

        public bool inverse = false;

        public GameObject objectPrefab;
    }
}