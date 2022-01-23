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
            List<Vector3Int> blockedPositions = new List<Vector3Int>();

            foreach (var freeSpots in freeEstateSpots)
            {
                if(blockedPositions.Contains(freeSpots.Key))
                {
                    continue;
                }
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
                            var halfSize = Mathf.CeilToInt(buildingTypes[i].sizeRequired / 2.0f);
                            List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                            
                            if(VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpots, blockedPositions, ref tempPositionsBlocked))
                            {
                                blockedPositions.AddRange(tempPositionsBlocked);
                                var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpots.Key, rotation);
                                structuresDictionary.Add(freeSpots.Key, building);
                                foreach(var pos in tempPositionsBlocked)
                                {
                                    structuresDictionary.Add(pos, building);
                                }
                            }
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

        private bool VerifyIfBuildingFits(
            int halfSize,
            Dictionary<Vector3Int, Direction> freeEstateSpots,
            KeyValuePair<Vector3Int, Direction> freeSpots,
            List<Vector3Int> blockedPositions,
            ref List<Vector3Int> tempPositionsBlocked)
        {
            Vector3Int direction = Vector3Int.zero;
            if(freeSpots.Value == Direction.Down || freeSpots.Value == Direction.Up)
            {
                direction = Vector3Int.right;
            }
            else
            {
                direction = new Vector3Int(0, 0, 1);
            }
            for(int i = 1; i <= halfSize; i++)
            {
                var pos1 = freeSpots.Key + direction * i;
                var pos2 = freeSpots.Key - direction * i;
                if(!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2) ||
                    blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2))
                {
                    return false;
                }
                tempPositionsBlocked.Add(pos1);
                tempPositionsBlocked.Add(pos2);
            }
            return true;
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