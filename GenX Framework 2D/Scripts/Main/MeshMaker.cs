using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenX2D
{

    public static class MeshMaker
    {

        static bool isSmooth;

        static SquareGrid sqrGrid;

        static List<Vector3> vertices;
        static List<int> triangles;

        static List<int> destroyedIndices = new List<int>();

        /// <summary>
        /// Last generated mesh.
        /// </summary>
        public static Mesh lastMapMesh;

        /// <summary>
        /// Generate a 2D mesh from the input map.
        /// </summary>
        /// <param name="map">Input map.</param>
        /// <param name="_squareSize">Size of the map</param>
        /// <param name="smooth">Smooth the mesh?</param>
        /// <returns>Generated mesh.</returns>
        public static Mesh MakeMesh(bool[,] map, float squareSize, bool smooth)
        {
            isSmooth = smooth;

            vertices = new List<Vector3>();
            triangles = new List<int>();

            destroyedIndices = new List<int>();

            if (smooth)
            {
                sqrGrid = new SquareGrid(map, squareSize);
                for (int x = 1; x < sqrGrid.grid.GetLength(0) - 1; x++)
                {
                    for (int y = 1; y < sqrGrid.grid.GetLength(1) - 1; y++)
                    {
                        TriangulateSmooth(sqrGrid.grid[x, y]);
                    }
                }
            }
            else
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        if (!map[x, y]) continue;
                        vertices.Add(new Vector3(x * squareSize, y * squareSize));
                        vertices.Add(new Vector3(x * squareSize, (y + 1) * squareSize));
                        vertices.Add(new Vector3((x + 1) * squareSize, y * squareSize));

                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 3);

                        vertices.Add(new Vector3((x + 1) * squareSize, (y + 1) * squareSize));

                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                    }
                }
            }

            lastMapMesh = new Mesh();

            lastMapMesh.vertices = vertices.ToArray();
            lastMapMesh.triangles = triangles.ToArray();

            lastMapMesh.RecalculateNormals();

            Vector2[] UVs = new Vector2[vertices.Count];

            for (int i = 0; i < UVs.Length; i++)
            {
                UVs[i] = new Vector2(vertices[i].x / map.GetLength(0), vertices[i].y / map.GetLength(1)) / squareSize;
            }

            lastMapMesh.uv = UVs;

            return lastMapMesh;
        }


        /// <summary>
        /// Generate a 2D mesh from the input peaks.
        /// </summary>
        /// <param name="smoothedPeaks">Input peaks.</param>
        /// <param name="size">Size of the mesh.</param>       
        /// <param name="yDepth">Y Depth of the mesh.</param>
        /// <returns>Generated mesh.</returns>
        public static Mesh MakeMesh(Vector2[] smoothedPeaks, float size, int yDepth)
        {
            yDepth = (yDepth < 1) ? 1 : yDepth;

            vertices = new List<Vector3>();
            triangles = new List<int>();

            destroyedIndices = new List<int>();

            for (int i = 0; i < smoothedPeaks.Length; i++)
            {

                vertices.Add((smoothedPeaks[i] + (Vector2.down* yDepth)) * size);
                vertices.Add((smoothedPeaks[i]) * size);

                Vector2 nextPos = (i < smoothedPeaks.Length - 1) ? smoothedPeaks[i + 1] : smoothedPeaks[i];

                vertices.Add((nextPos) * size);

                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);

                vertices.Add((nextPos + (Vector2.down * yDepth)) * size);

                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            
            }

            lastMapMesh = new Mesh();

            lastMapMesh.vertices = vertices.ToArray();
            lastMapMesh.triangles = triangles.ToArray();

            lastMapMesh.RecalculateNormals();
            lastMapMesh.RecalculateBounds();

            Vector2[] UVs = new Vector2[vertices.Count];

            for (int i = 0; i < UVs.Length; i++)
            {
                UVs[i] = new Vector2(vertices[i].x / smoothedPeaks.GetLength(0), vertices[i].y / lastMapMesh.bounds.max.y) / size;
            }

            lastMapMesh.uv = UVs;

            return lastMapMesh;
        }


        /// <summary>
        /// Generate a 2D mesh from the input map.
        /// </summary>
        /// <param name="map">Input map.</param>
        /// <param name="_squareSize">Size of the map</param>
        /// <param name="smooth">Smooth the mesh?</param>
        /// <returns>Generated mesh.</returns>
        public static Mesh MakeMesh(Block2D[,] map, float _squareSize, bool smooth)
        {
            bool[,] boolMap = MapManager2D.BlockToBoolMap(map);

            return MakeMesh(boolMap, _squareSize, smooth);
        }


        /// <summary>
        /// Make a plane below peaks.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="_squareSize"></param>
        /// <param name="peaks"></param>
        /// <returns>Generated mesh.</returns>
        public static Mesh MakePlane(int width, int height, float _squareSize, int[] peaks)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();

            destroyedIndices = new List<int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y > peaks[x] - 2) continue;

                    vertices.Add(new Vector3(x * _squareSize, y * _squareSize));
                    vertices.Add(new Vector3(x * _squareSize, (y + 1) * _squareSize));
                    vertices.Add(new Vector3((x + 1) * _squareSize, y * _squareSize));

                    triangles.Add(vertices.Count - 2);
                    triangles.Add(vertices.Count - 1);
                    triangles.Add(vertices.Count - 3);

                    vertices.Add(new Vector3((x + 1) * _squareSize, (y + 1) * _squareSize));

                    triangles.Add(vertices.Count - 3);
                    triangles.Add(vertices.Count - 1);
                    triangles.Add(vertices.Count - 2);
                }
            }

            lastMapMesh = new Mesh();

            lastMapMesh.vertices = vertices.ToArray();
            lastMapMesh.triangles = triangles.ToArray();

            lastMapMesh.RecalculateNormals();


            Vector2[] UVs = new Vector2[vertices.Count];

            for (int i = 0; i < UVs.Length; i++)
            {
                UVs[i] = new Vector2(vertices[i].x / width, vertices[i].y / height) / _squareSize;
            }

            lastMapMesh.uv = UVs;

            return lastMapMesh;
        }
        
        static List<Vector2> Points;
        static HashSet<Vector2> path;

        /// <summary>
        /// Make a collider from the input map.
        /// </summary>
        /// <param name="map">Input map.</param>
        /// <param name="parent">Parent transform.</param>
        public static void MakeCollider(bool[,] map, Transform parent = null)
        {
            Points = new List<Vector2>();
            path = new HashSet<Vector2>();

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] && (x == 0 || y == 0 || x == map.GetLength(0) - 1 || y == map.GetLength(1) - 1))
                    {
                        Points.Add(new Vector2(x + 0.5F, y + 0.5F));
                        continue;
                    }

                    if (map[x, y] && !HasLinearNeighbours(x, y, map))
                        Points.Add(new Vector2(x + 0.5F, y + 0.5F));
                }
            }

            int colliderIndex = 0;

            foreach (Vector2 v in Points)
            {
                Transform collider = new GameObject("Collider_" + colliderIndex, new BoxCollider2D().GetType()).transform;
                collider.parent = parent;
                collider.localPosition = v;

                colliderIndex++;
            }
        }

        static void FollowPath(Vector2 point)
        {
            if (!Points.Contains(point) || path.Contains(point) || point.x < 0 || point.y < 0 ||
                point.x >= MapManager2D.currentChunkSizeX || point.y >= MapManager2D.currentChunkSizeY) return;
            path.Add(point);
            FollowPath(point + Vector2.up);
            FollowPath(point + Vector2.right);
            FollowPath(point + Vector2.left);
            FollowPath(point + Vector2.down);

            FollowPath(point + Vector2.left + Vector2.up);
            FollowPath(point + Vector2.right + Vector2.up);
            FollowPath(point + Vector2.left + Vector2.down);
            FollowPath(point + Vector2.right + Vector2.down);

        }

        /// <summary>
        /// Make a collider from the input map.
        /// </summary>
        /// <param name="blockMap">Input map.</param>
        /// <param name="parent">Parent transform.</param>
        public static void MakeCollider(Block2D[,] blockMap, Transform parent = null)
        {
            MakeCollider(MapManager2D.BlockToBoolMap(blockMap), parent);
        }

        public static bool HasLinearNeighbours(int x, int y, bool[,] map)
        {
            if (x <= 0 || y <= 0 || x >= map.GetLength(0) - 1 || y >= map.GetLength(1) - 1) return false;
            return map[x - 1, y] && map[x, y + 1] && map[x + 1, y] && map[x, y - 1];
        }

        /// <summary>
        /// Destroy a face on the chunk or last map.
        /// </summary>
        /// <param name="xPos">X position of the face.</param>
        /// <param name="yPos">Y position of the face.</param>
        /// <param name="inputChunk"></param>
        /// <returns>Updated mesh.</returns>
        public static Mesh DestroyFace(int xPos, int yPos, Chunk2D inputChunk = null)
        {

            //********If modifying chunk************

            if (inputChunk != null)
            {
                List<Vector3> verticesChunk = new List<Vector3>(inputChunk.thisChunkMesh.vertices);
                List<int> trianglesChunk = new List<int>(inputChunk.thisChunkMesh.triangles);

                Vector3 v = new Vector3(xPos + 1, yPos);

                int vertexInd = 0;

                for (int i = 0; i < verticesChunk.Count; i++)
                {
                    if (verticesChunk[i] == v)
                    {
                        vertexInd = i + 2;
                        break;
                    }
                }

                if (vertexInd == -1)
                {
                    if (DemoManager.showWarnings)
                        Debug.LogWarning("A face you are trying to destroy doesn't exist.");
                    return inputChunk.thisChunkMesh;
                }
                if (inputChunk.destroyedIndices.Contains(vertexInd))
                {
                    if (DemoManager.showWarnings)
                        Debug.LogWarning("A face you are trying to destroy has already been destroyed.");
                    return inputChunk.thisChunkMesh;
                }

                inputChunk.destroyedIndices.Add(vertexInd);

                for (int i = 0; i < trianglesChunk.Count; i++)
                {
                    if (trianglesChunk[i] == vertexInd)
                    {
                        trianglesChunk.RemoveRange(i, 6);
                        break;
                    }
                }

                inputChunk.thisChunkMesh.triangles = trianglesChunk.ToArray();

                return inputChunk.thisChunkMesh;
            }

            //********If NOT modifying chunk************

            if (!lastMapMesh)
            {
                Debug.LogError("Mesh of a map doesn't exist.");
                return null;
            }

            if (isSmooth)
            {
                if (DemoManager.showWarnings)
                    Debug.LogWarning("Smooth mesh editing is not available yet.");
                return lastMapMesh;
            }

            int vertexIndex = vertices.IndexOf(new Vector3(xPos, yPos));

            if (vertexIndex == -1)
            {
                if (DemoManager.showWarnings)
                    Debug.LogWarning("A face you are trying to destroy doesn't exist.");
                return lastMapMesh;
            }
            if (destroyedIndices.Contains(vertexIndex))
            {
                if (DemoManager.showWarnings)
                    Debug.LogWarning("A face you are trying to destroy has already been destroyed.");
                return lastMapMesh;
            }

            destroyedIndices.Add(vertexIndex);

            for (int i = 0; i < triangles.Count; i++)
            {
                if (triangles[i] == vertexIndex)
                {
                    triangles.RemoveRange(i, 6);
                    break;
                }
            }

            lastMapMesh.triangles = triangles.ToArray();

            return lastMapMesh;
        }

        static void TriangulateSharp(Square leftUp, Square rightUp, Square rightDown, Square leftDown)
        {
            if (leftUp.config > 0 && rightUp.config > 0 && leftDown.config > 0 && rightDown.config > 0)
                PointsToMesh(leftUp.centralMiddle, rightUp.centralMiddle, rightDown.centralMiddle, leftDown.centralMiddle);
        }

        static void TriangulateSmooth(Square square)
        {
            switch (square.config)
            {
                case 0:
                    break;

                //1 triangle
                case 1:
                    PointsToMesh(square.centerL, square.centerB, square.bottomL);
                    break;
                case 2:
                    PointsToMesh(square.bottomR, square.centerB, square.centerR);
                    break;
                case 4:
                    PointsToMesh(square.topR, square.centerR, square.centerT);
                    break;
                case 8:
                    PointsToMesh(square.topL, square.centerT, square.centerL);
                    break;

                //2 triangles
                case 3:
                    PointsToMesh(square.centerR, square.bottomR, square.bottomL, square.centerL);
                    break;
                case 6:
                    PointsToMesh(square.centerT, square.topR, square.bottomR, square.centerB);
                    break;
                case 9:
                    PointsToMesh(square.topL, square.centerT, square.centerB, square.bottomL);
                    break;
                case 12:
                    PointsToMesh(square.topL, square.topR, square.centerR, square.centerL);
                    break;
                case 15:
                    PointsToMesh(square.topL, square.topR, square.bottomR, square.bottomL);
                    break;

                //3 triangles
                case 5:
                    PointsToMesh(square.centerT, square.topR, square.centerR, square.centerB, square.bottomL, square.centerL);
                    break;
                case 10:
                    PointsToMesh(square.topL, square.centerT, square.centerR, square.bottomR, square.centerB, square.centerL);
                    break;

                //4 triangles
                case 7:
                    PointsToMesh(square.centerT, square.topR, square.bottomR, square.bottomL, square.centerL);
                    break;
                case 11:
                    PointsToMesh(square.topL, square.centerT, square.centerR, square.bottomR, square.bottomL);
                    break;
                case 13:
                    PointsToMesh(square.topL, square.topR, square.centerR, square.centerB, square.bottomL);
                    break;
                case 14:
                    PointsToMesh(square.topL, square.topR, square.bottomR, square.centerB, square.centerL);
                    break;
            }
        }

        static void PointsToMesh(params Point[] points)
        {
            AssignVertices(points);

            if (points.Length >= 3) CreateTriangle(points[0], points[1], points[2]);
            if (points.Length >= 4) CreateTriangle(points[0], points[2], points[3]);
            if (points.Length >= 5) CreateTriangle(points[0], points[3], points[4]);
            if (points.Length >= 6) CreateTriangle(points[0], points[4], points[5]);
        }

        static void AssignVertices(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].vertexI == -1)
                {
                    points[i].vertexI = points[i].vertexI = vertices.Count;
                    vertices.Add(points[i].position);
                }
            }
        }

        static void CreateTriangle(Point a, Point b, Point c)
        {
            triangles.Add(a.vertexI);
            triangles.Add(b.vertexI);
            triangles.Add(c.vertexI);
        }

    }

    internal class Point
    {
        public Vector2 position;

        public int vertexI = -1;

        public Point(Vector2 pos)
        {
            position = pos;
        }
    }

    internal class ControlPoint : Point
    {
        public bool isActive;
        public Point up, right;

        public ControlPoint(Vector2 pos, bool _isActive, float sqrSize) : base(pos)
        {
            isActive = _isActive;
            up = new Point(pos + Vector2.up * sqrSize * 0.5F);
            right = new Point(pos + Vector2.right * sqrSize * 0.5F);
        }
    }

    internal class Square
    {
        public ControlPoint topL, topR, bottomR, bottomL;

        public int config;

        public Point centerT, centerR, centerB, centerL;

        public Point centralMiddle;

        public Square(ControlPoint _topL, ControlPoint _topR, ControlPoint _bottomR, ControlPoint _bottomL)
        {
            topL = _topL;
            topR = _topR;
            bottomR = _bottomR;
            bottomL = _bottomL;

            centerT = topL.right;
            centerR = bottomR.up;
            centerB = bottomL.right;
            centerL = bottomL.up;

            centralMiddle = new Point(new Vector2(centerT.position.x, centerT.position.y - 0.5F));

            if (topL.isActive) config += 8;
            if (topR.isActive) config += 4;
            if (bottomR.isActive) config += 2;
            if (bottomL.isActive) config += 1;
        }
    }

    internal class SquareGrid
    {
        public Square[,] grid;

        public SquareGrid(bool[,] map, float squareSize)
        {
            int pointCountX = map.GetLength(0);
            int pointCountY = map.GetLength(1);

            ControlPoint[,] ctrlPoints = new ControlPoint[pointCountX, pointCountY];

            for (int x = 0; x < pointCountX; x++)
            {
                for (int y = 0; y < pointCountY; y++)
                {
                    ctrlPoints[x, y] = new ControlPoint(new Vector2(x * squareSize + squareSize / 2F,
                        y * squareSize + squareSize / 2F), map[x, y], squareSize);
                }
            }

            grid = new Square[pointCountX - 1, pointCountY - 1];

            for (int x = 0; x < pointCountX - 1; x++)
            {
                for (int y = 0; y < pointCountY - 1; y++)
                {
                    grid[x, y] = new Square(ctrlPoints[x, y + 1], ctrlPoints[x + 1, y + 1], ctrlPoints[x + 1, y], ctrlPoints[x, y]);
                }
            }
        }
    }
}