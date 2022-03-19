using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MeshMakerBlocks {

    /// <summary>
    /// Generate a single mesh from the input map.
    /// </summary>
    /// <param name="map">Input map.</param>
    /// <returns>Generated mesh.</returns>
    public static Mesh GenerateMesh(bool[,,] map)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Mesh outputMesh = new Mesh();

        int xLength = map.GetLength(0);
        int yLength = map.GetLength(1);
        int zLength = map.GetLength(2);

        //Create vertices and triangles according to the input map
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    if (!map[x, y, z])
                        continue;


                    if (y == yLength - 1 || !map[x, y + 1, z])
                    {
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                    }

                    if (y == 0 || !map[x, y - 1, z])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));

                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 3);

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);
                    }

                    if (x == 0 || !map[x - 1, y, z])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));

                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                    }

                    if (x == xLength - 1 || !map[x + 1, y, z])
                    {
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 3);
                    }

                    if (z == zLength - 1 || !map[x, y, z + 1])
                    {
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);

                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);

                    }

                    if (z == 0 || !map[x, y, z - 1])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));


                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 4);

                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);
                    }
                }
            }
        }

        outputMesh.vertices = vertices.ToArray();
        outputMesh.triangles = triangles.ToArray();

        outputMesh.RecalculateNormals();
        outputMesh.RecalculateBounds();

        return outputMesh;
    }

    /// <summary>
    /// Generate a single mesh from the input map with multiple submeshes.
    /// </summary>
    /// <param name="map">Input map.</param>
    /// <returns>Generated mesh.</returns>
    public static Mesh GenerateMesh(Voxel[,,] map, int blockTypeNumber)
    {
        List<Vector3> vertices = new List<Vector3>();

        List<int>[] triangles = new List<int>[blockTypeNumber];
        for (int i = 0; i < blockTypeNumber; i++)
        {
            triangles[i] = new List<int>();
        }

        Mesh outputMesh = new Mesh();

        int xLength = map.GetLength(0);
        int yLength = map.GetLength(1);
        int zLength = map.GetLength(2);

        //Create vertices and triangles according to the input map
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    if (map[x, y, z].TypeID == 0)
                        continue;

                    int type = map[x, y, z].TypeID - 1;

                    if (y == yLength - 1 || map[x, y + 1, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);

                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 2);
                    }

                    if (y == 0 || map[x, y - 1, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));

                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 3);

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);
                    }

                    if (x == 0 || map[x - 1, y, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));

                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 2);

                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                    }

                    if (x == xLength - 1 || map[x + 1, y, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 3);
                    }

                    if (z == zLength - 1 || map[x, y, z + 1].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);

                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 1);

                    }

                    if (z == 0 || map[x, y, z - 1].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));


                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 4);

                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);
                    }
                }
            }
        }

        outputMesh.vertices = vertices.ToArray();

        outputMesh.subMeshCount = blockTypeNumber;

        for (int i = 0; i < blockTypeNumber; i++)
        {
            outputMesh.SetTriangles(triangles[i], i);
        }

        outputMesh.RecalculateNormals();
        outputMesh.RecalculateBounds();

        return outputMesh;
    }


    /// <summary>
    /// Generate an array of meshes from the input map.
    /// </summary>
    /// <param name="map">Input map.</param>
    /// <returns>Generated mesh.</returns>
    public static Mesh[] GenerateMeshes(Voxel[,,] map, int blockTypeNumber)
    {
        List<Vector3> vertices = new List<Vector3>();

        List<int>[] triangles = new List<int>[blockTypeNumber];

        for (int i = 0; i < blockTypeNumber; i++)
        {
            triangles[i] = new List<int>();
        }

        List<Mesh> outputMeshes = new List<Mesh>();

        int xLength = map.GetLength(0);
        int yLength = map.GetLength(1);
        int zLength = map.GetLength(2);

        //Create vertices and triangles according to the input map
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    if (map[x, y, z].TypeID == 0)
                        continue;

                    int type = map[x, y, z].TypeID - 1;

                    if (y == yLength - 1 || map[x, y + 1, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);

                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 2);
                    }

                    if (y == 0 || map[x, y - 1, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));

                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 3);

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);
                    }

                    if (x == 0 || map[x - 1, y, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));

                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 2);

                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                    }

                    if (x == xLength - 1 || map[x + 1, y, z].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);

                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 3);
                    }

                    if (z == zLength - 1 || map[x, y, z + 1].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 3);

                        triangles[type].Add(vertices.Count - 4);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 1);

                    }

                    if (z == 0 || map[x, y, z - 1].TypeID == 0)
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));


                        triangles[type].Add(vertices.Count - 3);
                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 4);

                        triangles[type].Add(vertices.Count - 1);
                        triangles[type].Add(vertices.Count - 2);
                        triangles[type].Add(vertices.Count - 4);
                    }

                    if (vertices.Count > 64900)
                    {
                        //Finalize a mesh and add it to the list
                        Mesh newMesh = new Mesh();
                        newMesh.vertices = vertices.ToArray();

                        newMesh.subMeshCount = blockTypeNumber;

                        for (int i = 0; i < blockTypeNumber; i++)
                        {
                            newMesh.SetTriangles(triangles[i], i);
                            triangles[i].Clear();

                        }

                        newMesh.RecalculateNormals();
                        newMesh.RecalculateBounds();

                        outputMeshes.Add(newMesh);

                        vertices.Clear();
                    }
                }
            }
        }

        if (vertices.Count >= 3)
        {
            //Finalize a mesh and add it to the list
            Mesh newMesh = new Mesh();
            newMesh.vertices = vertices.ToArray();
            newMesh.subMeshCount = blockTypeNumber;

            for (int i = 0; i < blockTypeNumber; i++)
            {
                newMesh.SetTriangles(triangles[i], i);
                triangles[i].Clear();

            }

            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();

            outputMeshes.Add(newMesh);

            vertices.Clear();
        }

        return outputMeshes.ToArray();
    }

    /// <summary>
    /// Generate a single mesh from the input map.
    /// </summary>
    /// <param name="map">Input map.</param>
    /// <returns>Generated mesh.</returns>
    public static Mesh[] GenerateMeshes(bool[,,] map)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<Mesh> outputMeshes = new List<Mesh>();

        int xLength = map.GetLength(0);
        int yLength = map.GetLength(1);
        int zLength = map.GetLength(2);

        //Create vertices and triangles according to the input map
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    if (!map[x, y, z])
                        continue;


                    if (y == yLength - 1 || !map[x, y + 1, z])
                    {
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                    }

                    if (y == 0 || !map[x, y - 1, z])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));

                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 3);

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);
                    }

                    if (x == 0 || !map[x - 1, y, z])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));

                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                    }

                    if (x == xLength - 1 || !map[x + 1, y, z])
                    {
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 3);
                    }

                    if (z == zLength - 1 || !map[x, y, z + 1])
                    {
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));


                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);

                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);

                    }

                    if (z == 0 || !map[x, y, z - 1])
                    {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));


                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 4);

                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);
                    }

                    if (vertices.Count > 64900)
                    {
                        Mesh newMesh = new Mesh();
                        newMesh.vertices = vertices.ToArray();
                        newMesh.triangles = triangles.ToArray();

                        newMesh.RecalculateNormals();
                        newMesh.RecalculateBounds();

                        outputMeshes.Add(newMesh);

                        vertices.Clear();
                        triangles.Clear();
                    }
                }
            }
        }

        if (vertices.Count >= 3)
        {
            Mesh newMesh = new Mesh();
            newMesh.vertices = vertices.ToArray();
            newMesh.triangles = triangles.ToArray();

            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();

            outputMeshes.Add(newMesh);

            vertices.Clear();
            triangles.Clear();
        }

        return outputMeshes.ToArray();
    }

    /// <summary>
    /// Generate a single mesh from the input map.
    /// </summary>
    /// <param name="map">Input map.</param>
    /// <returns>Generated mesh.</returns>
    public static Mesh GenerateMesh(Voxel[,,] map)
    {
        return GenerateMesh(map.ToBoolMap());
    }

}
