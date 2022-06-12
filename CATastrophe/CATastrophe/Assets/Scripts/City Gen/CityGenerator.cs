using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class CityGenerator : MonoBehaviour
{

    public int TileSideLength = 2;
    //recommended these be divisible by 3
    public int XSize = 12;
    public int ZSize = 12;

    //allows building a mesh when city is created
    [SerializeField]
    private NavMeshSurface _surface;

    [Range(0, 100)]
    public int AlleyDensity = 50;
    [Range(0, 100)]
    public int AlleyBias = 50;

    private int[,] _posGrid;
    private int[,] _rotGrid;

    public GameObject StraightRoad;
    public GameObject TRoad;
    public GameObject CrossRoad;

    public GameObject SolidHouse;
    public GameObject StraightAlley;
    public GameObject LAlley;



    //1 = straight piece
    //2 = t shaped piece
    //3 = 4 way piece

    //11 = solid house
    //12 = straight alley
    //13 = l shaped alley

    //look into a primitive marching squares-esque system for this
    [InspectorButton("OnButtonClicked")]
    public bool GenerateCityButton;

    private void OnButtonClicked()
    {
        GenerateCity();

    }

    private void GenerateCity()
    {
        GenerateTileGrid();
        PrettyPrintGrid(_posGrid);
        GenerateRotGrid();
        PrettyPrintGrid(_rotGrid);
        ParseGrid(_posGrid);
        _surface.RemoveData();
        _surface.BuildNavMesh();
        
    }

    private void GenerateTileGrid()
    {
        _posGrid = new int[ZSize, XSize];
        //lay out road grid
        for (var z = 0; z < ZSize; z++)
        {
            for (var x = 0; x < XSize; x++)
            {
                if (x % 3 == 0 || z % 3 == 0)
                {
                    _posGrid[z, x] = 1;
                }
            }
        }

        //Debug.Log("Roads marked");
        //PrettyPrintGrid(_posGrid);
        //the first grouping is bounded at (1, 1), (2, 1), (1, 2), (2, 2) and each one is offset by 2 relative to the neighbor
        for (var z = 1; z < ZSize; z += 3)
        {
            for (var x = 1; x < XSize; x += 3)
            {
                _posGrid[z, x] = GenHouse();
                _posGrid[z + 1, x] = GenHouse();
                _posGrid[z, x + 1] = GenHouse();
                _posGrid[z + 1, x + 1] = GenHouse();
            }
        }

        //Debug.Log("Houses Placed");
        //PrettyPrintGrid(_posGrid);
        //convert roads to proper types, this is intended to be robust against external tilegrid changes, like fixed geometry
        for (var z = 0; z < ZSize; z++)
        {
            for (var x = 0; x < XSize; x++)
            {
                //only check road pieces
                if (_posGrid[z, x] != 1) continue;
                bool down, left, right;

                var up = down = left = right = false;
                //check neighbors, 2 neighbors is a straight, 3 neighbors is a T and 4 neighbors is an intersection
                if (z + 1 < ZSize)
                {
                    up = _posGrid[z + 1, x] == 1;
                }
                if (z - 1 >= 0)
                {
                    down = _posGrid[z - 1, x] == 1;
                }
                if (x + 1 < XSize)
                {
                    right = _posGrid[z, x + 1] == 1;
                }
                if (x - 1 >= 0)
                {
                    left = _posGrid[z, x - 1] == 1;
                }
                //simplified 3/4 majority boolean logic
                if ((up ^ down ^ left ^ right) & ((up & down) | (left & right)))
                {
                    _posGrid[z, x] = 2;
                }
                if (up && down && left && right)
                {
                    _posGrid[z, x] = 3;
                }
            }
        }
        //Debug.Log("Roads converted");
        //PrettyPrintGrid(_posGrid);
    }

    private void GenerateRotGrid()
    {
        //NB: all tiles have are oriented the same way
        _rotGrid = new int[ZSize, XSize];
        //buildings street facing faces are in the -X and -Z direction (-Z is "down", -X is "left")
        for (var z = 0; z < ZSize; z++)
        {
            for (var x = 0; x < XSize; x++)
            {
                //for roads
                if (_posGrid[z, x] < 10)
                {
                    _rotGrid[z, x] = AlignRoadTile(z, x);
                }
                else if (_posGrid[z, x] > 10)
                {
                    _rotGrid[z, x] = AlignBuildingTile(z, x);
                }
            }
        }
    }

    private void ParseGrid(int[,] posGrid)
    {
        for (var x = 0; x < XSize; x++)
        {
            for (var z = 0; z < XSize; z++)
            {
                var rot = Quaternion.Euler(new Vector3(0, _rotGrid[z, x] * -90, 0));
                var chosen = posGrid[z, x] switch
                {
                    1 => StraightRoad,
                    2 => TRoad,
                    3 => CrossRoad,
                    11 => SolidHouse,
                    12 => StraightAlley,
                    13 => LAlley,
                    _ => CrossRoad
                };
                GameObject.Instantiate(chosen, new Vector3(x, 0, z) * TileSideLength, rot);
            }
        }
    }

    //honestly not that pretty
    private static void PrettyPrintGrid(int[,] grid)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < grid.GetLength(1); i++)
        {
            for (var j = 0; j < grid.GetLength(0); j++)
            {
                sb.Append(grid[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    private int GenHouse()
    {
        if (Random.value * 100 < AlleyDensity)
        {
            return Random.value * 100 > AlleyBias ? 12 : 13;
        }
        return 11;
    }

    private int AlignRoadTile(int z, int x)
    {
        //0 = no rotation
        //1 = 90 deg ccw
        //2 = 180 deg ccw
        //3 = 180 deg ccw
        if (z >= ZSize && x >= XSize)
        {
            return 0;
        }
        var roadType = _posGrid[z, x];
        switch (roadType)
        {
            case 1:
                //roads default to left right, check the tile above, if its a road rotate 90 otherwise leave it
                if (z + 1 >= ZSize || _posGrid[z + 1, x] < 10)
                    return 1;
                else
                    return 0;
            case 2:
                //the blocked section of a t road is facing right (+X) by default
                //out of bounds is equivalent to the face away direction
                if (z + 1 >= ZSize || _posGrid[z + 1, x] > 9)
                    return 1;
                if (z - 1 < 0 || _posGrid[z - 1, x] > 9)
                    return 3;
                if (x - 1 < 0 || _posGrid[z, x - 1] > 9)
                    return 2;
                return 0;
            default:
                return 0;
        }
    }


    private int AlignBuildingTile(int z, int x)
    {
        //0 = bottom left corner    no rotation
        //1 = bottom right corner   90 deg ccw
        //2 = top right corner      180 deg ccw
        //3 = top left corner       180 deg ccw
        var isUpTileRoad = (z + 1 < ZSize && _posGrid[z + 1, x] < 10);
        var isLeftTileRoad = (x - 1 >= 0 && _posGrid[z, x - 1] < 10);

        if (isUpTileRoad)
        {
            return isLeftTileRoad ? 3 : 2;
        }
        return isLeftTileRoad ? 0 : 1;
    }
}