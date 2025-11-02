using UnityEngine;

public class Node
{
    // Düğümün sahne üzerindeki dünya konumu (World Position)
    public Vector2 worldPosition; 
    
    // Düğümün ızgara içindeki X ve Y koordinatları
    public int gridX;
    public int gridY;

    // Geçilebilir (Yürünebilir) mi? (Engel var mı?)
    public bool isWalkable; 

    // A* Maliyetleri
    public int gCost; // Başlangıçtan bu düğüme olan gerçek maliyet
    public int hCost; // Bu düğümden hedefe olan tahmin edilen (Heuristik) maliyet
    
    // Yolu geri izlemek için ebeveyn düğüm
    public Node parent; 

    // F Maliyeti: G + H
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Node(bool _isWalkable, Vector2 _worldPos, int _gridX, int _gridY)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}