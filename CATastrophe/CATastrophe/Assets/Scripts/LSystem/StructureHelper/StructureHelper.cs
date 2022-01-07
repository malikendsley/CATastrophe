using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
    public class StructureHelper : MonoBehaviour
    {
        public GameObject prefab;
        public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();

        public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions)
        {
            Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
            foreach (var position in freeEstateSpots.Keys)
            {
                Instantiate(prefab, position, Quaternion.identity, transform);
            }
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
                        freeSpaces.Add(newPosition, Direction.Right);
                    }
                }
            }
            return freeSpaces;
        }
    }
}