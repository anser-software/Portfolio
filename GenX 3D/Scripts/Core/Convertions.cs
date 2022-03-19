using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Convertions {

    /// <summary>
    /// Convert a bitmap to a voxel map.
    /// </summary>
    /// <param name="boolmap"></param>
    /// <param name="type">Voxel type every cell will have</param>
    /// <returns></returns>
    public static Voxel[,,] ToVoxelMap(this bool[,,] boolmap, byte type)
    {
        int xSize = boolmap.GetLength(0);
        int ySize = boolmap.GetLength(1);
        int zSize = boolmap.GetLength(2);

        Voxel[,,] output = new Voxel[xSize, ySize, zSize];

        byte zero = 0;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    output[x, y, z] = new Voxel(boolmap[x, y, z] ? type : zero);
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Convert a voxel map to a bitmap
    /// </summary>
    /// <param name="voxelmap"></param>
    /// <returns></returns>
    public static bool[,,] ToBoolMap(this Voxel[,,] voxelmap)
    {
        int xSize = voxelmap.GetLength(0);
        int ySize = voxelmap.GetLength(1);
        int zSize = voxelmap.GetLength(2);

        bool[,,] output = new bool[xSize, ySize, zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    output[x, y, z] = voxelmap[x,y,z].TypeID != 0;
                }
            }
        }

        return output;
    }
}
