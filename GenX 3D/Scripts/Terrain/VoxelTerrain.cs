using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrain : MonoBehaviour {

    #region General
    public int size = 20;

    public static VoxelTerrain main { get; set; }
    #endregion

    #region Generation
    public Material material;

    public bool useSeed;
    public string seed;

    /// <summary>
    /// List of all block types
    /// </summary>
    public List<BlockType> blockTypes;

    /// <summary>
    /// The noise used to generate caves
    /// </summary>
    public NoiseLayer caveNoise;

    /// <summary>
    /// The cave generation threshold
    /// </summary>
    public float caveSize;
    #endregion

    void Awake()
    {
        main = FindObjectOfType<VoxelTerrain>();
    }

    /// <summary>
    /// Create a structure GameObject at position with current values.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Created GameObject</returns>
    public GameObject Create(Vector3 position)
    {
        //Generate a map
        Voxel[,,] map = GenerateTerrain((int)position.x, (int)position.y, (int)position.z);

        GameObject result = new GameObject("Voxel Terrain");

        if ((map.GetLength(0) * map.GetLength(1) * map.GetLength(2)) * 2 < 65000)
        {
            //If resulting mesh can't have more than 65000 verts
            result.AddComponent<MeshRenderer>().material = material;
            result.AddComponent<MeshFilter>().mesh = MeshMakerBlocks.GenerateMesh(map);
        }
        else
        {
            //If resulting mesh CAN have more than 65000 verts we split it
            Mesh[] meshes = MeshMakerBlocks.GenerateMeshes(map, blockTypes.Count);

            for (int i = 0; i < meshes.Length; i++)
            {
                GameObject currentMesh = new GameObject(i.ToString());

                Material[] mats = new Material[blockTypes.Count];

                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = blockTypes[j].material;
                }

                currentMesh.AddComponent<MeshRenderer>().materials = mats;

                currentMesh.AddComponent<MeshFilter>().mesh = meshes[i];

                currentMesh.transform.parent = result.transform;
            }
        }

        result.transform.position = position;

        return result;
    }

    /// <summary>
    /// Returns a generated 3D voxel map of the terrain with current values.
    /// </summary>
    /// <param name="globalXpos"></param>
    /// <param name="globalYpos"></param>
    /// <param name="globalZpos"></param>
    /// <returns></returns>
    public Voxel[,,] GenerateTerrain(int globalXpos, int globalYpos, int globalZpos)
    {
        //Is we don't use seed or it's not set - assign a random seed
        if (!useSeed || seed == null || seed.Trim().Length < 1) seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

        System.Random rand = new System.Random(seed.GetHashCode());

        Random.InitState(rand.Next(-9000, 9000));

        int offset = rand.Next(-9000, 9000);

        Voxel[,,] map = new Voxel[size, size, size];

        int[] blockLevels = new int[blockTypes.Count];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(2); z++)
            {
                for (int i = 0; i < blockTypes.Count; i++)
                {
                    if (blockTypes[i].genType == GenerationType.Range && blockTypes[i].noiseLayers.Count > 0)
                    {
                        blockLevels[i] = (int)Noise.Get2D(x + globalXpos + offset, z + globalZpos + offset, blockTypes[i].noiseLayers[0].scale,
                            blockTypes[i].noiseLayers[0].magnitude, blockTypes[i].noiseLayers[0].exponent);

                        for (int n = 1; n < blockTypes[i].noiseLayers.Count; n++)
                        {
                            blockLevels[i] += (int)Noise.Get2D(x + globalXpos + offset, z + globalZpos + offset, blockTypes[i].noiseLayers[n].scale,
                                blockTypes[i].noiseLayers[n].magnitude, blockTypes[i].noiseLayers[n].exponent);
                        }
                    }
                }

                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int i = 0; i < blockLevels.Length; i++)
                    {
                        if (y < blockTypes[i].maxY && y >= blockTypes[i].minY)
                        {
                            if (blockTypes[i].genType == GenerationType.Range && y + globalYpos < blockLevels[i] + blockTypes[i].yOffset && map[x, y, z].TypeID == 0)
                            {
                                map[x, y, z].TypeID = (byte)(i + 1);
                            }
                            else if (blockTypes[i].genType == GenerationType.Group && (map[x, y, z].TypeID != 0 || i == 0))
                            {
                                if (Noise.Get3D(x + globalXpos + offset, y + globalYpos + offset, y + globalZpos + offset, 15, 100, 1) < blockTypes[i].groupSize)
                                    map[x, y, z].TypeID = (byte)(i + 1);
                            }
                            else if (blockTypes[i].genType == GenerationType.Random && Random.Range(0F, 100F) < blockTypes[i].spawnChance && map[x, y, z].TypeID != 0)
                            {
                                map[x, y, z].TypeID = (byte)(i + 1);
                            }
                        }

                    }

                    //Caves
                    if (map[x, y, z].TypeID != 0 && blockTypes[map[x, y, z].TypeID - 1].canHaveCavesInside &&
                        Noise.Get3D(x + globalXpos + offset, y + globalYpos + offset, z + globalZpos + offset,
                        caveNoise.scale, caveNoise.magnitude, caveNoise.exponent) < caveSize)
                    {
                        map[x, y, z].TypeID = 0;
                    }
                }
            }
        }

        return map;
    }

    /// <summary>
    /// Returns a generated 3D voxel map of the terrain with given values.
    /// </summary>
    /// <param name="globalXpos"></param>
    /// <param name="globalYpos"></param>
    /// <param name="globalZpos"></param>
    /// <param name="chunkSize"></param>
    /// <param name="_blockTypes"></param>
    /// <param name="_caveUpperYLimit">The height, above which caves won't be generated</param>
    /// <param name="_caveNoise"></param>
    /// <param name="_caveSize"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Voxel[,,] GenerateTerrain(int globalXpos, int globalYpos, int globalZpos, int chunkSize, 
        List<BlockType> _blockTypes, int _caveUpperYLimit, NoiseLayer _caveNoise, int _caveSize, int offset = 0)
    {
        Random.InitState(offset);

        Voxel[,,] map = new Voxel[chunkSize, chunkSize, chunkSize];

        int[] blockLevels = new int[_blockTypes.Count];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(2); z++)
            {
                for (int i = 0; i < _blockTypes.Count; i++)
                {
                    //if (!_blockTypes[i].biomes.Contains(currentBiome)) continue;
                    if (_blockTypes[i].genType == GenerationType.Range && _blockTypes[i].noiseLayers.Count > 0)
                    {
                        blockLevels[i] = (int)Noise.Get2D(x + globalXpos + offset, z + globalZpos + offset, _blockTypes[i].noiseLayers[0].scale,
                            _blockTypes[i].noiseLayers[0].magnitude, _blockTypes[i].noiseLayers[0].exponent);

                        for (int n = 1; n < _blockTypes[i].noiseLayers.Count; n++)
                        {
                            blockLevels[i] += (int)Noise.Get2D(x + globalXpos + offset, z + globalZpos + offset, _blockTypes[i].noiseLayers[n].scale,
                                _blockTypes[i].noiseLayers[n].magnitude, _blockTypes[i].noiseLayers[n].exponent);
                        }
                    }
                }

                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int i = 0; i < blockLevels.Length; i++)
                    {
                        //if (!_blockTypes[i].biomes.Contains(currentBiome)) continue;

                        if (y < _blockTypes[i].maxY && y >= _blockTypes[i].minY)
                        {
                            if (_blockTypes[i].genType == GenerationType.Range && y + globalYpos < blockLevels[i] + _blockTypes[i].yOffset && map[x, y, z].TypeID == 0)
                            {
                                map[x, y, z].TypeID = (byte)(i + 1);
                            }
                            else if (_blockTypes[i].genType == GenerationType.Group && (map[x, y, z].TypeID != 0 || i == 0))
                            {
                                if (Noise.Get3D(x + globalXpos + offset, y + globalYpos + offset, y + globalZpos + offset, 15, 100, 1) < _blockTypes[i].groupSize)
                                    map[x, y, z].TypeID = (byte)(i + 1);
                            }
                            else if (_blockTypes[i].genType == GenerationType.Random && Random.Range(0F, 100F) < _blockTypes[i].spawnChance && map[x, y, z].TypeID != 0)
                            {
                                map[x, y, z].TypeID = (byte)(i + 1);
                            }
                        }

                    }

                    //Caves
                    if (map[x, y, z].TypeID != 0 && _blockTypes[map[x, y, z].TypeID - 1].canHaveCavesInside &&
                        Noise.Get3D(x + globalXpos + offset, y + globalYpos + offset, z + globalZpos + offset,
                        _caveNoise.scale, _caveNoise.magnitude, _caveNoise.exponent) < _caveSize)
                    { 
                        map[x, y, z].TypeID = 0;
                    }
                }
            }
        }

        return map;
    }
}
