using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour {

    #region General
    public int width = 20;
    public int height = 20;
    public int zDepth = 20;

    public static CellularAutomata main { get; set; }
    #endregion

    #region Generation
    public Material material;

    public bool useSeed;
    public string seed;

    /// <summary>
    /// If a living cell has less living neighbours than this value - it dies.
    /// </summary>
    public int deathThreshold = 14;

    /// <summary>
    /// If a dead cell has more living neighbours than this value - it becomes alive.
    /// </summary>
    public int birthThreshold = 12;

    /// <summary>
    /// Fullness of the initial map.
    /// </summary>
    public int chanceToStartAlive = 50;

    /// <summary>
    /// Number of times to repeat the simulation.
    /// </summary>
    public int outputGeneration = 6;

    /// <summary>
    /// Invert the final map?
    /// </summary>
    public bool invert = false;

    /// <summary>
    /// Make the final map empty inside?
    /// </summary>
    public bool makeHollow = false;
    #endregion

    void Awake()
    {
        main = FindObjectOfType<CellularAutomata>();
    }

    /// <summary>
    /// Create a structure GameObject at position with current values.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Created GameObject</returns>
    public GameObject Create(Vector3 position)
    {
        //Generate a map
        bool[,,] map = Generate();

        GameObject result = new GameObject("Cellular Automata");

        if ((map.GetLength(0) * map.GetLength(1) * map.GetLength(2)) * 2 < 65000)
        {
            //If resulting mesh can't have more than 65000 verts
            result.AddComponent<MeshRenderer>().material = material;
            result.AddComponent<MeshFilter>().mesh = MeshMakerBlocks.GenerateMesh(map);
        }
        else
        {
            //If resulting mesh CAN have more than 65000 verts we split it
            Mesh[] meshes = MeshMakerBlocks.GenerateMeshes(map);

            for (int i = 0; i < meshes.Length; i++)
            {
                GameObject currentMesh = new GameObject(i.ToString());

                currentMesh.AddComponent<MeshRenderer>().material = material;
                currentMesh.AddComponent<MeshFilter>().mesh = meshes[i];

                currentMesh.transform.parent = result.transform;
            }
        }

        result.transform.position = position;

        return result;
    }

    /// <summary>
    /// Returns a generated 3D map.
    /// </summary>
    /// <param name="_width">Width of the map</param>
    /// <param name="_height">Height of the map</param>
    /// <param name="_zDepth">Depth of the map</param>
    /// <param name="_deathThreshold">If a living cell has less living neighbours than this value - it dies</param>
    /// <param name="_birthThreshold">If a dead cell has more living neighbours than this value - it becomes alive</param>
    /// <param name="_chanceToStartAlive">Fullness of the initial map</param>
    /// <param name="_outputGeneration">Number of times to repeat the simulation</param>
    /// <param name="_invert">Invert the structure</param>
    /// <param name="_makeHollow">Remove the inside of the structure</param>
    /// <param name="_seed">Generation seed</param>
    /// <returns>Generated 3D map</returns>
    public static bool[,,] Generate(int _width, int _height, int _zDepth, int _deathThreshold, int _birthThreshold, int _chanceToStartAlive, int _outputGeneration, bool _invert = false, bool _makeHollow = false, string _seed = null)
    {
        bool[,,] outputMap = new bool[_width, _height, _zDepth];

        if (_seed == null || _seed.Trim().Length < 1) _seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

        System.Random rand = new System.Random(_seed.GetHashCode());

        //Initialize output map array
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _zDepth; z++)
                {
                    outputMap[x, y, z] = (rand.Next(0, 100) < _chanceToStartAlive);
                }
            }
        }

        //Repeat simulation for a number of times defined by '_outputGeneration'
        for (int i = 0; i < _outputGeneration; i++)
        {
            outputMap = Simulate(outputMap, _deathThreshold, _birthThreshold);
        }

        if (_invert)
        {
            outputMap = outputMap.Invert();

            if (_makeHollow)
                outputMap = outputMap.MakeHollow();
        }

        return outputMap;
    }

    /// <summary>
    /// Returns a 3D map generated using current values.
    /// </summary>
    /// <returns></returns>
    public bool[,,] Generate()
    {
        bool[,,] outputMap = new bool[width, height, zDepth];

        if (!useSeed || seed == null || seed.Trim().Length < 1) seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

        System.Random rand = new System.Random(seed.GetHashCode());

        //Initialize output map array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < zDepth; z++)
                {
                    outputMap[x, y, z] = (rand.Next(0, 100) < chanceToStartAlive);
                }
            }
        }

        //Repeat simulation for a number of times defined by '_outputGeneration'
        for (int i = 0; i < outputGeneration; i++)
        {
            outputMap = Simulate(outputMap, deathThreshold, birthThreshold);
        }

        if (invert)
        {
            //Invert the map
            outputMap = outputMap.Invert();

            if (makeHollow)
                outputMap = outputMap.MakeHollow();
        }

        return outputMap;
    }

    /// <summary>
    /// Returns 'inputMap' simulated 1 more time
    /// </summary>
    /// <param name="inputMap"></param>
    /// <param name="_deathThreshold">If a living cell has less living neighbours than this value - it dies</param>
    /// <param name="_birthThreshold">If a dead cell has more living neighbours than this value - it becomes alive</param>
    /// <returns>'inputMap' simulated 1 more time</returns>
    public static bool[,,] Simulate(bool[,,] inputMap, int _deathThreshold, int _birthThreshold)
    {
        bool[,,] outputMap = new bool[inputMap.GetLength(0), inputMap.GetLength(1), inputMap.GetLength(2)];

        for (int x = 0; x < inputMap.GetLength(0); x++)
        {
            for (int y = 0; y < inputMap.GetLength(1); y++)
            {
                for (int z = 0; z < inputMap.GetLength(2); z++)
                {
                    //Get number of living cells around the cell at x,y,z
                    int thisCellsNeighbourCount = inputMap.GetNeighbourCount(x, y, z);

                    if (inputMap[x, y, z])
                    {
                        //If cell at x,y is alive we check if it has less living neighbours than '_deathThreshold' in order to become dead
                        if (thisCellsNeighbourCount < _deathThreshold) outputMap[x, y, z] = false;
                        //If it doesn't - just record it's value to the output map
                        else outputMap[x, y, z] = true;
                    }
                    else
                    {
                        //If cell at x,y is dead we do the same thing but inverted and compare neighbour count to '_birthThreshold'
                        if (thisCellsNeighbourCount > _birthThreshold) outputMap[x, y, z] = true;
                        else outputMap[x, y, z] = false;
                    }
                }
            }
        }

        return outputMap;
    }

}
