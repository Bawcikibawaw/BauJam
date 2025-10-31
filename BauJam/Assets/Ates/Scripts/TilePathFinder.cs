using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePathfinder : MonoBehaviour
{
    public Tilemap walkableTilemap; 
    public TileBase unwalkableTile; 
    
    private readonly TileCoordinate[] directions = new TileCoordinate[]
    {
        new TileCoordinate(0, 1), new TileCoordinate(0, -1),
        new TileCoordinate(1, 0), new TileCoordinate(-1, 0)
    };

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        if (walkableTilemap == null)
        {
            Debug.LogError("TilePathfinder: Walkable Tilemap atanmamış!");
            return new List<Vector3>();
        }

        Vector3Int startCell = walkableTilemap.WorldToCell(startWorldPos);
        Vector3Int endCell = walkableTilemap.WorldToCell(endWorldPos);
        
        if (startCell.x == endCell.x && startCell.y == endCell.y) return new List<Vector3>();
        
        Queue<TileNode> queue = new Queue<TileNode>();
        Dictionary<TileCoordinate, TileNode> visited = new Dictionary<TileCoordinate, TileNode>();

        TileNode startNode = new TileNode(new TileCoordinate(startCell.x, startCell.y));
        queue.Enqueue(startNode);
        visited.Add(startNode.Coordinate, startNode);
        TileNode finalNode = null;

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (current.Coordinate.x == endCell.x && current.Coordinate.y == endCell.y)
            {
                finalNode = current;
                break;
            }

            foreach (var dir in directions)
            {
                TileCoordinate nextCoord = new TileCoordinate(current.Coordinate.x + dir.x, current.Coordinate.y + dir.y);
                Vector3Int nextCell = new Vector3Int(nextCoord.x, nextCoord.y, 0);

                if (!visited.ContainsKey(nextCoord))
                {
                    TileBase tile = walkableTilemap.GetTile(nextCell);
                    // Engel (unwalkableTile) veya boşluk (null) kontrolü
                    if (tile == null || tile == unwalkableTile) continue; 
                    
                    TileNode nextNode = new TileNode(nextCoord) { Parent = current };
                    visited.Add(nextCoord, nextNode);
                    queue.Enqueue(nextNode);
                }
            }
        }
        
        // Yolun inşası
        if (finalNode != null)
        {
            List<Vector3> path = new List<Vector3>();
            TileNode temp = finalNode;
            while (temp.Parent != null)
            {
                Vector3 worldPos = walkableTilemap.GetCellCenterWorld(new Vector3Int(temp.Coordinate.x, temp.Coordinate.y, 0));
                path.Add(worldPos);
                temp = temp.Parent;
            }
            path.Reverse(); 
            return path;
        }

        return new List<Vector3>();
    }
}