using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace GenX2D
{

    public static class Terrain2D
    {
        /// <summary>
        /// Current biome index.
        /// </summary>
        public static int currentBiome;

        static int ChunksLeftInCurrentBiome = 0;

        static bool isBiomeExclusive;

        /// <summary>
        /// Get noise at position with the input values.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="scale">Noise map scale.</param>
        /// <param name="mag">Output value multiplier.</param>
        /// <param name="exp">Output value exponent.</param>
        /// <returns>Noise at position with the input values.</returns>
        public static int Noise(int x, int y, float scale, float mag, float exp)
        {
            return (int)(Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale) * mag), (exp)));
        }

        /// <summary>
        /// Manually set current biome index.
        /// </summary>
        /// <param name="biome">Biome index.</param>
        public static void SetBiome(int biome)
        {
            currentBiome = biome;
            
            isBiomeExclusive = true;
        }

        /// <summary>
        /// Generate block 2D terrain with x-offset.
        /// </summary>
        /// <param name="posX">x-offset</param>
        /// <param name="blockTypes">List of block types to generate.</param>
        /// <returns>Generated terrain.</returns>
        public static Block2D[,] GenerateTerrain(int posX, List<BlockType2D> blockTypes)
        {
            if (MapManager2D.mapManager.allBiomes.Count < 1)
            {
                Debug.LogError("No biomes have been definded.");
                return new Block2D[0, 0];
            }

            if(blockTypes.Count < 1)
            {
                Debug.LogError("No block types have been definded.");
                return new Block2D[0, 0];
            }

            if (ChunksLeftInCurrentBiome <= 0 && !isBiomeExclusive)
            {
                int newBiome = 0;

                int index = 0;

                do
                {
                    int randomBiome = MapManager2D.random.Next(0, 100);
                    int biomeIndex = 0;

                    int[] biomeIndicesSorted = MapManager2D.mapManager.allBiomes.OrderBy(b => b.generationChance).Select(b => b.index).ToArray();

                    for (int i = 0; i < MapManager2D.mapManager.allBiomes.Count; i++)
                    {
                        biomeIndex += MapManager2D.mapManager.allBiomes[biomeIndicesSorted[i]].generationChance;
                        if (biomeIndex >= randomBiome)
                        {
                            newBiome = biomeIndicesSorted[i];
                            break;
                        }
                    }

                    index++;
                } while (currentBiome == newBiome && index < 50 && MapManager2D.mapManager.allBiomes.Count > 1);

                currentBiome = newBiome;

                ChunksLeftInCurrentBiome = MapManager2D.random.Next(MapManager2D.mapManager.allBiomes[currentBiome].minSize, MapManager2D.mapManager.allBiomes[currentBiome].maxSize);
            }

            int thisSeed = int.Parse(MapManager2D.realSeed.ToString().Remove(3));

            posX += thisSeed;
            int posY = thisSeed;

            Block2D[,] blocks = new Block2D[MapManager2D.currentChunkSizeX, MapManager2D.currentChunkSizeY];

            int[] blockLevels = new int[blockTypes.Count];

            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int i = 0; i < blockTypes.Count; i++)
                {
                    if (!blockTypes[i].biomes.Contains(currentBiome)) continue;
                    if (blockTypes[i].thisGenType == GenerationType.RangeY && blockTypes[i].noiseLayers.Count > 0)
                    {
                        blockLevels[i] = Noise(x + posX, posY, blockTypes[i].noiseLayers[0].scale,
                                blockTypes[i].noiseLayers[0].magnitude, blockTypes[i].noiseLayers[0].exponent);

                        for (int n = 1; n < blockTypes[i].noiseLayers.Count; n++)
                        {
                            blockLevels[i] += Noise(x + posX, posY, blockTypes[i].noiseLayers[n].scale,
                                blockTypes[i].noiseLayers[n].magnitude, blockTypes[i].noiseLayers[n].exponent);
                        }
                    }
                }

                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int i = 0; i < blockLevels.Length; i++)
                    {
                        if (!blockTypes[i].biomes.Contains(currentBiome)) continue;
                        if (y < blockTypes[i].maxY && y >= blockTypes[i].minY)
                        {
                            if (blockTypes[i].thisGenType == GenerationType.RangeY && y < blockLevels[i] + blockTypes[i].yOffset && blocks[x, y].blockType == 0)
                            {
                                blocks[x, y].blockType = (byte)(i + 1);
                            }
                            else if (blockTypes[i].thisGenType == GenerationType.Group && (blocks[x, y].blockType != 0 || i == 0))
                            {
                                if (Noise(x + posX, y + posY, 13, 40, 1) < blockTypes[i].groupSize)
                                    blocks[x, y].blockType = (byte)(i + 1);
                            }
                            else if (blockTypes[i].thisGenType == GenerationType.Random && MapManager2D.random.Next(0, 100) < blockTypes[i].spawnChance && blocks[x, y].blockType != 0)
                            {
                                blocks[x, y].blockType = (byte)(i + 1);
                            }
                        }

                    }

                    if (blocks[x, y].blockType != 0 && y + ((posY - thisSeed) * MapManager2D.currentChunkSizeY) < MapManager2D.mapManager.caveSpawnLimitY && Noise(x + posX, y + posY, MapManager2D.mapManager.caveScale,
                        MapManager2D.mapManager.caveMagnitude, 1) < MapManager2D.mapManager.cavesSize && MapManager2D.mapManager.allBlockTypes[blocks[x, y].blockType - 1].canHaveCavesInside)
                    { //Caves
                        blocks[x, y].blockType = 0;
                    }
                }
            }

            ChunksLeftInCurrentBiome--;
            return blocks;
        }

        /// <summary>
        /// Generate only height values from the input noise.
        /// </summary>
        /// <param name="globalPositionX"></param>
        /// <param name="width">Size of the output array.</param>
        /// <param name="noiseLayers">Input noise layers.</param>
        /// <returns>Height array.</returns>
        public static int[] GeneratePeaks(int globalPositionX, int width, List<NoiseLayer> noiseLayers)
        {
            int[] peaks = new int[width];

            for (int x = 0; x < width; x++)
            {
                int peak = 0;

                foreach(NoiseLayer noiseLayer in noiseLayers)
                {
                    peak += Noise(x + globalPositionX, 0, noiseLayer.scale, noiseLayer.magnitude, noiseLayer.exponent);
                }

                peaks[x] = peak;
            }

            return peaks;
        }

        /// <summary>
        /// Returns smoothed points of the input.
        /// </summary>
        /// <param name="peaks">Input height array.</param>
        /// <returns></returns>
        public static Vector2[] SmoothPeaks(int[] peaks)
        {
            Vector2[] points;
            List<Vector2> smoothedPeaks;

            int curvedLength = peaks.Length - 1;

            smoothedPeaks = new List<Vector2>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new Vector2[peaks.Length];

                for(int i = 0; i< peaks.Length; i++)
                {
                    points[i] = new Vector2(i, peaks[i]);
                }

                for (int j = peaks.Length - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                smoothedPeaks.Add(points[0]);
            }

            return (smoothedPeaks.ToArray());
        }
    }

    

    public struct Block2D
    {
        public byte blockType;
        public byte squareIndex;

        public Block2D(byte _blockType)
        {
            blockType = _blockType;
            squareIndex = 0;
        }
    }

    [Serializable]
    public class Biome
    {
        //Name of the biome
        public string name;

        /// <summary>
        /// Chance that next biome will be this one
        /// </summary>
        public int generationChance;

        /// <summary>
        /// Biome's unique index
        /// </summary>
        public int index;

        /// <summary>
        /// Min and max size in chunks
        /// </summary>
        public int minSize;
        public int maxSize;

        /// <summary>
        /// Background texture
        /// </summary>
        public Texture2D backgroundTexture;

        public Biome(string _name, int _generationChance, int _index, int _minSize, int _maxSize)
        {
            name = _name;
            generationChance = _generationChance;
            index = _index;

            minSize = _minSize;
            maxSize = _maxSize;
        }
    }
}