using UnityEngine;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    private Grid grid; 

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Debug.Log($"Pathfinding başlatıldı: Başlangıç: {startPos}, Hedef: {targetPos}");
        
        // Dünya koordinatlarını Node objelerine çevir
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        // KRİTİK LOGLAR: Başlangıç ve Bitiş Düğüm Durumunu Kontrol Et
        Debug.Log($"[A* DEBUG] START NODE: Walkable: {startNode.isWalkable}, Grid Pos: ({startNode.gridX}, {startNode.gridY})");
        Debug.Log($"[A* DEBUG] TARGET NODE: Walkable: {targetNode.isWalkable}, Grid Pos: ({targetNode.gridX}, {targetNode.gridY})");
        
        // 1. Hedefin Geçilebilirlik Kontrolü
        if (!targetNode.isWalkable) 
        {
            grid.path = null;
            Debug.LogWarning("Hedef nokta geçilemez (Unwalkable) bir alanın içinde.");
            return;
        }

        // 2. Başlangıç Düğümü Geçilebilirlik Kontrolü (Opsiyonel ama önerilir)
        if (!startNode.isWalkable)
        {
            grid.path = null;
            Debug.LogWarning("NPC başlangıç noktası geçilemez bir alanın içinde. Yol bulma iptal edildi.");
            return;
        }


        Debug.Log("A* algoritması döngüsüne girmeye hazırlanılıyor..."); // KRİTİK LOG

        // A* Listeleri
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>(); 

        openSet.Add(startNode);

        // Ana A* Döngüsü
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            
            // En düşük F maliyetine sahip düğümü bul
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || 
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // HEDEF BULUNDU
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // Komşuları kontrol et
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                // Komşu geçilemez ise veya zaten incelenmişse atla
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                
                // Daha iyi bir yol bulunduysa veya düğüm Açık Listede değilse
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode); 
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        
        // Yol bulunamazsa (OpenSet boşaldıysa)
        grid.path = null;
        Debug.LogWarning("Hedefe giden bir yol bulunamadı. OpenSet boşaldı.");
    }

    // Yolu Geri İzleme Fonksiyonu
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path; 
        Debug.Log("Yol bulundu! Uzunluk: " + path.Count + " adım.");
    }

    // Heuristik Hesaplama: Mesafe (Çapraz: 14, Düz: 10 maliyet)
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}