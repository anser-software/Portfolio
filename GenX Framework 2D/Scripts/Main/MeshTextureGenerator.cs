using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{

    public static class MeshTextureGenerator
    {

        /// <summary>
        /// Set texture of a chunk async.
        /// </summary>
        /// <param name="map">Block map of the chunk.</param>
        /// <param name="tiles">List of block types to generate.</param>
        /// <param name="targetChunk">MeshRenderer component of chunk game object.</param>
        /// <param name="loadSpeed">Texture generation speed. Large value - fast generation but low frame rate.</param>
        /// <returns></returns>
        public static IEnumerator GenerateTextureAsync(Block2D[,] map, List<BlockType2D> blockTypes, MeshRenderer targetChunk, float loadSpeed)
        {
            Resources.UnloadUnusedAssets();
            loadSpeed *= 100;
            int textureSizePerTile = Mathf.RoundToInt(MapManager2D.currentSquareSize * MapManager2D.currentBlockTextureSize);

            Texture2D output = new Texture2D(map.GetLength(0) * textureSizePerTile, map.GetLength(1) * textureSizePerTile);

            Texture2D[] tilesInstance = new Texture2D[blockTypes.Count];

            for (int i = 0; i < blockTypes.Count; i++)
            {
                if (blockTypes[i].thisDrawingType == DrawingType.OneTexture)
                {
                    //If block doesn't have a main texture - continue.
                    if (!blockTypes[i].mainTexture) continue;

                    //If size of the tile's
                    if (blockTypes[i].mainTexture.width != textureSizePerTile || blockTypes[i].mainTexture.height != textureSizePerTile)
                        tilesInstance[i] = Scale(blockTypes[i].mainTexture, textureSizePerTile, textureSizePerTile, FilterMode.Point);
                    else
                        tilesInstance[i] = blockTypes[i].mainTexture;
                }
                else if(blockTypes[i].thisDrawingType == DrawingType.Tilemap)
                {
                    foreach(Texture2D texture in blockTypes[i].tileTextures)
                    {
                        if(texture.width != textureSizePerTile || texture.height != textureSizePerTile)
                        {
                            Debug.LogError("Size of the texture " + texture.name + " is inappropriate. It is " + texture.width + "x" + texture.height + " when it should be " + textureSizePerTile + "x" + textureSizePerTile);
                            yield break;
                        }
                    }
                }
                else if (blockTypes[i].thisDrawingType == DrawingType.RandomTexture)
                {
                    if (blockTypes[i].randomTextures.Count < 1)
                    {
                        Debug.LogError("Block type \"" + blockTypes[i].name + "\" " + "(" + i + ")" + " has no textures to set.");
                        yield break;
                    }

                    foreach (Texture2D texture in blockTypes[i].randomTextures)
                    {
                        if (texture.width != textureSizePerTile || texture.height != textureSizePerTile)
                        {
                            Debug.LogError("Size of the texture " + texture.name + " is inappropriate. It is " + texture.width + "x" + texture.height + " when it should be " + textureSizePerTile + "x" + textureSizePerTile);
                            yield break;
                        }
                    }
                }
            }

            int loadingIndex = 0;

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y].blockType == 0) continue;

                    if (blockTypes[map[x, y].blockType - 1].thisDrawingType == DrawingType.OneTexture)
                    {
                        if (!blockTypes[map[x, y].blockType - 1].mainTexture) continue;
                        output.SetPixels(x * textureSizePerTile, y * textureSizePerTile, textureSizePerTile, textureSizePerTile,
                            tilesInstance[map[x, y].blockType - 1].GetPixels(0, 0, textureSizePerTile, textureSizePerTile));
                    }
                    else if (blockTypes[map[x, y].blockType - 1].thisDrawingType == DrawingType.Tilemap)
                    {
                        output.SetPixels(x * textureSizePerTile, y * textureSizePerTile, textureSizePerTile, textureSizePerTile,
                            blockTypes[map[x, y].blockType - 1].tileTextures[map[x, y].squareIndex].GetPixels(0, 0, textureSizePerTile, textureSizePerTile));
                    }
                    else if (blockTypes[map[x, y].blockType - 1].thisDrawingType == DrawingType.RandomTexture)
                    {
                        output.SetPixels(x * textureSizePerTile, y * textureSizePerTile, textureSizePerTile, textureSizePerTile,
                            blockTypes[map[x, y].blockType - 1].randomTextures[MapManager2D.random.Next(0,
                            blockTypes[map[x, y].blockType - 1].randomTextures.Count)].GetPixels(0, 0, textureSizePerTile, textureSizePerTile));
                    }

                    loadingIndex++;
                    if (loadingIndex == loadSpeed)
                    {
                        loadingIndex = 0;
                        yield return null;
                    }
                }
            }

            output.filterMode = FilterMode.Point;
            output.wrapMode = TextureWrapMode.Clamp;
            output.Apply();

            if (!targetChunk)
                yield break;

            targetChunk.material.mainTexture = output;

            yield return null;
        }

        /// <summary>
        /// Set background texture of the terrain.
        /// </summary>
        /// <param name="backgroundTexture">Input texture.</param>
        /// <returns>Output texture</returns>
        public static Texture2D SetTerrainInside(Texture2D backgroundTexture)
        {
            Texture2D output = Object.Instantiate(backgroundTexture);

            output.wrapMode = TextureWrapMode.Repeat;

            return output;
        }

        /// <summary>
        /// Generate texture for a map.
        /// </summary>
        /// <param name="map">Input block map.</param>
        /// <param name="tiles">List of block types to generate.</param>
        /// <returns>Generated texture.</returns>
        public static Texture2D GenerateTexture(Block2D[,] map, List<BlockType2D> blockTypes, float squareSize)
        {
            Resources.UnloadUnusedAssets();
            int textureSizePerTile = Mathf.RoundToInt(squareSize * MapManager2D.currentBlockTextureSize);

            Texture2D output = new Texture2D(map.GetLength(0) * textureSizePerTile, map.GetLength(1) * textureSizePerTile);

            Texture2D[] tilesInstance = new Texture2D[blockTypes.Count];

            for (int i = 0; i < blockTypes.Count; i++)
            {
                if (blockTypes[i].thisDrawingType == DrawingType.OneTexture)
                {
                    if (!blockTypes[i].mainTexture) continue;
                    if (blockTypes[i].mainTexture.width != textureSizePerTile || blockTypes[i].mainTexture.height != textureSizePerTile)
                        tilesInstance[i] = Scale(blockTypes[i].mainTexture, textureSizePerTile, textureSizePerTile, FilterMode.Point);
                    else
                        tilesInstance[i] = blockTypes[i].mainTexture;
                }
            }

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y].blockType == 0) continue;

                    if (blockTypes[map[x, y].blockType - 1].thisDrawingType == DrawingType.OneTexture)
                    {
                        if (!blockTypes[map[x, y].blockType - 1].mainTexture) continue;
                        output.SetPixels(x * textureSizePerTile, y * textureSizePerTile, textureSizePerTile, textureSizePerTile,
                            tilesInstance[map[x, y].blockType - 1].GetPixels(0, 0, textureSizePerTile, textureSizePerTile));
                    }
                    else
                    {

                        output.SetPixels(x * textureSizePerTile, y * textureSizePerTile, textureSizePerTile, textureSizePerTile,
                            blockTypes[map[x, y].blockType - 1].tileTextures[map[x, y].squareIndex].GetPixels(0, 0, textureSizePerTile, textureSizePerTile));

                    }
                }
            }

            output.filterMode = FilterMode.Point;
            output.wrapMode = TextureWrapMode.Clamp;
            output.Apply();

            return output;
        }

        /// <summary>
        /// Generate texture for a map with only one block type.
        /// </summary>
        /// <param name="map">Input block map.</param>
        /// <param name="tile">Block type to generate.</param>
        /// <returns>Generated texture.</returns>
        public static Texture2D GenerateTexture(Block2D[,] map, BlockType2D tile, float squareSize)
        {
            List<BlockType2D> b = new List<BlockType2D>();
            b.Add(tile);

            return GenerateTexture(map, b, squareSize);
        }

        /// <summary>
        /// Resize the input texture.
        /// </summary>
        /// <param name="source">Input texture.</param>
        /// <param name="newWidth">Target width.</param>
        /// <param name="newHeight">Target height.</param>
        /// <returns>Output texture.</returns>
        public static Texture2D Scale(Texture2D source, int newWidth, int newHeight, FilterMode filterMode)
        {
            source.filterMode = filterMode;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = filterMode;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            return nTex;
        }

    }
}