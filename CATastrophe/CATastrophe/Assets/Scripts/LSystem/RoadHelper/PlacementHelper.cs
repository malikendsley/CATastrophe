using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
    public static class PlacementHelper
    {
        public static List<Direction> FindNeighbor(Vector3Int position, ICollection<Vector3Int> collection)
        {
            List<Direction> neighbourDirections = new List<Direction>();
            if (collection.Contains(position + Vector3Int.right))
            {
                neighbourDirections.Add(Direction.Right);
            }
            if (collection.Contains(position - Vector3Int.right))
            {
                neighbourDirections.Add(Direction.Left);
            }
            if (collection.Contains(position + new Vector3Int(0, 0, 1)))
            {
                neighbourDirections.Add(Direction.Up);
            }
            if (collection.Contains(position - new Vector3Int(0, 0, 1)))
            {
                neighbourDirections.Add(Direction.Down);
            }
            return neighbourDirections;
        }
    }
}