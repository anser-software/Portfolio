using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GenX2D
{

    public static class Dungeon2D
    {

        /// <summary>
        /// Generates a dungeon and returns an array of roooms in it.
        /// </summary>
        /// <param name="_minRoomSize"></param>
        /// <param name="_maxRoomSize"></param>
        /// <param name="_minRoomCount"></param>
        /// <param name="_maxRoomCount"></param>
        /// <param name="_roomFullness">A value from 0 to 100.</param>
        /// <param name="_valueOffset">First generation parameter.</param>
        /// <param name="_valueDifference">Second generation parameter.</param>
        /// <param name="_roomSmoothness"></param>
        /// <param name="seed">Generation seed.</param>
        /// <returns>An array of roooms in it.</returns>
        public static Room2D[] GenerateDungeonRooms(int _minRoomSize, int _maxRoomSize, int _minRoomCount, int _maxRoomCount,
            int _roomFullness, int _valueOffset, int _valueDifference, int _roomSmoothness, string _seed = null)
        {
            if (_seed == null || _seed.Trim().Length < 1) _seed = Random.Range(0, int.MaxValue).ToString();

            System.Random random = new System.Random(_seed.GetHashCode());

            int roomCount = random.Next(_minRoomCount, _maxRoomCount + 1);

            //Set actual generation parameters according to input values
            int biThreshold = _valueOffset;
            int deThreshold = Mathf.Clamp(_valueOffset + _valueDifference, 0, 8);

            List<Room2D> allRooms = new List<Room2D>();

            Random.InitState(_seed.GetHashCode());

            for (int i = 0; i < roomCount; i++)
            {
                int size = random.Next(_minRoomSize, _maxRoomSize + 1);

                Vector2 position = Vector2.one * (_maxRoomSize + MapManager2D.mapManager.maxDistanceBetweenRooms);

                int index = 0;

                if (allRooms.Count > 0)
                {
                    do
                    {
                        //Position the new room randomly, but not too close to any other room
                        position += (Random.insideUnitCircle * MapManager2D.mapManager.maxDistanceBetweenRooms) + (Vector2.one * MapManager2D.mapManager.maxDistanceBetweenRooms);

                        index++;
                    } while ((allRooms.Any(p => Vector2.Distance(p.position, position) < MapManager2D.mapManager.minDistanceBetweenRooms)) && index < 200);
                }

                Room2D room = new Room2D();

                room.position = position;

                //Generate this room
                room.map = CellularAutomata.Generate(size, size, deThreshold, biThreshold, _roomFullness, _roomSmoothness, 0, 0, _seed: _seed);

                room.presenceMap = new bool[room.map.GetLength(0), room.map.GetLength(1)];
                bool[,] newMap = new bool[room.map.GetLength(0), room.map.GetLength(1)];

                bool[,] largestCave = CellularAutomata.LargestCaveInArea(room.map);


                for (int x = 0; x < newMap.GetLength(0); x++)
                {
                    for (int y = 0; y < newMap.GetLength(1); y++)
                    {
                        if (largestCave[x, y])
                            room.presenceMap[x, y] = true;

                        int neighbours = CellularAutomata.GetNeighbourCount(x, y, room.map);

                        if (neighbours < MapManager2D.mapManager.borderThickness && neighbours > 0)
                            newMap[x, y] = true;
                    }
                }

                room.map = newMap;

                allRooms.Add(room);
            }

            int index2 = 0;

            //Move every room that is too close to another room away from it, and move every room that is too far away closer.
            foreach (Room2D roomA in allRooms)
            {
                while ((allRooms.Any(p => Vector2.Distance(p.position, roomA.position) < MapManager2D.mapManager.minDistanceBetweenRooms)) && index2 < 500)
                {
                    foreach (Room2D roomB in allRooms.Where(p => Vector2.Distance(p.position, roomA.position) < MapManager2D.mapManager.minDistanceBetweenRooms))
                    {
                        roomA.position -= (roomB.position - roomA.position).normalized;
                    }
                    index2++;
                }

                while (allRooms.Where(p => Vector2.Distance(p.position, roomA.position) < MapManager2D.mapManager.maxDistanceBetweenRooms).Count() < 2 && index2 < 1500)
                {

                    roomA.position -= (roomA.position - (Vector2.one * (_maxRoomSize + MapManager2D.mapManager.maxDistanceBetweenRooms))).normalized;

                    foreach (Room2D roomB in allRooms.Where(p => Vector2.Distance(p.position, roomA.position) > MapManager2D.mapManager.maxDistanceBetweenRooms))
                    {
                        roomA.position -= (roomA.position - roomB.position).normalized;
                    }
                    index2++;
                }
            }

            return allRooms.ToArray();
        }



        /// <summary>
        /// Generates a dungeon and returns the whole map of it.
        /// </summary>
        /// <param name="_minRoomSize"></param>
        /// <param name="_maxRoomSize"></param>
        /// <param name="_minRoomCount"></param>
        /// <param name="_maxRoomCount"></param>
        /// <param name="_roomFullness">A value from 0 to 100</param>
        /// <param name="_valueOffset">First generation parameter</param>
        /// <param name="_valueDifference">Second generation parameter</param>
        /// <param name="_roomSmoothness"></param>
        /// <param name="seed"></param>
        /// <returns>Map of the generated dungeon.</returns>
        public static bool[,] GenerateDungeon(int _minRoomSize, int _maxRoomSize, int _minRoomCount, int _maxRoomCount,
            int _roomFullness, int _valueOffset, int _valueDifference, int _roomSmoothness, int _minDistanceBetweenRooms, int _maxDistanceBetweenRooms,  string _seed = null)
        {
            if (_seed == null || _seed.Trim().Length < 1) _seed = Random.Range(0, int.MaxValue).ToString();

            System.Random random = new System.Random(_seed.GetHashCode());

            int roomCount = random.Next(_minRoomCount, _maxRoomCount + 1);

            //Set actual generation parameters according to input values
            int biThreshold = _valueOffset;
            int deThreshold = Mathf.Clamp(_valueOffset + _valueDifference, 0, 8);

            List<Room2D> allRooms = new List<Room2D>();

            Random.InitState(_seed.GetHashCode());

            for (int i = 0; i < roomCount; i++)
            {
                int size = random.Next(_minRoomSize, _maxRoomSize + 1);

                Vector2 position = Vector2.one * (_maxRoomSize + _maxDistanceBetweenRooms);

                int index = 0;

                if (allRooms.Count > 0)
                {
                    //Position the new room randomly, but not too close to any other room
                    do
                    {
                        position += (Random.insideUnitCircle * _maxDistanceBetweenRooms) + (Vector2.one * _maxDistanceBetweenRooms);

                        index++;
                    } while ((allRooms.Any(p => Vector2.Distance(p.position, position) < _minDistanceBetweenRooms)) && index < 200);
                }

                Room2D room = new Room2D();

                room.position = position;

                //Generate this room
                room.map = CellularAutomata.Generate(size, size, deThreshold, biThreshold, _roomFullness, _roomSmoothness, 0, 0, _seed: _seed);

                room.presenceMap = new bool[room.map.GetLength(0), room.map.GetLength(1)];
                bool[,] newMap = new bool[room.map.GetLength(0), room.map.GetLength(1)];

                bool[,] largestCave = CellularAutomata.LargestCaveInArea(room.map);


                for (int x = 0; x < newMap.GetLength(0); x++)
                {
                    for (int y = 0; y < newMap.GetLength(1); y++)
                    {
                        if (largestCave[x, y])
                            room.presenceMap[x, y] = true;

                        int neighbours = CellularAutomata.GetNeighbourCount(x, y, room.map);

                        if (neighbours < MapManager2D.mapManager.borderThickness && neighbours > 0)
                            newMap[x, y] = true;
                    }
                }

                room.map = newMap;

                allRooms.Add(room);
            }

            int index2 = 0;

            //Move every room that is too close to another room away from it, and move every room that is too far away closer.
            foreach (Room2D roomA in allRooms)
            {
                while ((allRooms.Any(p => Vector2.Distance(p.position, roomA.position) < _minDistanceBetweenRooms)) && index2 < 500)
                {
                    foreach (Room2D roomB in allRooms.Where(p => Vector2.Distance(p.position, roomA.position) < _minDistanceBetweenRooms))
                    {
                        roomA.position -= (roomB.position - roomA.position).normalized;
                    }
                    index2++;
                }

                while (allRooms.Where(p => Vector2.Distance(p.position, roomA.position) < _maxDistanceBetweenRooms).Count() < 2 && index2 < 1500)
                {

                    roomA.position -= (roomA.position - (Vector2.one * (_maxRoomSize + _maxDistanceBetweenRooms))).normalized;

                    foreach (Room2D roomB in allRooms.Where(p => Vector2.Distance(p.position, roomA.position) > _maxDistanceBetweenRooms))
                    {
                        roomA.position -= (roomA.position - roomB.position).normalized;

                    }
                    index2++;
                }
            }

            Vector2 bottL = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            Vector2 topR = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            //Find the bottom left and top right corners of the dungeon
            for (int i = 0; i < allRooms.Count; i++)
            {
                if (allRooms[i].position.x < bottL.x) bottL.x = allRooms[i].position.x;
                if (allRooms[i].position.y < bottL.y) bottL.y = allRooms[i].position.y;

                if (allRooms[i].position.x > topR.x) topR.x = allRooms[i].position.x;
                if (allRooms[i].position.y > topR.y) topR.y = allRooms[i].position.y;
            }

            //Increase the top right corner by the max size a room can have
            topR += Vector2.one * _maxRoomSize;

            bool[,] dungeonMap = new bool[(int)topR.x, (int)topR.y];

            bool[,] globalPresenceMap = new bool[(int)topR.x, (int)topR.y];

            for (int i = 0; i < allRooms.Count; i++)
            {
                for (int x = 0; x < allRooms[i].map.GetLength(0); x++)
                {
                    for (int y = 0; y < allRooms[i].map.GetLength(1); y++)
                    {
                        if (CellularAutomata.IsOutOfMap((int)(x + allRooms[i].position.x), (int)(y + allRooms[i].position.y), dungeonMap))
                        {
                            Debug.LogError((int)(x + allRooms[i].position.x) + " " + (int)(y + allRooms[i].position.y) + ": Out of map at " + i);
                            return dungeonMap;
                        }

                        dungeonMap[(int)(x + allRooms[i].position.x), (int)(y + allRooms[i].position.y)] = allRooms[i].map[x, y];
                    }
                }

                for (int x = 0; x < allRooms[i].presenceMap.GetLength(0); x++)
                {
                    for (int y = 0; y < allRooms[i].presenceMap.GetLength(1); y++)
                    {
                        globalPresenceMap[(int)(x + allRooms[i].position.x), (int)(y + allRooms[i].position.y)] = allRooms[i].presenceMap[x, y];
                    }
                }
            }

            Vert[] initVerts = new Vert[allRooms.Count];

            for (int i = 0; i < initVerts.Length; i++)
            {
                initVerts[i] = new Vert(allRooms[i].position);
            }

            List<Vert> outVerts = new List<Vert>();

            initVerts[0].treeIndex = 0;
            for (int i = 0; i < initVerts.Length; i++)
            {
                Vert[] localAll = initVerts.Where(v => (v.connectedVert == null || v.connectedVert.position != initVerts[i].position) && v.position != initVerts[i].position && v.treeIndex != initVerts[i].treeIndex).OrderBy(v => Vector2.Distance(v.position, initVerts[i].position)).ToArray();

                if (localAll.Length > 0)
                {
                    initVerts[i].connectedVert = localAll[0];

                    outVerts.Add(initVerts[i]);
                    initVerts[i].weight = (int)Vector2.Distance(initVerts[i].position, initVerts[i].connectedVert.position);
                    initVerts[i].treeIndex = 0;
                }
            }

            initVerts = initVerts.OrderBy(v => v.weight).ToArray();

            //Connect every room
            for (int i = 0; i < outVerts.Count; i++)
            {
                List<Vector2> points = new List<Vector2>();

                points = GetLine((int)outVerts[i].position.x, (int)outVerts[i].position.y, (int)outVerts[i].connectedVert.position.x, (int)outVerts[i].connectedVert.position.y);

                if (points.Count < 1) continue;

                allRooms[i].points = points;

                foreach (Vector2 point in points)
                {
                    int pX = (int)point.x + (_minRoomSize / 2);
                    int pY = (int)point.y + (_minRoomSize / 2);

                    for (int x = -MapManager2D.mapManager.passageBorderThickness; x <= MapManager2D.mapManager.passageBorderThickness; x++)
                    {
                        for (int y = -MapManager2D.mapManager.passageBorderThickness; y <= MapManager2D.mapManager.passageBorderThickness; y++)
                        {
                            if (!globalPresenceMap[pX + x, pY + y])
                                dungeonMap[pX + x, pY + y] = true;
                        }
                    }
                }
                foreach (Vector2 point in points)
                {
                    int pX = (int)point.x + (_minRoomSize / 2);
                    int pY = (int)point.y + (_minRoomSize / 2);

                    for (int x = -MapManager2D.mapManager.passageWidth; x <= MapManager2D.mapManager.passageWidth; x++)
                    {
                        for (int y = -MapManager2D.mapManager.passageWidth; y <= MapManager2D.mapManager.passageWidth; y++)
                        {
                            dungeonMap[pX + x, pY + y] = false;
                        }
                    }
                }
            }

            for (int i = 0; i < allRooms.Count; i++)
            {
                foreach (Vector2 point in allRooms[i].points)
                {
                    int pX = (int)point.x + (_minRoomSize / 2);
                    int pY = (int)point.y + (_minRoomSize / 2);

                    for (int x = -MapManager2D.mapManager.passageWidth; x <= MapManager2D.mapManager.passageWidth; x++)
                    {
                        for (int y = -MapManager2D.mapManager.passageWidth; y <= MapManager2D.mapManager.passageWidth; y++)
                        {
                            dungeonMap[pX + x, pY + y] = false;
                        }
                    }
                }
            }

            return dungeonMap;
        }

        public static void GenerateSquareDungeon(int _minRoomCount, int _maxRoomCount, int _minRoomWidth, int _maxRoomWidth, int _minRoomHeight, int _maxRoomHeight,
            int _minDistanceBetweenRooms, int _maxDistanceBetweenRooms, float distanceMultiplier, int borderThickness)
        {

            int targetRoomCount = Random.Range(_minRoomCount, _maxRoomCount + 1);

            List<SquareRoom2D> allRooms = new List<SquareRoom2D>();

            SquareRoom2D initRoom = new SquareRoom2D(Random.Range(_minRoomWidth, _maxRoomWidth + 1), 
                Random.Range(_minRoomHeight, _maxRoomHeight + 1), Vector2.zero, new Cell(targetRoomCount/2, targetRoomCount/2));

            allRooms.Add(initRoom);

            bool[,] globalMap = new bool[targetRoomCount + 1, targetRoomCount + 1];

            for (int i = 0; i<200; i++)
            {
                if (allRooms.Count >= targetRoomCount)
                    break;

                int sideIndex = Random.Range(0, 4);
                int targetRoomIndex = Random.Range(0, allRooms.Count);

                SquareRoom2D targetRoom = allRooms[targetRoomIndex];

                switch (sideIndex)
                {
                    case 0:
                        if (targetRoom.Down == null)
                        {
                            Cell globalPos = new Cell(targetRoom.globalPos.x, targetRoom.globalPos.y - 1);
                            if (globalMap[globalPos.x, globalPos.y])
                                break;

                            SquareRoom2D newRoom = new SquareRoom2D(Random.Range(_minRoomWidth, _maxRoomWidth + 1),
                    Random.Range(_minRoomHeight, _maxRoomHeight + 1), new Vector2(targetRoom.position.x, targetRoom.position.y - targetRoom.height * distanceMultiplier), globalPos);
                            allRooms[targetRoomIndex].Down = newRoom;
                            allRooms.Add(newRoom);
                            globalMap[globalPos.x, globalPos.y] = true;
                        }
                        break;
                    case 1:
                        if (targetRoom.Left == null)
                        {
                            Cell globalPos = new Cell(targetRoom.globalPos.x-1, targetRoom.globalPos.y);
                            if (globalMap[globalPos.x, globalPos.y])
                                break;

                            SquareRoom2D newRoom = new SquareRoom2D(Random.Range(_minRoomWidth, _maxRoomWidth + 1),
                Random.Range(_minRoomHeight, _maxRoomHeight + 1), new Vector2(targetRoom.position.x - targetRoom.width * distanceMultiplier, targetRoom.position.y), globalPos);
                            allRooms[targetRoomIndex].Left = newRoom;
                            allRooms.Add(newRoom);
                            globalMap[globalPos.x, globalPos.y] = true;
                        }
                        break;
                    case 2:
                        if (targetRoom.Up == null)
                        {
                            Cell globalPos = new Cell(targetRoom.globalPos.x, targetRoom.globalPos.y + 1);
                            if (globalMap[globalPos.x, globalPos.y])
                                break;

                            SquareRoom2D newRoom = new SquareRoom2D(Random.Range(_minRoomWidth, _maxRoomWidth + 1),
                Random.Range(_minRoomHeight, _maxRoomHeight + 1), new Vector2(targetRoom.position.x, targetRoom.position.y + targetRoom.height * distanceMultiplier), globalPos);
                            allRooms[targetRoomIndex].Up = newRoom;
                            allRooms.Add(newRoom);
                            globalMap[globalPos.x, globalPos.y] = true;
                        }
                        break;
                    case 3:
                        if (targetRoom.Right == null)
                        {
                            Cell globalPos = new Cell(targetRoom.globalPos.x+1, targetRoom.globalPos.y);
                            if (globalMap[globalPos.x, globalPos.y])
                                break;

                            SquareRoom2D newRoom = new SquareRoom2D(Random.Range(_minRoomWidth, _maxRoomWidth + 1),
                Random.Range(_minRoomHeight, _maxRoomHeight + 1), new Vector2(targetRoom.position.x + targetRoom.width * distanceMultiplier, targetRoom.position.y), globalPos);
                            allRooms[targetRoomIndex].Right = newRoom;
                            allRooms.Add(newRoom);
                            globalMap[globalPos.x, globalPos.y] = true;
                        }
                        break;
                }
            }

            //*********MOVE NEXT TO MAP MANAGER************

            Transform dungeon = new GameObject("Dungeon").transform;

            Material dungeonMat = Object.Instantiate(MapManager2D.mapManager.meshMaterial);

            foreach (SquareRoom2D room in allRooms)
            {
                int width = room.width;
                int height = room.height;

                bool[,] currentRoom = new bool[width+2, height+2];


                for (int x = 1; x < width+1; x++)
                {
                    for (int y = 1; y < height+1; y++)
                    {
                        if (x <= borderThickness || x >= width-(borderThickness-1) || y <= borderThickness || y >= height - (borderThickness - 1))
                            currentRoom[x, y] = true;
                    }
                }

                if (room.Down != null)
                {
                    currentRoom[width / 2, 1] = false;
                    Debug.DrawLine(room.position, room.Down.position, Color.blue, 1000F);
                }
                if (room.Left != null)
                {
                    currentRoom[1, height / 2] = false;
                    Debug.DrawLine(room.position, room.Left.position, Color.blue, 1000F);
                }
                if (room.Up != null)
                {
                    currentRoom[width / 2, height] = false;
                    Debug.DrawLine(room.position, room.Up.position, Color.blue, 1000F);
                }
                if (room.Right != null)
                {
                    currentRoom[width, height/2] = false;
                    Debug.DrawLine(room.position, room.Right.position, Color.blue, 1000F);
                }

                

                

                GameObject g = new GameObject("Room");

                g.AddComponent<MeshFilter>().mesh = MeshMaker.MakeMesh(currentRoom, 1F, false);
                MeshRenderer meshRenderer = g.AddComponent<MeshRenderer>();
                meshRenderer.material = dungeonMat;
                meshRenderer.material.mainTexture = MeshTextureGenerator.GenerateTexture(MapManager2D.BoolToBlockMap(currentRoom),
                    MapManager2D.mapManager.allBlockTypes[0], 1);
                g.AddComponent<BoxCollider2D>();

                g.transform.position = new Vector3(room.position.x-(width/2), room.position.y-(height/2));

                g.transform.parent = dungeon;

                if (Physics2D.OverlapAreaAll(g.transform.position, g.transform.position+new Vector3(width,height)).Length > 1)
                    GameObject.Destroy(g);
            }

            
        }

        /// <summary>
        /// Get a line between point x1,y1 and point x2,y2
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Line between point x1,y1 and point x2,y2</returns>
        public static List<Vector2> GetLine(int x1, int y1, int x2, int y2)
        {
            List<Vector2> points = new List<Vector2>();

            int dy = y2 - y1;
            int dx = x2 - x1;
            int stepx, stepy;

            if (dy < 0) { dy = -dy; stepy = -1; } else { stepy = 1; }
            if (dx < 0) { dx = -dx; stepx = -1; } else { stepx = 1; }
            dy <<= 1;        // dy is now 2*dy
            dx <<= 1;        // dx is now 2*dx

            points.Add(new Vector2(x1, y1));

            if (dx > dy)
            {
                int fraction = dy - (dx >> 1);  // same as 2*dy - dx
                while (x1 != x2)
                {
                    if (fraction >= 0)
                    {
                        y1 += stepy;
                        fraction -= dx;          // same as fraction -= 2*dx
                    }
                    x1 += stepx;
                    fraction += dy;              // same as fraction -= 2*dy
                    points.Add(new Vector2(x1, y1));
                }
            }
            else
            {
                int fraction = dx - (dy >> 1);
                while (y1 != y2)
                {
                    if (fraction >= 0)
                    {
                        x1 += stepx;
                        fraction -= dy;
                    }
                    y1 += stepy;
                    fraction += dx;
                    points.Add(new Vector2(x1, y1));
                }
            }

            return points;
        }
    }

    public class SquareRoom2D
    {
        public SquareRoom2D Left = null;
        public SquareRoom2D Right = null;
        public SquareRoom2D Up = null;
        public SquareRoom2D Down = null;

        public int width;
        public int height;
        public Vector2 position;

        public Cell globalPos;

        public bool[,] map;

        public SquareRoom2D (int _width, int _height, Vector2 _position, Cell _globalPos)
        {
            width = _width;
            height = _height;
            position = _position;
            globalPos = _globalPos;
        }
    }

    public class Room2D
    {
        /// <summary>
        /// Position of the room
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// Map of the room
        /// </summary>
        public bool[,] map;

        /// <summary>
        /// Map of dead(empty) cells
        /// </summary>
        public bool[,] presenceMap;

        /// <summary>
        /// List of passageway points in a line from this room to the connected room.
        /// </summary>
        public List<Vector2> points = new List<Vector2>();
    }

    internal class Vert
    {
        public Vector2 position;
        public int treeIndex = -1;

        public Vert connectedVert = null;

        public int weight;

        public Vert(Vector2 pos)
        {
            position = pos;
        }

    }
}