using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{

    public int tileSideLength = 2;
    //recommended these be divisible by 3
    public int xSize = 12;
    public int zSize = 12;


    [Range(0, 100)]
    public int alleyDensity = 50;
    [Range(0, 100)]
    public int alleyBias = 50;
    private int[,] tilegrid;
    
    public GameObject straightRoad;
    public GameObject tRoad;
    public GameObject crossRoad;

    public GameObject solidHouse;
    public GameObject straightAlley;
    public GameObject lAlley;



    //1 = straight piece
    //2 = t shaped piece
    //3 = 4 way piece

    //11 = solid house
    //12 = straight alley
    //13 = l shaped alley

    //look into a primitive marching squares-esque system for this
    private void Awake()
    {
        tilegrid = GenerateTileGrid();
        ProcessGrid(tilegrid);
    }

    int[,] GenerateTileGrid()
    {
        int[,] grid = new int[xSize, zSize];
        //lay out road grid
        for (int Z = 0; Z < zSize; Z++)
        {
            for (int X = 0; X < xSize; X++)
            {
                if (X % 3 == 0 || Z % 3 == 0)
                {
                    grid[Z, X] = 1;
                }
            }
        }

        Debug.Log("Roads marked");
        PrettyPrintGrid(grid);
        //the first grouping is bounded at (1, 1), (2, 1), (1, 2), (2, 2) and each one is offset by 2 relative to the neighbor
        for (int mZ = 1; mZ < zSize; mZ += 3)
        {
            for (int mX = 1; mX < xSize; mX += 3)
            {
                //Debug.Log("Working at X: " + mZ + " Y: " + mX);
                grid[mZ, mX] = GenHouse();
                grid[mZ + 1, mX] = GenHouse();
                grid[mZ, mX + 1] = GenHouse();
                grid[mZ + 1, mX + 1] = GenHouse();
            }
        }

        Debug.Log("Houses Placed");
        PrettyPrintGrid(grid);
        //convert roads to proper types, this is intended to be robust against external tilegrid changes, like fixed geometry
        for (int mZ = 0; mZ < zSize; mZ++)
        {
            for (int mX = 0; mX < xSize; mX++)
            {
                //only check road pieces
                if (grid[mZ, mX] == 1)
                {
                    bool up, down, left, right;

                    up = down = left = right = false;
                    //check neighbors, 2 neighbors is a straight, 3 neighbors is a T and 4 neighbors is an intersection
                    if (mZ + 1 < zSize)
                    {
                        up = grid[mZ + 1, mX] == 1;
                    }
                    if (mZ - 1 >= 0)
                    {
                        down = grid[mZ - 1, mX] == 1;
                    }
                    if (mX + 1 < xSize)
                    {
                        right = grid[mZ, mX + 1] == 1;
                    }
                    if (mX - 1 >= 0)
                    {
                        left = grid[mZ, mX - 1] == 1;
                    }
                    //simplified 3/4 majority boolean logic
                    if ((up ^ down ^ left ^ right) & ((up & down) | (left & right)))
                    {
                        grid[mZ, mX] = 2;
                    }
                    if (up && down && left && right)
                    {
                        grid[mZ, mX] = 3;
                    }
                }
            }
        }
        Debug.Log("Roads converted");
        PrettyPrintGrid(grid);
        return grid;
    }

    void ProcessGrid(int[,] grid)
    {
        for (int X = 0; X < xSize; X++)
        {
            for (int Y = 0; Y < xSize; Y++)
            {
                switch (grid[Y, X])
                {
                    case 1:
                        GameObject.Instantiate(straightRoad, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                    case 2:
                        GameObject.Instantiate(tRoad, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                    case 3:
                        GameObject.Instantiate(crossRoad, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                    case 11:
                        GameObject.Instantiate(solidHouse, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                    case 12:
                        GameObject.Instantiate(straightAlley, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                    case 13:
                        GameObject.Instantiate(lAlley, new Vector3(X, 0, Y) * tileSideLength, Quaternion.identity);
                        break;
                }
            }
        }
    }

    //honestly not that pretty
    void PrettyPrintGrid(int[,] grid)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                sb.Append(grid[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    int GenHouse()
    {
        if (Random.value * 100 < alleyDensity)
        {
            if (Random.value * 100 > alleyBias)
            {
                return 12;
            }
            else
            {
                return 13;
            }
        }
        return 11;
    }
}