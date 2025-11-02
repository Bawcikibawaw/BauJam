using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    // A* Algoritmasının kullanacağı düğümler dizisi
    public Node[,] grid; 
    
    [Header("Grid Ayarları")]
    [Tooltip("Oyun alanınızın dünya koordinatlarındaki toplam büyüklüğü.")]
    public Vector2 gridWorldSize; 
    
    [Tooltip("Bir düğümün (node) yarıçapı. Genellikle Tile boyutunun yarısıdır (örn: 0.5f).")]
    public float nodeRadius = 0.5f; 
    
    [Header("Engel Ayarları")]
    [Tooltip("Geçilemez engellerin bulunduğu Tilemap. Bu Tilemap üzerindeki herhangi bir Tile engel sayılır.")]
    public Tilemap obstacleTilemap; 
    
    // NPC'ye iletilecek ve Gizmos ile çizilecek yol
    [HideInInspector] public List<Node> path; 

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        // Izgara boyutlarını hesapla
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    // Izgarayı oluşturan ana fonksiyon
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        // Izgaranın sol alt köşesinin dünya koordinatını hesapla
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Mevcut düğümün dünya konumunu hesapla
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                
                bool walkable = true;
                if (obstacleTilemap != null)
                {
                    // Dünya koordinatını Tilemap'in hücre koordinatına çevir
                    Vector3Int cellPos = obstacleTilemap.WorldToCell(worldPoint);
                    
                    // ÇOKLU ENGEL ÇÖZÜMÜ: O hücrede herhangi bir Tile olup olmadığını kontrol et
                    if (obstacleTilemap.GetTile(cellPos) != null)
                    {
                        walkable = false;
                    }
                }

                // Düğümü oluştur ve ızgaraya kaydet
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
        Debug.Log($"[GRID] Izgara başarıyla oluşturuldu. Boyut: {gridSizeX}x{gridSizeY}");
    }

    // Dünya konumundan en yakın ızgara düğümünü döndürür (Kayma Kontrolü Logları Dahil)
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        // Yüzdelik konumu hesapla (0 ile 1 arasında)
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        
        // Değerleri sınırla (0-1)
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Izgara indeksini hesapla
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
        // KRİTİK HATA AYIKLAMA LOGU: Kayma olup olmadığını buradan kontrol edin
        Debug.Log($"[GRID DEBUG] Dünya Konumu {worldPosition:F2} -> Yüzde: ({percentX:F2}, {percentY:F2}) -> Grid İndeksi: ({x}, {y})");

        // Kayma nedeniyle indeksler bazen array dışına çıkabilir, bu kontrol genellikle NodeFromWorldPoint mantığı ile halledilir ama yine de bir kontrol eklenebilir.
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
             Debug.LogError($"[GRID HATA] Hesaplanan indeks ({x}, {y}) ızgara sınırları dışında! Konum kaymasını kontrol edin.");
             // Sınırlar dışındaysa en yakın sınır düğümünü döndür (güvenlik)
             x = Mathf.Clamp(x, 0, gridSizeX - 1);
             y = Mathf.Clamp(y, 0, gridSizeY - 1);
        }
        
        return grid[x, y];
    }
    
    // Mevcut düğümün komşularını döndürür (Çapraz dahil 8 yön)
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
    
    // Debugging için ızgarayı ve yolu çizer
    void OnDrawGizmos()
    {
        // Izgara sınırlarını çiz
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            // Tüm düğümleri çiz (Geçilebilir: Yarı saydam Beyaz, Geçilemez: Kırmızı)
            foreach (Node n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.white * 0.5f : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
            
            // Bulunan yolu sarı ile çiz
            if (path != null)
            {
                Gizmos.color = Color.yellow; 
                foreach (Node n in path)
                {
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter * 0.5f)); 
                }
            }
        }
    }
}