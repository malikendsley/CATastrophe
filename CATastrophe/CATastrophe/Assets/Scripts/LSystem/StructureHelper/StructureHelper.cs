using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
    public class StructureHelper : MonoBehaviour
    {
        public BuildingType[] buildingTypes;
        public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();

        public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions)
        {
            Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
            foreach (var freeSpots in freeEstateSpots)
            {
                var rotation = Quaternion.identity;
                switch (freeSpots.Value)
                {
                    case Direction.Up:
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case Direction.Down:
                        rotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case Direction.Right:
                        rotation = Quaternion.Euler(0, 180, 0);
                        break;
                    default:
                        break;
                }
                for(int i = 0; i < buildingTypes.Length; i++)
                {
                    if(buildingTypes[i].quantity == -1)
                    {
                        var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpots.Key, rotation);
                        structuresDictionary.Add(freeSpots.Key, building);
                        break;
                    }
                    if (buildingTypes[i].IsBuildingAvailable())
                    {
                        if (buildingTypes[i].sizeRequired > 1)
                        {
                            
                        }
                        else
                        {
                            var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpots.Key, rotation);
                            structuresDictionary.Add(freeSpots.Key, building);
                            
                        }
                        break;
                    }

                }
            }
        }

        private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
        {
            var newStructure = Instantiate(prefab, position, rotation, transform);
            return newStructure;
        }

        private Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions)
        {
            Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
            foreach (var position in roadPositions)
            {
                var neighborDirections = PlacementHelper.FindNeighbor(position, roadPositions);
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if(neighborDirections.Contains(direction) == false)
                    {
                        var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                        if(freeSpaces.ContainsKey(newPosition))
                        {
                            continue;
                        }
                        freeSpaces.Add(newPosition, PlacementHelper.GetReverseDirection(direction));
                    }
                }
            }
            return freeSpaces;
        }
    }
}