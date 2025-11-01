using UnityEngine;
using System.Collections.Generic;

// Tilemap hücresinin koordinatları
public struct TileCoordinate
{
    public int x;
    public int y;

    public TileCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // Karşılaştırma ve hashleme için zorunlu metotlar
    public static bool operator ==(TileCoordinate a, TileCoordinate b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(TileCoordinate a, TileCoordinate b) => !(a == b);
    public override bool Equals(object obj) => obj is TileCoordinate other && this == other;
    public override int GetHashCode() => (x, y).GetHashCode();
}

// Yol bulma algoritması için her hücreyi temsil eden düğüm
public class TileNode
{
    public TileCoordinate Coordinate;
    public TileNode Parent; // Yolda bir önceki düğüm

    public TileNode(TileCoordinate coord)
    {
        Coordinate = coord;
    }
}