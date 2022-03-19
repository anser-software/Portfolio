using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace GenX2D
{

    public enum GenerationPreset { PolishedCave, BlockyCave, OpenCave, Facility, Maze, CityLayout, Circuit }

    public static class CellularAutomata
    {

        /// <summary>
        /// Returns a generated 2D map
        /// </summary>
        /// <param name="_width">Width of the map</param>
        /// <param name="_height">Height of the map</param>
        /// <param name="_deathThreshold">If a living cell has less living neighbours than this value - it dies</param>
        /// <param name="_birthThreshold">If a dead cell has more living neighbours than this value - it becomes alive</param>
        /// <param name="_chanceToStartAlive">Fullness of the initial map</param>
        /// <param name="_outputGeneration">Number of times to repeat the simulation</param>
        /// <param name="_caveAliveThreshold">If a cave has less dead cells inside than this value - it will be filled</param>
        /// <param name="_islandAliveThreshold">If an island has less living cells inside than this value - it will be destroyed</param>
        /// <param name="_seed">Generation seed</param>
        /// <returns>Generated 2D map</returns>
        public static bool[,] Generate(int _width, int _height, int _deathThreshold, int _birthThreshold, int _chanceToStartAlive, int _outputGeneration, int _caveAliveThreshold, int _islandAliveThreshold, string _seed = null)
        {
            bool[,] outputMap = new bool[_width, _height];

            if (_seed == null || _seed.Trim().Length < 1) _seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

            System.Random rand = MapManager2D.random;

            //Initialize output map array
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    outputMap[x, y] = (rand.Next(0, 100) < _chanceToStartAlive);
                }
            }

            //Repeat simulation for a number of times defined by '_outputGeneration'
            for (int i = 0; i < _outputGeneration; i++)
            {
                outputMap = Simulate(outputMap, _deathThreshold, _birthThreshold);
            }

            //Remove caves and islands that are too small
            if(_caveAliveThreshold > 0 && _islandAliveThreshold > 0)
                outputMap = RemoveCavesAndIslands(outputMap, _caveAliveThreshold, _islandAliveThreshold);

            MapManager2D.lastMap = outputMap;

            return outputMap;
        }

        /// <summary>
        /// Returns 'inputMap' simulated 1 more time
        /// </summary>
        /// <param name="inputMap"></param>
        /// <param name="_deathThreshold">If a living cell has less living neighbours than this value - it dies</param>
        /// <param name="_birthThreshold">If a dead cell has more living neighbours than this value - it becomes alive</param>
        /// <returns>'inputMap' simulated 1 more time</returns>
        public static bool[,] Simulate(bool[,] inputMap, int _deathThreshold, int _birthThreshold)
        {
            bool[,] outputMap = new bool[inputMap.GetLength(0), inputMap.GetLength(1)];

            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                for (int y = 0; y < inputMap.GetLength(1); y++)
                {
                    //Get number of living cells around the cell at x,y
                    int thisCellsNeighbourCount = GetNeighbourCount(x, y, inputMap);

                    if (inputMap[x, y])
                    {
                        //If cell at x,y is alive we check if it has less living neighbours than '_deathThreshold' in order to become dead
                        if (thisCellsNeighbourCount < _deathThreshold) outputMap[x, y] = false;
                        //If it doesn't - just record it's value to the output map
                        else outputMap[x, y] = true;
                    }
                    else
                    {
                        //If cell at x,y is dead we do the same thing but inverted and compare neighbour count to '_birthThreshold'
                        if (thisCellsNeighbourCount > _birthThreshold) outputMap[x, y] = true;
                        else outputMap[x, y] = false;
                    }
                }
            }

            return outputMap;
        }

        static List<List<Cell>> Caves;//List of all caves. Every individual 'cave' list contains a list of Cells inside that cave
        static List<List<Cell>> Islands;//List of all individual 'islands' of filled cells. Every 'island' is a list of Cells in it


        static bool[,] isCheckedForFloodFill;//Is Cell checked during flood fill

        static List<Cell> currentCave = new List<Cell>();
        static List<Cell> currentIsland = new List<Cell>();

        /// <summary>
        /// Returns the largest closed empty area in 'inputMap'
        /// </summary>
        /// <param name="inputMap"></param>
        /// <returns>The largest closed empty area in 'inputMap'</returns>
        public static bool[,] LargestCaveInArea(bool[,] inputMap)
        {
            Caves = new List<List<Cell>>();
            Islands = new List<List<Cell>>();
            isCheckedForFloodFill = new bool[inputMap.GetLength(0), inputMap.GetLength(1)];

            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                for (int y = 0; y < inputMap.GetLength(1); y++)
                {
                    if (!inputMap[x, y] && !isCheckedForFloodFill[x, y])
                    {
                        FloodFill(x, y, true, inputMap);
                        Caves.Add(currentCave); //Once the flood fill algorithm for this cave is finished we know every...
                                                //...cell in this cave so we add it to 'Caves' list
                        currentCave = new List<Cell>(); //Reset the 'currentCave' list
                    }
                }
            }

            if (Caves.Count < 1)
                return inputMap;

            //Order caves by number of cells in them so 'Caves[0]' would be the largest cave
            Caves = Caves.OrderByDescending(c => c.Count).ToList();

            bool[,] outputMap = new bool[inputMap.GetLength(0), inputMap.GetLength(1)];

            foreach (Cell cell in Caves[0])
            {
                outputMap[cell.x, cell.y] = true;
            }

            return outputMap;
        }

        /// <summary>
        /// Removes all caves smaller than '_caveAliveThreshold' and islands smaller than '_islandAliveThreshold'
        /// </summary>
        /// <param name="inputMap"></param>
        /// <param name="_caveAliveThreshold">If a cave has less dead cells inside than this value - it will be filled</param>
        /// <param name="_islandAliveThreshold">If an island has less living cells inside than this value - it will be destroyed</param>
        /// <returns>Output map</returns>
        public static bool[,] RemoveCavesAndIslands(bool[,] inputMap, int _caveAliveThreshold, int _islandAliveThreshold)
        {

            if (_caveAliveThreshold < 1 && _islandAliveThreshold < 1)
                return inputMap;

            bool[,] outputMap = inputMap;

            Caves = new List<List<Cell>>();
            Islands = new List<List<Cell>>();
            isCheckedForFloodFill = new bool[outputMap.GetLength(0), outputMap.GetLength(1)];

            for (int x = 0; x < outputMap.GetLength(0); x++)
            {
                for (int y = 0; y < outputMap.GetLength(1); y++)
                {
                    if (!outputMap[x, y] && !isCheckedForFloodFill[x, y])
                    {
                        FloodFill(x, y, true, outputMap);
                        Caves.Add(currentCave); //Once the flood fill algorithm for this cave is finished we know every...
                                                //...cell in this cave so we add it to 'Caves' list
                        currentCave = new List<Cell>(); //Reset the 'currentCave' list
                    }
                }
            }

            //Same thing we did with caves we do with islands, but instead of checking for dead cells we check for living ones
            for (int x = 0; x < outputMap.GetLength(0); x++)
            {
                for (int y = 0; y < outputMap.GetLength(1); y++)
                {
                    if (!!outputMap[x, y] && !isCheckedForFloodFill[x, y])
                    {
                        FloodFill(x, y, false, outputMap);
                        Islands.Add(currentIsland);
                        currentIsland = new List<Cell>();
                    }
                }
            }

            foreach (List<Cell> Cave in Caves.ToList())
            {
                if (Cave.Count < _caveAliveThreshold) //If number of cells in a cave is too small..
                {
                    foreach (Cell Cell in Cave)//Fill every cell in this cave
                    {
                        outputMap[Cell.x, Cell.y] = true;
                    }
                    Caves.Remove(Cave);//And remove it from the list
                }
            }

            //Same with islands but instead of filling cells we destroy every cell in an island
            foreach (List<Cell> Island in Islands.ToList())
            {
                if (Island.Count < _islandAliveThreshold)
                {
                    foreach (Cell Cell in Island)
                    {
                        outputMap[Cell.x, Cell.y] = false;
                    }
                }
            }

            //Order caves by number of cells in them so 'Caves[0]' would be the largest cave
            Caves = Caves.OrderByDescending(c => c.Count).ToList();

            return outputMap;
        }

        //Checks every cell around x,y, then every cell around every cell around x,y and so on...
        //...until there is no more cells of this type(Filled or empty, defined by 'CheckingForEmptyCells') left
        //See documentation for more infromation
        static void FloodFill(int x, int y, bool CheckingForEmptyCells, bool[,] inputMap)
        {
            if (IsOutOfMap(x, y, inputMap) || isCheckedForFloodFill[x, y] || !inputMap[x, y] != CheckingForEmptyCells) return;
            isCheckedForFloodFill[x, y] = true;
            if (CheckingForEmptyCells)
                currentCave.Add(new Cell(x, y));
            else currentIsland.Add(new Cell(x, y));
            FloodFill(x - 1, y, CheckingForEmptyCells, inputMap);
            FloodFill(x + 1, y, CheckingForEmptyCells, inputMap);
            FloodFill(x, y + 1, CheckingForEmptyCells, inputMap);
            FloodFill(x, y - 1, CheckingForEmptyCells, inputMap);
        }

        /// <summary>
        /// Returns the number of neighbours of the input cell on the input map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="inputMap"></param>
        /// <returns></returns>
        public static int GetNeighbourCount(int x, int y, bool[,] inputMap)
        {
            int neighbourCount = 0;
            for (int w = -1; w <= 1; w++)
            {
                for (int h = -1; h <= 1; h++)
                {
                    if (w == 0 && h == 0) continue;
                    if (IsOutOfMap(x + w, y + h, inputMap))
                    {
                        neighbourCount++;
                        continue;
                    }
                    if (inputMap[x + w, y + h]) neighbourCount++;
                }
            }

            return neighbourCount;
        }

        /// <summary>
        /// Checks if input coordinates are out of the 'inputMap'
        /// </summary>
        public static bool IsOutOfMap(int x, int y, bool[,] inputMap)
        {
            return x < 0 || y < 0 || x >= inputMap.GetLength(0) || y >= inputMap.GetLength(1);
        }
    }

    public struct Cell
    {
        public int x;
        public int y;

        public Cell(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}