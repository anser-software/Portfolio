using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationType { Range, Group, Random }

[System.Serializable]
public class BlockType
{
    public string name = "Name";

    public bool hideInEditor = false;

    public Material material;

    //Generation
    public GenerationType genType;

    public int yOffset;
    public bool canHaveCavesInside = true;

    public int minY;
    public int maxY;

    public int groupSize;

    public float spawnChance;

    public List<NoiseLayer> noiseLayers = new List<NoiseLayer>();

    public BlockType()
    {
        maxY = (VoxelTerrain.main) ? VoxelTerrain.main.size * 2 : 25;

        yOffset = 5;

        groupSize = 10;
        spawnChance = 1F;

        noiseLayers.Add(new NoiseLayer { scale = 5, magnitude = 5, exponent = 1.5F });
    }

    public BlockType(BlockType init)
    {
        name = init.name;

        material = init.material;
    }
}
