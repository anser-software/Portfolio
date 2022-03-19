using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{

    public class Chunk2D : MonoBehaviour
    {

        /// <summary>
        /// Vertex indices of this chunk mesh, that have been destroyed.
        /// </summary>
        public List<int> destroyedIndices = new List<int>();

        /// <summary>
        /// Mesh of this chunk.
        /// </summary>
        public Mesh thisChunkMesh;

        /// <summary>
        /// Highest y coordinate that have a block, of every x coordinate
        /// </summary>
        public int[] peaks;

        /// <summary>
        /// This chunk's map
        /// </summary>
        public Block2D[,] blocks;

        void Start()
        {
            thisChunkMesh = GetComponent<MeshFilter>().mesh;
        }

        public void SetMap(Block2D[,] _map)
        {
            blocks = _map;
            peaks = new int[MapManager2D.currentChunkSizeX];

            int thisSeed = int.Parse(MapManager2D.realSeed.ToString().Remove(3));

            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if (y > 0 && blocks[x, y].blockType == 0 && blocks[x, y - 1].blockType != 0)
                    {
                        //Set peaks
                        peaks[x] = y;
                    }

                    //Set the square index of every block (See documentation for more)
                    if (blocks[x, y].blockType != 0)
                    {
                        blocks[x, y].squareIndex = 0;

                        if (MapManager2D.mapManager.allBlockTypes[blocks[x, y].blockType - 1].thisDrawingType == DrawingType.OneTexture)
                            continue;

                        if (y <= 0 || y >= blocks.GetLength(1) - 1)
                        {
                            blocks[x, y].squareIndex = 15;
                            continue;
                        }


                        if (x <= 0 || x >= blocks.GetLength(0) - 1)
                        {
                            int level = 0;

                            BlockType2D blockType = MapManager2D.mapManager.allBlockTypes[blocks[x, y].blockType - 1];

                            if (x >= blocks.GetLength(0) - 1)
                            {
                                foreach (NoiseLayer n in blockType.noiseLayers)
                                {
                                    level += Terrain2D.Noise(x + thisSeed + 1 + (int)transform.position.x, thisSeed, n.scale, n.magnitude, n.exponent);
                                }

                                if (y < level + blockType.yOffset)
                                    blocks[x, y].squareIndex += 2;

                                if (blocks[x - 1, y].blockType != 0) blocks[x, y].squareIndex += 8;

                            }
                            else if (x <= 0)
                            {
                                foreach (NoiseLayer n in blockType.noiseLayers)
                                {
                                    level += Terrain2D.Noise(x + thisSeed - 1 + (int)transform.position.x, thisSeed, n.scale, n.magnitude, n.exponent);
                                }

                                if (y < level + blockType.yOffset)
                                    blocks[x, y].squareIndex += 8;

                                if (blocks[x + 1, y].blockType != 0) blocks[x, y].squareIndex += 2;
                            }

                            if (blocks[x, y + 1].blockType != 0) blocks[x, y].squareIndex += 1;
                            if (blocks[x, y - 1].blockType != 0) blocks[x, y].squareIndex += 4;

                            continue;
                        }

                        if (blocks[x, y + 1].blockType != 0) blocks[x, y].squareIndex += 1;
                        if (blocks[x + 1, y].blockType != 0) blocks[x, y].squareIndex += 2;
                        if (blocks[x, y - 1].blockType != 0) blocks[x, y].squareIndex += 4;
                        if (blocks[x - 1, y].blockType != 0) blocks[x, y].squareIndex += 8;
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D trigger)
        {
            //If the camera is on this chunk, generate neighbour chunks
            if (trigger.CompareTag("MainCamera"))
            {
                StartCoroutine(MapManager2D.mapManager.GenerateChunk((int)transform.position.x, 1, 0.4F));
            }
        }

        void Update()
        {
            //Unload this chunk if the camera is far enough.
            if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > MapManager2D.currentChunkUnloadDistance)
            {
                StartCoroutine(MapManager2D.UnloadChunk((int)transform.position.x, 0));
            }
        }
    }
}