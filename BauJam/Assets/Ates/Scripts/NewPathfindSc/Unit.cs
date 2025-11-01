using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Coroutine için gerekli

public class Unit : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f; 
    public float minDistanceToNode = 0.1f; 

    // Referanslar
    private PathFinding pathfinder; 
    private Grid grid;               
    private GameManager gameManager; 
    
    // Yolu Takip Etme Değişkenleri
    private List<Node> currentPath; 
    private int targetIndex;         
    
    private void Start()
    {
        Debug.Log("UNIT SCRIPT BAŞLATILIYOR (Start metodu çalıştı)"); 
    
        // Referansları al
        pathfinder = FindObjectOfType<PathFinding>();
        grid = FindObjectOfType<Grid>();
        gameManager = GameManager.Instance; 
    
        // Hata Ayıklama Logları: Hangi referansın gelmediğini kontrol edelim
        bool isPathfinderNull = pathfinder == null;
        bool isGridNull = grid == null;
        bool isGameManagerNull = gameManager == null;

        if (isPathfinderNull || isGridNull || isGameManagerNull)
        {
            // Eğer bu log çıkmıyorsa, koşulunuz yanlış demektir.
            Debug.LogError($"[KRİTİK HATA] Unit Referansları Eksik! GM: {!isGameManagerNull}, Grid: {!isGridNull}, PF: {!isPathfinderNull}.");
            enabled = false; 
            return;
        }
    
        // Eğer tüm referanslar alındıysa, ABONE OL
        gameManager.OnNPCWalkToLocation += OnWalkToLocationRequested;
        Debug.Log("UNIT Başarıyla Abone Oldu ve Tüm Referanslar Alındı."); // ABONE OLMA BAŞARISI LOGU
    }

    private void OnDestroy() // Obje yok edildiğinde aboneliği kaldır
    {
        if (gameManager != null)
        {
            gameManager.OnNPCWalkToLocation -= OnWalkToLocationRequested;
        }
    }

    // 1. OnWalkToLocationRequested (GameManager'dan çağrılır)
    // -----------------------------------------------------------------
    private void OnWalkToLocationRequested(Vector3 targetPosition)
    {
        // Önceki Coroutine'i durdur
        StopAllCoroutines(); 
        
        // Yol bulma işlemini başlat
        pathfinder.FindPath(transform.position, targetPosition);
        
        // Yol bulunduysa, takip etmeye başla
        if (grid.path != null && grid.path.Count > 0)
        {
            currentPath = grid.path;
            targetIndex = 0;
            StartCoroutine(FollowPath()); // Coroutine'i başlat
        } 
        else 
        {
            Debug.LogWarning("Yol bulunamadı veya hedef geçilemez. Hareket başlatılamadı.");
            // Yol bulunamazsa, GameManager'a hareketin bittiğini bildir.
            gameManager.isNPCMoving = false; 
        }
    }

    // 2. FollowPath (Yolu takip eden ana Coroutine)
    // ---------------------------------------------
    private IEnumerator FollowPath()
    {
        // BAŞLANGIÇ: Hareketi başlat ve GameManager'a bildir
        Debug.Log("--- Coroutine BAŞLADI ---");
        gameManager.isNPCMoving = true;
        
        // Güvenlik kontrolü: Hız sıfırsa hareket edemez
        if (moveSpeed <= 0) {
            Debug.LogError("Hareket Hızı (moveSpeed) sıfır veya negatif! NPC hareket edemez.");
            gameManager.isNPCMoving = false;
            yield break; // Coroutine'i durdur
        }

        while (targetIndex < currentPath.Count)
        {
            Vector3 currentTargetNodePos = currentPath[targetIndex].worldPosition;

            // Düğüme ulaşana kadar hareket et
            while (Vector2.Distance(transform.position, currentTargetNodePos) > minDistanceToNode)
            {
                // Hata Ayıklama Logu: Hareketin her karede ilerlediğini görmek için
                // Debug.Log($"Hareket Ediyor... Mesafe: {Vector2.Distance(transform.position, currentTargetNodePos):F3}");
                
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    currentTargetNodePos, 
                    moveSpeed * Time.deltaTime
                );
                yield return null; // Bir sonraki karede devam et
            }

            // DÜĞÜM GEÇİŞ LOGU
            Debug.Log($"Düğüme Ulaşıldı: Index {targetIndex}.");
            targetIndex++;
        }

        // SON: Hedefe ulaşıldı
        Debug.Log("--- Coroutine BİTTİ, Hedefe Ulaşıldı ---");
        gameManager.isNPCMoving = false;
        
        // Debug için yolu temizle
        grid.path = null;
        currentPath = null;
    }
}