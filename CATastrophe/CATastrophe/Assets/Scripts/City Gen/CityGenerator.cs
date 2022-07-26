using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CityGenerator : MonoBehaviour
{

    public int TileSideLength = 2;

    public int XSize = 12;
    public int ZSize = 12;

    public int ParkCap = 0;
    
    //allows building a mesh when city is created
    public NavMeshSurface Surface;

    [Range(0, 100)]
    public int AlleyDensity = 50;
    [Range(0, 100)]
    public int AlleyBias = 50;

    private int[,] _posGrid;
    private int[,] _rotGrid;

    private readonly List<(int, int)> _parkTargets = new List<(int, int)>();
    //use this to contain terrain
    private GameObject _emptyParent;
    
    public GameObject StraightRoad;
    public GameObject TRoad;
    public GameObject CrossRoad;
    public GameObject CornerRoad;
    
    public GameObject SolidHouse;
    public GameObject StraightAlley;
    public GameObject LAlley;

    public GameObject BeachStraight;
    public GameObject BeachCorner;

    public GameObject Park;
    public GameObject Water;

    private GameObject waterRef;
    private bool _pastClicked;
    private readonly List<GameObject> _citypieces = new List<GameObject>();
    private bool cityMade = false;

    private Vector3 homeTile;
    private System.Random rand = new System.Random();
    //1 = straight piece
    //2 = t shaped piece
    //3 = 4 way piece

    //11 = solid house
    //12 = straight alley
    //13 = l shaped alley

    //look into a primitive marching squares-esque system for this
    [InspectorButton("OnButtonClicked")] [UsedImplicitly]
    public bool GenerateCityButton;

    [UsedImplicitly]
    private void OnButtonClicked()
    {
        if (_pastClicked)
        {
            ClearCity();
        }
        GenerateCity();
        Debug.Log("Past Clicked: " + _pastClicked);
            _pastClicked = true;
    }

    private void GenerateCity()
    {

        GenerateTileGrid();
        GenerateRotGrid();
        PlaceParks();
        LayBeach();
        ParseGrid(_posGrid, _rotGrid);
        Surface.RemoveData();
        Surface.BuildNavMesh();
        cityMade = true;
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

                if (!up || !down || !left || !right) continue;
                _posGrid[z, x] = 3;
                _parkTargets.Add((z, x));
            }
        }

        //lay out corner pieces at corners of matrix
        _posGrid[0, 0] = 4;
        _posGrid[0, XSize - 1] = 4;
        _posGrid[ZSize - 1, 0] = 4;
        _posGrid[ZSize - 1, XSize - 1] = 4;

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
                else if (_posGrid[z, x] >= 10)
                {
                    _rotGrid[z, x] = AlignBuildingTile(z, x);
                }
            }
        }
    }

    private void PlaceParks()
    {
        Debug.Log("# of park targets: " + _parkTargets.Count);
        _parkTargets.Shuffle();
        //cap the number of parks to either number of slots or user defined
        for (var i = 0; i < Math.Min(_parkTargets.Count, ParkCap); i++)
        {
            
            var (z, x) = _parkTargets[i];
            //Debug.Log("Evaluating" + z + ", " + x);
            //check surroundings to avoid placing parks too close
            bool downValid, leftValid, rightValid;
            var upValid = downValid = leftValid = rightValid = false;

            var upInBounds = z + 4 < ZSize;
            var downInBounds = z - 4 >= 0;
            var leftInBounds = x - 4 >= 0;
            var rightInBounds = x + 4 < XSize;

            bool urValid, llValid, lrValid;
            var ulValid = urValid = llValid = lrValid = false;
            //out of bounds equates to true, short circuit to respect index bounds
            if (z + 4 >= ZSize || _posGrid[z + 4, x] < 30)
            {
                //Debug.Log("Up Valid");
                upValid = true;
            }
            if (z - 4 < 0 || _posGrid[z - 4, x] < 30)
            {
                //Debug.Log("Down Valid");
                downValid = true;
            }
            if (x + 4 >= XSize || _posGrid[z, x + 4] < 30)
            {
                //Debug.Log("Right Valid");
                rightValid = true;
            }
            if (x - 4 < 0 || _posGrid[z, x - 4] < 30)
            {
                //Debug.Log("Left Valid");
                leftValid = true;
            }
            if (upValid && downValid && leftValid && rightValid)
            {
                //out of bounds equates to true, short circuit to respect index bounds
                if (!upInBounds || !leftInBounds || _posGrid[z + 4, x - 4] < 30)
                {
                    ulValid = true;
                }

                if (!upInBounds || !rightInBounds || _posGrid[z + 4, x + 4] < 30)
                {
                    urValid = true;
                }

                if (!downInBounds || !leftInBounds || _posGrid[z - 4, x - 4] < 30)
                {
                    llValid = true;
                }

                if (!downInBounds || !rightInBounds || _posGrid[z - 4, x + 4] < 30)
                {
                    lrValid = true;
                }

                if (ulValid && urValid && llValid && lrValid)
                {
                    LayPark(z, x);
                }
            }
        }
        _parkTargets.Clear();
        
    }

    private void LayPark(int z, int x)
    {
        for (var i = -2; i <= 2; i++)
        {
            for (var j = -2; j <= 2; j++)
            {
                _posGrid[z + i, x + j] = 30;
            }
        }
        Debug.Log("Placing Park");
        _posGrid[z, x] = 31;
        _rotGrid[z, x] = Random.Range(0, 4);
    }

    private void LayBeach()
    {
        var newPosGrid = Pad(_posGrid);
        var newRotGrid = Pad(_rotGrid);

        for (var z = 0; z < ZSize + 2; z++)
        {
            newPosGrid[z, 0] = 21;
            newPosGrid[z, XSize + 1] = 21;
            newRotGrid[z, 0] = 1;
            newRotGrid[z, XSize + 1] = 3;
        }

        for (var x = 0; x < XSize + 2; x++)
        {
            newPosGrid[0, x] = 21;
            newPosGrid[ZSize + 1, x] = 21;
            newRotGrid[0, x] = 2;
            newRotGrid[ZSize + 1, x] = 0;
        }

        newPosGrid[0, 0] = 22;
        newPosGrid[0, XSize + 1] = 22;
        newPosGrid[ZSize + 1, 0] = 22;
        newPosGrid[ZSize + 1, XSize + 1] = 22;

        newRotGrid[0, 0] = 1;
        newRotGrid[0, XSize + 1] = 2;
        newRotGrid[ZSize + 1, 0] = 0;
        newRotGrid[ZSize + 1, XSize + 1] = 3;
        
        _posGrid = newPosGrid;
        _rotGrid = newRotGrid;
    }

    private void ParseGrid(int[,] posGrid, int[,] rotGrid)
    {

        //ive been doing this manually over and over, just codify it already
        
        _emptyParent = new GameObject("Terrain")
        {
            transform =
            {
                position = Vector3.zero,
                rotation = Quaternion.identity
            }
        };
        _citypieces.Add(_emptyParent);
        var parentTransform = _emptyParent.transform;

        for (var x = 0; x < posGrid.GetLength(1); x++)
        {
            for (var z = 0; z < posGrid.GetLength(0); z++)
            {
                var rot = Quaternion.Euler(new Vector3(0, rotGrid[z, x] * -90, 0));
                var chosen = posGrid[z, x] switch
                {
                    0 => null,
                    1 => StraightRoad,
                    2 => TRoad,
                    3 => CrossRoad,
                    4 => CornerRoad,
                    11 => SolidHouse,
                    12 => StraightAlley,
                    13 => LAlley,
                    21 => BeachStraight,
                    22 => BeachCorner,
                    30 => null,
                    31 => Park,
                    _ => CrossRoad
                };

                if (chosen != null)
                {
                    _citypieces.Add(GameObject.Instantiate(chosen, new Vector3(x, 0, z) * TileSideLength, rot, parentTransform));
                }
            }
        }

        homeTile = _citypieces[rand.Next(_citypieces.Count)].transform.position;
        waterRef = GameObject.Instantiate(Water, new Vector3((TileSideLength * XSize) / 2, -1, (TileSideLength * XSize) / 2), Quaternion.identity, parentTransform);
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
        
        var roadType = _posGrid[z, x];
        switch (roadType)
        {
            case 1:
                //roads default to left right, check the tile above, if its a road rotate 90 otherwise leave it
                if (z + 1 < ZSize && _posGrid[z + 1, x] < 10)
                    return 1;
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
            case 3:
                return 0;
            case 4:
                //Debug.Log("Aligning at corner");
                //the blocked section of corner pieces are in the +X and -Z directions at 0 rot
                var up = (z + 1 < ZSize && _posGrid[z + 1, x] < 10);
                var left = (x - 1 >= 0 && _posGrid[z, x - 1] < 10);

                return up switch
                {
                    true when left => 3,
                    true => 2,
                    false when left => 0,
                    _ => 1
                };

            default:
                Debug.Log("Defaulting, Error: Road type = " + roadType);
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

    public static int[,] Pad(int[,] input)
    {
        var h = input.GetLength(0);
        var w = input.GetLength(1);
        var output = new int[h + 2, w + 2];

        for (var r = 0; r < h; ++r)
        {
            Array.Copy(input, r * w, output, (r + 1) * (w + 2) + 1, w);
        }

        return output;
    }

    public void ClearCity()
    {
        Debug.Log("Clearing City");
        _parkTargets.Clear();
        if (_posGrid != null)
        {
            Array.Clear(_posGrid, 0, _posGrid.Length);
        }
        if (_rotGrid != null)
        {
            Array.Clear(_rotGrid, 0, _rotGrid.Length);
        }

        foreach (var obj in _citypieces)
        {
            DestroyImmediate(obj);
        }
        _citypieces.Clear();
        DestroyImmediate(waterRef);
    }

    public Vector3 getSpawn()
    {
        return homeTile;
    }

    public bool isCityMade()
    {
        return cityMade;
    }
}

