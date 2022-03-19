using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

    /// <summary>
    /// Returns the number of neighbours of the input cell on the map.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int GetNeighbourCount(this bool[,,] inputMap, int x, int y, int z)
    {
        int neighbourCount = 0;

        for (int w = -1; w <= 1; w++)
        {
            for (int h = -1; h <= 1; h++)
            {
                for (int d = -1; d <= 1; d++)
                {
                    if (w == 0 && h == 0 && d == 0) continue;

                    if (inputMap.IsOutOfMap(x + w, y + h, z + d))
                    {
                        neighbourCount++;
                        continue;
                    }

                    if (inputMap[x + w, y + h, z + d]) neighbourCount++;
                }
            }
        }

        return neighbourCount;
    }

    /// <summary>
    /// Checks if input coordinates are out of the map.
    /// </summary>
    public static bool IsOutOfMap(this bool[,,] inputMap, int x, int y, int z)
    {
        return x < 0 || y < 0 || z < 0 || x >= inputMap.GetLength(0) || y >= inputMap.GetLength(1) || z >= inputMap.GetLength(2);
    }

    /// <summary>
    /// Invert the map.
    /// </summary>
    /// <returns></returns>
    public static bool[,,] Invert(this bool[,,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int z = 0; z < map.GetLength(2); z++)
                {
                    map[x, y, z] = !map[x, y, z];
                }
            }
        }

        return map;
    }

    /// <summary>
    /// Makes the map empty on the inside.
    /// </summary>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static bool[,,] MakeHollow(this bool[,,] map, int threshold = 24)
    {
        bool[,,] newMap = new bool[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
        
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int z = 0; z < map.GetLength(2); z++)
                {
                    if (map.GetNeighbourCount(x, y, z) >= threshold)
                        newMap[x, y, z] = false;
                    else
                        newMap[x, y, z] = map[x, y, z];
                }
            }
        }

        return newMap;
    }

}
