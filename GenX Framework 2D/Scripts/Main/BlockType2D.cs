using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{

    [System.Serializable]
    public enum DrawingType { OneTexture, Tilemap, RandomTexture }

    [System.Serializable]
    public enum GenerationType { RangeY, Random, Group }

    [System.Serializable]
    public class BlockType2D
    {

        /// <summary>
        /// Editor UI only!
        /// </summary>
        public bool hideInEditor;

        /// <summary>
        /// Editor UI only!
        /// </summary>
        public bool foldoutInEditor;

        /// <summary>
        /// Editor UI only!
        /// </summary>
        public bool hideTileTextures;

        public string name = "Name";

        /// <summary>
        /// OneTexture - every block has one particular texure;
        /// <para /> Tilemap - every block's texture depends on it's placement.
        /// <para /> RandomTexture - select a random texture from 'randomTextures' list.
        /// </summary>
        public DrawingType thisDrawingType;

        /// <summary>
        /// All texures a block can have if it's drawing type is set to RandomTexture;
        /// </summary>
        public List<Texture2D> randomTextures = new List<Texture2D>();

        /// <summary>
        /// Texture every block will have if 'thisDrawingType' is set to OneTexture.
        /// </summary>
        public Texture2D mainTexture;

        public bool canHaveCavesInside;

        /// <summary>
        /// All texures a block can have if it's drawing type is set to Tilemap;
        /// <para>For the order see documentation or block's tab in the inspector.</para>
        /// </summary>
        public Texture2D[] tileTextures = new Texture2D[16];

        /// <summary>
        /// Global height offset of this block type.
        /// </summary>
        public int yOffset;

        /// <summary>
        /// Blocks of this type will not be generated higher than this value.
        /// </summary>
        public int maxY;

        /// <summary>
        /// Blocks of this type will not be generated lower than this value.
        /// </summary>
        public int minY;

        /// <summary>
        /// RangeY - 1D Perlin noise based generation;
        /// <para /> Random - every block has a random chance of being this type;
        /// <para /> Group - 2D Perlin noise based generation.
        /// </summary>
        public GenerationType thisGenType = new GenerationType();

        /// <summary>
        /// List of biomes (Theid indices) that this block type can be generated in.
        /// </summary>
        public List<int> biomes = new List<int>();

        /// <summary>
        /// List of noise layers for this block type's generation
        /// </summary>
        public List<NoiseLayer> noiseLayers = new List<NoiseLayer>();

        /// <summary>
        /// If 'thisGenType' is Random - chance that a block will be of this type
        /// </summary>
        public float spawnChance;

        /// <summary>
        /// If 'thisGenType' is Group - size of the group
        /// </summary>
        public int groupSize;

        /// <summary>
        /// Make a copy of the other block type.
        /// </summary>
        /// <param name="original"></param>
        public BlockType2D(BlockType2D original)
        {
            name = original.name;
            groupSize = original.groupSize;
            spawnChance = original.spawnChance;
            noiseLayers = original.noiseLayers;
            biomes = original.biomes;
            thisGenType = original.thisGenType;
            minY = original.minY;
            maxY = original.maxY;
            yOffset = original.yOffset;
            tileTextures = original.tileTextures;
            mainTexture = original.mainTexture;
            thisDrawingType = original.thisDrawingType;
        }

        public BlockType2D() { }

    }


    [System.Serializable]
    public class NoiseLayer
    {
        public float scale;
        public float magnitude;
        public float exponent;

        public NoiseLayer(float _scale, float _magnitude, float _exponent)
        {
            scale = _scale;
            magnitude = _magnitude;
            exponent = _exponent;
        }
    }
}