using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{

    [System.Serializable]
    public class MapManager2D : MonoBehaviour
    {

        /// <summary>
        /// Instance of 'MapManager2D' script in the scene for easier access.
        /// </summary>
        public static MapManager2D mapManager;

        //************Editor settings***************

        public bool editorHideBiomes;
        public bool editorHideBlocks;
        public bool editorHideStructures;

        [SerializeField]
        int menuSelection;

        [SerializeField]
        int typeSelection;

        //***********General settings***************

        /// <summary>
        /// Size of 1 square in units.
        /// </summary>
        public float squareSize = 1F;
        public static float currentSquareSize { get { return mapManager.squareSize; } }

        /// <summary>
        /// If unchecked - seed is random.
        /// </summary>
        public bool useSeed;

        /// <summary>
        /// Map generation seed. Leave empty for random seed.
        /// </summary>
        public string seed;

        /// <summary>
        /// Returns 'seed' variable of the 'MapManager2D' instance.
        /// </summary>
        public static string currentSeed { get { return mapManager.seed; } }

        public static int realSeed;

        /// <summary>
        /// Global instance of System.Random class.
        /// </summary>
        public static System.Random random;

        /// <summary>
        /// List of all structures.
        /// </summary>
        public List<Structure2D> allStructures = new List<Structure2D>();

        /// <summary>
        /// If checked colliders will be generated for the output map.
        /// </summary>
        public bool generateColliders;

        /// <summary>
        /// If checked - map will be destructible.
        /// </summary>
        public bool allowDestruction;

        //Mesh

        /// <summary>
        /// If checked the edges of the mesh will be smoothed.
        /// </summary>
        public bool smoothMesh;

        /// <summary>
        /// Material of the output mesh.
        /// </summary>
        public Material meshMaterial;


        //*******Cellular automata settings**********

        /// <summary>
        /// Width of the cellular automata map.
        /// </summary> 
        public int width = 60;

        /// <summary>
        /// Width of the cellular automata map.
        /// </summary> 
        public static int mapWidth { get { return mapManager.width; } }

        /// <summary>
        /// Height of the cellular automata map.
        /// </summary>
        public int height = 60;

        /// <summary>
        /// Height of the cellular automata map.
        /// </summary>
        public static int mapHeight { get { return mapManager.width; } }

        /// <summary>
        /// If during a simulation a living cell has less neighbours than this value - it dies.
        /// </summary>
        public int deathThreshold = 4;

        /// <summary>
        /// If during a simulation a dead cell has more neighbours than this value - it becomes alive.
        /// </summary>
        public int birthThreshold = 4;

        /// <summary>
        /// Chance that during map initialization a cell will be declared alive.
        /// </summary>
        public int chanceToStartAlive = 50;

        /// <summary>
        /// How many times should the simulation repeat.
        /// </summary>
        public int outputGeneration = 5;

        /// <summary>
        /// Any cave that has less empty cells inside of it than this value will be filled
        /// </summary>
        public int caveAliveThreshold = 0;

        /// <summary>
        /// Any island that is made up of less cells than this value will be destroyed
        /// </summary>
        public int islandAliveThreshold = 0;

        /// <summary>
        /// Last generated 2-dimensional map, where false means cell is dead, true - it's alive
        /// </summary>
        public static bool[,] lastMap;


        //*************Terrain settings*****************

        /// <summary>
        /// X-axis size of the chunk
        /// </summary>
        public int chunkSizeX = 32;

        /// <summary>
        /// X-axis size of the chunk
        /// </summary>
        public static int currentChunkSizeX { get { return mapManager.chunkSizeX; } }

        /// <summary>
        /// Y-axis size of the chunk
        /// </summary>
        public int chunkSizeY = 128;

        /// <summary>
        /// Y-axis size of the chunk
        /// </summary>
        public static int currentChunkSizeY { get { return mapManager.chunkSizeY; } }

        /// <summary>
        /// Texture size of every block
        /// </summary>
        public int blockTextureSize = 16;

        /// <summary>
        /// Texture size of every block
        /// </summary>
        public static int currentBlockTextureSize { get { return mapManager.blockTextureSize; } }

        /// <summary>
        /// X-axis distance from the main camera to a chunk, at which that chunk is destroyed
        /// </summary>
        public int chunkUnloadDistance = 75;

        /// <summary>
        /// X-axis distance from the main camera to a chunk, at which that chunk is destroyed
        /// </summary>
        public static int currentChunkUnloadDistance { get { return mapManager.chunkUnloadDistance; } }

        //**Cave settings**

        public int cavesSize = 3;

        public int caveScale = 11;

        public int caveMagnitude = 8;

        /// <summary>
        /// Caves will not appear higher than this value
        /// </summary>
        public int caveSpawnLimitY = 120;


        /// <summary>
        /// List of all block types
        /// </summary>
        public List<BlockType2D> allBlockTypes = new List<BlockType2D>();

        /// <summary>
        /// List of all biomes
        /// </summary>
        public List<Biome> allBiomes = new List<Biome>();

        //************Dungeon setting*****************

        /// <summary>
        /// Min size a room can have.
        /// </summary>
        public int minRoomSize = 20;

        /// <summary>
        /// Max size a room can have.
        /// </summary>
        public int maxRoomSize = 30;

        /// <summary>
        /// Min number of rooms to generate.
        /// </summary>
        public int minRoomCount = 4;

        /// <summary>
        /// Max number of rooms to generate.
        /// </summary>
        public int maxRoomCount = 6;

        /// <summary>
        /// Thickness of room border
        /// </summary>
        public int borderThickness = 7;

        /// <summary>
        /// Width of passageways
        /// </summary>
        public int passageWidth = 2;

        /// <summary>
        /// Thickness of the passageway border
        /// </summary>
        public int passageBorderThickness = 4;

        public int minDistanceBetweenRooms = 35;
        public int maxDistanceBetweenRooms = 40;

        //**Room Generation**

        /// <summary>
        /// How full is the room 0 - 100%
        /// </summary>
        public int roomFullness = 50;

        public int roomSmoothness = 7;

        /// <summary>
        /// First generation value
        /// </summary>
        public int valueOffset = 4;

        /// <summary>
        /// Second generation value
        /// </summary>
        public int valueDifference = 1;


        void Awake()
        {
            mapManager = FindObjectOfType<MapManager2D>();
            //Dungeon2D.GenerateSquareDungeon(50, 50, 15, 25, 15, 25, 16, 20, 1.5F, 3);
            RegenerateSeed();
        }

        /// <summary>
        /// Set new random seed.
        /// </summary>
        public void RegenerateSeed ()
        {
            if (seed.Trim().Length < 1 || !useSeed) seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

            realSeed = seed.GetHashCode();
            random = new System.Random(realSeed);
        }

        //*******Cellular automata methods**********

        /// <summary>
        /// Create a cave at position using values from 'MapManager2D' in the scene.
        /// </summary>
        /// <param name="position">Position of the cave.</param>
        /// <param name="blockType">Block type that will make up this cave.</param>
        /// <param name="_seed">Generation seed.</param>
        /// <returns>GameObject of the created cave.</returns>
        public static GameObject CreateCave(Vector3 position, BlockType2D blockType, string _seed = null)
        {
            if (!mapManager)
            {
                Debug.LogError("There's no 'MapManager2D' script in the scene. Call the other overload of this method or attach 'MapManager2D' to any game object.");
                return null;
            }

            _seed = _seed ?? mapManager.seed;

            //Generate the map
            bool[,] bitMap = CellularAutomata.Generate(mapManager.width, mapManager.height, mapManager.deathThreshold,
                mapManager.birthThreshold, mapManager.chanceToStartAlive, mapManager.outputGeneration, mapManager.caveAliveThreshold, mapManager.islandAliveThreshold, _seed);

            //Create cave game object
            GameObject caveGameObject = new GameObject("Cave", new MeshFilter().GetType(), new MeshRenderer().GetType());
            caveGameObject.GetComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(bitMap, 1F, mapManager.smoothMesh);

            MeshRenderer caveMeshRenderer = caveGameObject.GetComponent<MeshRenderer>();
            caveMeshRenderer.material = mapManager.meshMaterial;
            caveMeshRenderer.material.mainTexture = MeshTextureGenerator.GenerateTexture(BoolToBlockMap(bitMap), blockType, 1F);

            caveGameObject.transform.position = position;

            if (mapManager)
                if (mapManager.generateColliders)
                    MeshMaker.MakeCollider(bitMap, caveGameObject.transform);

            return caveGameObject;
        }

        /// <summary>
        /// Create a cave at position with custom values.
        /// </summary>
        /// <param name="position">Position of the cave.</param>
        /// <param name="_width">Width of the cave.</param>
        /// <param name="_height">Height of the cave.</param>
        /// <param name="_deathThreshold">If during a simulation a living cell has less neighbours than this value - it dies.</param>
        /// <param name="_birthThreshold">If during a simulation a dead cell has more neighbours than this value - it becomes alive.</param>
        /// <param name="_chanceToStartAlive">Chance that during map initialization a cell will be declared alive.</param>
        /// <param name="_outputGeneration">How many times should the simulation repeat.</param>
        /// <param name="_caveAliveThreshold">Any cave that has less empty cells inside of it than this value will be filled.</param>
        /// <param name="_islandAliveThreshold">Any island that is made up of less cells than this value will be destroyed.</param>
        /// <param name="_seed">Generation seed.</param>
        /// <param name="smooth">Smooth the mesh.</param>
        /// <returns>GameObject of the created cave.</returns>
        public static GameObject CreateCave(Vector3 position, BlockType2D blockType, int _width, int _height, int _deathThreshold = -1, int _birthThreshold = -1, int _chanceToStartAlive = -1, int _outputGeneration = -1, int _caveAliveThreshold = -1, int _islandAliveThreshold = -1, string _seed = null, bool smooth = false)
        {
            if (!mapManager.meshMaterial)
                Debug.LogError("Please set mesh material variable of the MapManager2D script.");

            try
            {
                _seed = _seed ?? mapManager.seed;

                //Set all the values the weren't specified from the existing 'MapManager2D instance.
                if (mapManager)
                {
                    _deathThreshold = (_deathThreshold == -1) ? mapManager.deathThreshold : _deathThreshold;
                    _birthThreshold = (_birthThreshold == -1) ? mapManager.birthThreshold : _birthThreshold;
                    _chanceToStartAlive = (_chanceToStartAlive == -1) ? mapManager.chanceToStartAlive : _chanceToStartAlive;
                    _outputGeneration = (_outputGeneration == -1) ? mapManager.outputGeneration : _outputGeneration;
                    _caveAliveThreshold = (_caveAliveThreshold == -1) ? mapManager.caveAliveThreshold : _caveAliveThreshold;
                    _islandAliveThreshold = (_islandAliveThreshold == -1) ? mapManager.islandAliveThreshold : _islandAliveThreshold;
                }
            }
            catch
            {
                Debug.LogError("Not every value was specified and there's no 'MapManager2D' script in the scene. Attach 'MapManager2D' to any game object or specify every value when calling this method.");
                return null;
            }
            
            //Generate the map
            bool[,] bitMap = CellularAutomata.Generate(_width, _height, _deathThreshold, _birthThreshold,
                _chanceToStartAlive, _outputGeneration, _caveAliveThreshold, _islandAliveThreshold, _seed);

            
            //Create cave game object
            GameObject caveGameObject = new GameObject("Cave", new MeshFilter().GetType(), new MeshRenderer().GetType());
            caveGameObject.GetComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(bitMap, 1F, smooth);

            MeshRenderer caveMeshRenderer = caveGameObject.GetComponent<MeshRenderer>();
            caveMeshRenderer.material = mapManager.meshMaterial;
            caveMeshRenderer.material.mainTexture = MeshTextureGenerator.GenerateTexture(BoolToBlockMap(bitMap), blockType, 1F);

            caveGameObject.transform.position = position;

            if (mapManager)
                if (mapManager.generateColliders)
                    MeshMaker.MakeCollider(bitMap, caveGameObject.transform);

            return caveGameObject;
        }

        /// <summary>
        /// Create a cave at position from preset values.
        /// </summary>
        /// <param name="position">Position of the cave.</param>
        /// <param name="_width">Width of the cave.</param>
        /// <param name="_height">Height of the cave.</param>
        /// <param name="preset">Preset.</param>
        /// <param name="_seed">Generation seed.</param>
        /// <param name="smooth">Smooth the mesh.</param>
        /// <returns></returns>
        public static GameObject CreateCave(Vector3 position, BlockType2D blockType, int _width, int _height, GenerationPreset preset, string _seed = null, bool smooth = false)
        {
            switch(preset)
            {
                case GenerationPreset.PolishedCave:
                    return CreateCave(position, blockType, _width, _height, 4, 4, 50, 10, 125, 125, smooth: smooth);
                case GenerationPreset.BlockyCave:
                    return CreateCave(position, blockType, _width, _height, 3, 3, 31, 7, 10, 10, smooth: smooth);
                case GenerationPreset.OpenCave:
                    return CreateCave(position, blockType, _width, _height, 4, 4, 42, 7, 25, 25, smooth: smooth);
                case GenerationPreset.Facility:
                    return CreateCave(position, blockType, _width, _height, 5, 5, 67, 15, 20, 20, smooth: smooth);
                case GenerationPreset.Maze:
                    return CreateCave(position, blockType, _width, _height, 7, 0, 50, 7, 0, 0, smooth: smooth);
                case GenerationPreset.CityLayout:
                    return CreateCave(position, blockType, _width, _height, 7, 0, 2, 7, 0, 0, smooth: smooth);
                default: //Circuit
                    return CreateCave(position, blockType, _width, _height, 7, 0, 20, 7, 0, 0, smooth: smooth);
            }
        }

        /// <summary> 
        /// Returns current map with one more simulation step.
        /// </summary>
        public static bool[,] AddStep()
        {
            lastMap = CellularAutomata.Simulate(lastMap, mapManager.deathThreshold, mapManager.birthThreshold);
            return lastMap;
        }

        static GameObject[,] cells;

        /// <summary>
        /// Makes a map of prefabs from 'inputMap' at 'centerPos' position.
        /// </summary>
        /// <param name="prefab">Input prefab</param>
        /// <param name="inputMap"></param>
        /// <param name="centerPos">Center position of the map</param>
        /// <returns>Map game object.</returns>
        public static GameObject PrefabMap(GameObject prefab, bool[,] inputMap, Vector3 centerPos)
        {
            GameObject mapParent = null;

            if (cells.GetLength(0) != inputMap.GetLength(0) || cells.GetLength(1) != inputMap.GetLength(1))
            {
                mapParent = new GameObject("Map");
                cells = new GameObject[inputMap.GetLength(0), inputMap.GetLength(1)];
                for (int x = 0; x < inputMap.GetLength(0); x++)
                {
                    for (int y = 0; y < inputMap.GetLength(1); y++)
                    {
                        Transform cell = Instantiate(prefab).transform;
                        cell.transform.position = new Vector3(x + centerPos.x - (inputMap.GetLength(0) / 2), y + centerPos.y - (inputMap.GetLength(1) / 2));
                        cell.parent = mapParent.transform;
                        cells[x, y] = cell.gameObject;

                        if (inputMap[x, y])
                        {
                            cell.gameObject.SetActive(true);
                        }
                        else cell.gameObject.SetActive(false);
                    }
                }
            }

            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                for (int y = 0; y < inputMap.GetLength(1); y++)
                {
                    if (inputMap[x, y])
                    {
                        cells[x, y].SetActive(true);
                    }
                }
            }

            return mapParent;
        }

        /// <summary>
        /// Destroys a cell at position.
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        public static Mesh DestroyCell(int xPos, int yPos)
        {
            if (lastMap == null) return null;

            int convertedPositionX = Mathf.RoundToInt(((lastMap.GetLength(0) / 2) + xPos) / mapManager.squareSize);
            int convertedPositionY = Mathf.RoundToInt(((lastMap.GetLength(1) / 2) + yPos) / mapManager.squareSize);

            if (CellularAutomata.IsOutOfMap(convertedPositionX, convertedPositionY, lastMap)) return MeshMaker.lastMapMesh;

            lastMap[convertedPositionX, convertedPositionY] = false;

            return MeshMaker.DestroyFace(xPos, yPos);
        }

        //*************Terrain methods*****************

        /// <summary>
        /// Dictionary of every currently loaded chunk; key - x axis position.
        /// </summary>
        public static Dictionary<int, GameObject> chunks = new Dictionary<int, GameObject>();

        /// <summary>
        /// Asynchronously load chunks at x-axis position and radius.
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="xRadius"></param>
        /// <param name="loadSpeed">Speed at which to load chunks.</param>
        /// <returns></returns>
        public IEnumerator GenerateChunk(int xPosition, int xRadius, float loadSpeed = 1F)
        {
            if (mapManager.allBiomes.Count < 1)
            {
                Debug.LogError("No biomes have been definded.");
                yield break;
            }

            if (mapManager.allBlockTypes.Count < 1)
            {
                Debug.LogError("No block types have been definded.");
                yield break;
            }


            for (int x = -xRadius; x <= xRadius; x++)
            {
                if (chunks.ContainsKey((x * currentChunkSizeX) + xPosition))
                {
                    yield return null;
                    continue;
                }

                //Generate the chunk map
                Block2D[,] blockMap = Terrain2D.GenerateTerrain((x * currentChunkSizeX) + xPosition, mapManager.allBlockTypes);

                //Create chunk game object
                GameObject chunkGameObject = new GameObject("Chunk_" + (x + (xPosition / currentChunkSizeX)), new MeshFilter().GetType(), new MeshRenderer().GetType());
                chunks.Add((int)(x * currentChunkSizeX * squareSize) + xPosition, chunkGameObject);
                chunkGameObject.transform.position = new Vector3((x * currentChunkSizeX * squareSize) + xPosition, 0);

                yield return null;

                //Generate the mesh
                chunkGameObject.GetComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(blockMap, mapManager.squareSize, mapManager.smoothMesh);

                //Add chunk script to the object
                Chunk2D thisChunk = chunkGameObject.AddComponent<Chunk2D>();
                thisChunk.SetMap(blockMap);

                yield return null;

                //If the chunk was unloaded by the time we reached this line, skip to the next.
                if (!chunkGameObject)
                    continue;

                bool[,] boolMap = BlockToBoolMap(blockMap);

                //Generate chunk's background
                Texture2D bg = allBiomes.FirstOrDefault(b => b.index == Terrain2D.currentBiome).backgroundTexture;
                if (bg != null)
                {
                    GameObject chunkInside = new GameObject("ChunkA_" + x, new MeshFilter().GetType(), new MeshRenderer().GetType());
                    MeshFilter insideMeshFilter = chunkInside.GetComponent<MeshFilter>();

                    insideMeshFilter.mesh = MeshMaker.MakePlane(currentChunkSizeX, currentChunkSizeY, mapManager.squareSize, thisChunk.peaks);

                    MeshRenderer chunkInsideRenderer = chunkInside.GetComponent<MeshRenderer>();

                    Material mat = new Material(Shader.Find("MapGen2D/TextureColorUnlit"));
                    mat.mainTexture = MeshTextureGenerator.SetTerrainInside(allBiomes.FirstOrDefault(b => b.index == Terrain2D.currentBiome).backgroundTexture);
                    mat.color = new Color(0.5F, 0.5F, 0.5F);
                    mat.mainTextureScale = new Vector2(chunkSizeX, chunkSizeY);

                    chunkInsideRenderer.material = mat;

                    chunkInside.transform.position = new Vector3((x * currentChunkSizeX) + xPosition, 0, -1F) + (Vector3.forward * 2);
                    chunkInside.transform.parent = chunkGameObject.transform;
                }
                
                BoxCollider2D box = chunkGameObject.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(currentChunkSizeX, currentChunkSizeY);
                box.offset = new Vector2(currentChunkSizeX * 0.5F, currentChunkSizeY * 0.5F);

                MeshRenderer meshRenderer = chunkGameObject.GetComponent<MeshRenderer>();
                meshRenderer.material = mapManager.meshMaterial;
                yield return StartCoroutine(MeshTextureGenerator.GenerateTextureAsync(blockMap, mapManager.allBlockTypes, meshRenderer, loadSpeed));

                yield return null;

                if (!chunkGameObject)
                    continue;

                if (generateColliders)
                {
                    Transform collidersTransform = new GameObject("Colliders").transform;
                    collidersTransform.parent = chunkGameObject.transform;
                    collidersTransform.localPosition = Vector3.zero;

                    MeshMaker.MakeCollider(boolMap, collidersTransform);

                    yield return null;
                }

                SpawnStructures(new Vector2((x * chunkSizeX) + xPosition, 0), boolMap, allStructures, thisChunk.peaks, chunkGameObject.transform);
                yield return null;
            }
        }

        /// <summary>
        /// Asynchronously unload chunks at x-axis position and radius.
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="xRadius"></param>
        /// <returns></returns>
        public static IEnumerator UnloadChunk(int xPosition, int xRadius)
        {
            for (int x = -xRadius; x <= xRadius; x++)
            {
                if (!chunks.ContainsKey((int)(x * currentChunkSizeX * mapManager.squareSize) + xPosition)) continue;

                Destroy(chunks[(x * currentChunkSizeX) + xPosition]);
                chunks.Remove((x * currentChunkSizeX) + xPosition);
                yield return null;
                Resources.UnloadUnusedAssets();
                yield return null;
            }
        }

        /// <summary>
        /// Spawn structures on 'map' with offset.
        /// </summary>
        /// <param name="offset">Offset for all structures.</param>
        /// <param name="map"></param>
        /// <param name="structures">Structures to spawn.</param>
        /// <param name="peaks">Peaks of the chunk (If spawning on terrain).</param>
        /// <param name="parent">Parent transform of every structure.</param>
        public static void SpawnStructures(Vector2 offset, bool[,] map, List<Structure2D> structures, int[] peaks = null, Transform parent = null)
        {
            if (structures.Count < 1)
                return;

            List<Structure2D> suitableStructures = new List<Structure2D>();

            foreach (Structure2D str in mapManager.allStructures)
            {
                if ((!str.biomeDependent || str.biomeIndices.Contains(Terrain2D.currentBiome)) && str.objectPrefab != null)
                    suitableStructures.Add(str);
            }

            int largestWidth = structures.Max(s => s.width);
            int largestHeight = structures.Max(s => s.height);

            for (int x = 1; x < map.GetLength(0) - 1 - largestWidth; x++)
            {
                for (int y = 1; y < map.GetLength(1) - 1 - largestHeight; y++)
                {
                    foreach (Structure2D str in suitableStructures)
                    {
                        if(str.spawnAnywhere)
                        {
                            if(!map[x,y] && !str.inverse)
                            {
                                if (random.Next(0, 1000) < str.spawnChance)
                                {
                                    Instantiate(str.objectPrefab, new Vector3(x + offset.x, y + offset.y, 1),
                                        Quaternion.identity).transform.parent = parent;  
                                }
                            }
                            else if (map[x, y] && str.inverse)
                            {
                                if (random.Next(0, 1000) < str.spawnChance)
                                {
                                    Instantiate(str.objectPrefab, new Vector3(x + offset.x, y + offset.y, 1),
                                        Quaternion.identity).transform.parent = parent;
                                }
                            }
                        }

                        if (peaks == null || (str.spawnAboveGround && y >= peaks[x]) || (str.spawnBelowGround && y < peaks[x]))
                        {
                            if (!map[x, y] && !str.inverse)
                            {
                                if ((str.spawnOnlyDown && map[x, y - 1]) || (str.spawnOnlyLeft && map[x - 1, y]) ||
                                (str.spawnOnlyUp && map[x, y + 1]) || (str.spawnOnlyRight && map[x + 1, y]))
                                {
                                    if (random.Next(0, 1000) < str.spawnChance)
                                    {
                                        bool unspawnable = false;

                                        for (int w = 0; w < str.width; w++)
                                        {
                                            for (int h = 0; h < str.height; h++)
                                            {
                                                if (map[x + w, y + h])
                                                {
                                                    unspawnable = true;
                                                    break;
                                                }
                                            }

                                            if (unspawnable)
                                                break;
                                        }

                                        if (!unspawnable)
                                        {
                                            Instantiate(str.objectPrefab, new Vector3(x + offset.x, y + offset.y, 1), Quaternion.identity).transform.parent = parent;
                                        }
                                    }
                                }
                            }
                            else if (map[x,y] && str.inverse)
                            {
                                if (str.spawnAnywhere || (str.spawnOnlyDown && !map[x, y - 1]) || (str.spawnOnlyLeft && !map[x - 1, y]) ||
                                (str.spawnOnlyUp && !map[x, y + 1]) || (str.spawnOnlyRight && !map[x + 1, y]))
                                {
                                    if (random.Next(0, 1000) < str.spawnChance)
                                    {
                                        Instantiate(str.objectPrefab, new Vector3(x + offset.x, y + offset.y, 1), 
                                            Quaternion.identity).transform.parent = parent;
                                    }
                                }
                            }
                        }
                    }                    
                }
            }
        }

        public static void DestroyBlockAtMousePosition(float maxDistanceFromPlayer = -1)
        {
            if (!mapManager.allowDestruction)
                return;

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (maxDistanceFromPlayer != -1 && Vector2.Distance(mouseWorldPos, Camera.main.transform.position) > maxDistanceFromPlayer)
                return;

            Collider2D coll = Physics2D.OverlapPoint(mouseWorldPos);

            Chunk2D thisChunk = coll.transform.GetComponent<Chunk2D>();

            if (thisChunk == null)
                return;

            coll.transform.GetComponent<MeshFilter>().mesh = MeshMaker.DestroyFace((int)(mouseWorldPos.x - thisChunk.transform.position.x), (int)(mouseWorldPos.y - thisChunk.transform.position.y), thisChunk);
        }

        //*********DUNGEON********

        /// <summary>
        /// Create a dungeon.
        /// </summary>
        /// <param name="pos">Position of the dungeon.</param>
        /// <param name="blockType">Type of blocks that will make up this dungeon.</param>
        /// <returns>GameObject of the created dungeon.</returns>
        public static GameObject CreateDungeon(Vector3 pos, BlockType2D blockType = null, string seed = null)
        {
            bool[,] dungeonMap = Dungeon2D.GenerateDungeon(mapManager.minRoomSize, mapManager.maxRoomSize, mapManager.minRoomCount, mapManager.maxRoomCount,
                mapManager.roomFullness, mapManager.valueOffset, mapManager.valueDifference, mapManager.roomSmoothness, mapManager.minDistanceBetweenRooms, mapManager.maxDistanceBetweenRooms, seed);

            GameObject dungeonGameObject = new GameObject("Dungeon");
            dungeonGameObject.AddComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(dungeonMap, currentSquareSize, mapManager.smoothMesh);
            dungeonGameObject.AddComponent<MeshRenderer>().material = Instantiate(mapManager.meshMaterial);

            if (blockType != null)
                dungeonGameObject.GetComponent<MeshRenderer>().material.mainTexture = MeshTextureGenerator.GenerateTexture(BoolToBlockMap(dungeonMap), blockType, currentSquareSize);

            dungeonGameObject.transform.position = pos;

            if (mapManager)
                if (mapManager.generateColliders)
                    MeshMaker.MakeCollider(dungeonMap, dungeonGameObject.transform);

            return dungeonGameObject;
        }

        /// <summary>
        /// Create a dungeon.
        /// </summary>
        /// <param name="pos">Position of the dungeon.</param>
        /// <param name="spawnStructures"></param>
        /// <param name="blockType">Type of blocks that will make up this dungeon.</param>
        /// <param name="_minRoomSize"></param>
        /// <param name="_maxRoomSize"></param>
        /// <param name="_minRoomCount"></param>
        /// <param name="_maxRoomCount"></param>
        /// <param name="_roomFullness"></param>
        /// <param name="_valueOffset"></param>
        /// <param name="_valueDifference"></param>
        /// <param name="_roomSmoothness"></param>
        /// <param name="seed">Generation seed.</param>
        /// <param name="smooth">Smooth the mesh.</param>
        /// <returns>GameObject of the created dungeon.</returns>
        public static GameObject CreateDungeon(Vector3 pos, int _minRoomSize, int _maxRoomSize, int _minRoomCount, int _maxRoomCount,
            int _roomFullness, int _valueOffset, int _valueDifference, int _roomSmoothness, int _minDistanceBetweenRooms, int _maxDistanceBetweenRooms, BlockType2D blockType = null, string seed = null, bool smooth = false)
        {
            bool[,] dungeonMap = Dungeon2D.GenerateDungeon(_minRoomSize, _maxRoomSize, _minRoomCount, _maxRoomCount,
                _roomFullness, _valueOffset, _valueDifference, _roomSmoothness, _minDistanceBetweenRooms, _maxDistanceBetweenRooms, seed);

            GameObject dungeonGameObject = new GameObject("Dungeon");
            dungeonGameObject.AddComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(dungeonMap, currentSquareSize, smooth);
            dungeonGameObject.AddComponent<MeshRenderer>().material = Instantiate(mapManager.meshMaterial);

            if (blockType != null)
                dungeonGameObject.GetComponent<MeshRenderer>().material.mainTexture = MeshTextureGenerator.GenerateTexture(BoolToBlockMap(dungeonMap), blockType, currentSquareSize);

            dungeonGameObject.transform.position = pos;

            if (mapManager)
                if (mapManager.generateColliders)
                    MeshMaker.MakeCollider(dungeonMap, dungeonGameObject.transform);

            return dungeonGameObject;
        }

        public static Block2D[,] BoolToBlockMap(bool[,] input)
        {
            Block2D[,] output = new Block2D[input.GetLength(0), input.GetLength(1)];

            for (int x = 0; x < output.GetLength(0); x++)
            {
                for (int y = 0; y < output.GetLength(1); y++)
                {
                    if (input[x, y])
                    {
                        output[x, y] = new Block2D(1);

                        if (y <= 0 || y >= output.GetLength(1) - 1)
                        {
                            output[x, y].squareIndex = 15;
                            continue;
                        }


                        if (x <= 0 || x >= output.GetLength(0) - 1)
                        {
                            output[x, y].squareIndex = 15;
                            continue;
                        }

                        if (input[x, y + 1]) output[x, y].squareIndex += 1;
                        if (input[x + 1, y]) output[x, y].squareIndex += 2;
                        if (input[x, y - 1]) output[x, y].squareIndex += 4;
                        if (input[x - 1, y]) output[x, y].squareIndex += 8;
                    }
                    else
                        output[x, y] = new Block2D(0);
                }
            }

            return output;
        }

        public static bool[,] BlockToBoolMap(Block2D[,] blockMap)
        {
            bool[,] boolMap = new bool[blockMap.GetLength(0), blockMap.GetLength(1)];

            for (int x = 0; x < boolMap.GetLength(0); x++)
            {
                for (int y = 0; y < boolMap.GetLength(1); y++)
                {
                    boolMap[x, y] = (blockMap[x, y].blockType > 0);
                }
            }

            return boolMap;
        }
    }
}