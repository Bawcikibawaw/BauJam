using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f; 
    public float minDistanceToNode = 0.1f; 

    // Referanslar
    private Animator animator; 
    private SpriteRenderer spriteRenderer; 
    private PathFinding pathfinder; 
    private Grid grid;               
    private GameManager gameManager; 
    
    // ... (Yol Takip Değişkenleri)
    private List<Node> currentPath; 
    private int targetIndex;         
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        // SpriteRenderer sadece Flip için gereklidir, eğer animasyonlarınızda Flip kullanacaksanız tutun.
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        pathfinder = FindObjectOfType<PathFinding>();
        grid = FindObjectOfType<Grid>();
        gameManager = GameManager.Instance; 
        
        if (pathfinder == null || grid == null || gameManager == null || animator == null)
        {
            Debug.LogError($"[FATAL] Unit Referansları Eksik! Kontrol edin. Unit devre dışı bırakıldı.");
            enabled = false; 
            return;
        }
        
        gameManager.OnNPCWalkToLocation += OnWalkToLocationRequested;
    }

    private void OnDestroy() 
    {
        if (gameManager != null)
        {
            gameManager.OnNPCWalkToLocation -= OnWalkToLocationRequested;
        }
    }

    private void OnWalkToLocationRequested(Vector3 targetPosition)
    {
        StopAllCoroutines(); 
        pathfinder.FindPath(transform.position, targetPosition);
        
        if (grid.path != null && grid.path.Count > 0)
        {
            currentPath = grid.path;
            targetIndex = 0;
            StartCoroutine(FollowPath());
        } 
        else 
        {
            Debug.LogWarning("Yol bulunamadı veya hedef geçilemez. Hareket başlatılamadı.");
            gameManager.isNPCMoving = false;
            SetWalking(false); 
        }
    }

    private IEnumerator FollowPath()
    {
        gameManager.isNPCMoving = true;
        SetWalking(true); // Yürüme animasyonunu başlat

        while (targetIndex < currentPath.Count)
        {
            Vector3 currentTargetNodePos = currentPath[targetIndex].worldPosition;
            
            // Düğüm merkezine olan yön vektörünü hesapla
            Vector2 direction = (currentTargetNodePos - transform.position).normalized;
            
            // Hareket Yönünü Animator'a Gönder
            SetDirection(direction.x, direction.y); // <-- YENİ KONTROL FONKSİYONU

            // Düğüme ulaşana kadar hareket et
            while (Vector2.Distance(transform.position, currentTargetNodePos) > minDistanceToNode)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    currentTargetNodePos, 
                    moveSpeed * Time.deltaTime
                );
                // Hareket sırasında yönü sürekli güncelle (Bu, Blend Tree'yi canlandırır)
                direction = (currentTargetNodePos - transform.position).normalized;
                SetDirection(direction.x, direction.y);
                
                yield return null; 
            }

            // Düğüme ulaşıldı, bir sonraki düğüme geç
            targetIndex++;
        }

        // SON: Hedefe ulaşıldı
        gameManager.isNPCMoving = false;
        SetWalking(false); // Yürüme animasyonunu durdur
        
        // Son duruş yönünü koru (Idle animasyonunun son baktığı yönde kalması için)
        
        grid.path = null;
        currentPath = null;
    }

    // Yürüme durumunu ayarlayan yardımcı metot
    private void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isWalking);
        }
    }

    // Karakterin yönünü Animator parametreleri aracılığıyla ayarlayan yardımcı metot
    private void SetDirection(float horizontalInput, float verticalInput)
    {
        if (animator != null)
        {
            // Animator'a X ve Y yönlerini iletir
            // Blend Tree, bu değerlere bakarak hangi animasyonu oynatacağına karar verir.
            animator.SetFloat("moveY", horizontalInput);
            animator.SetFloat("moveX", verticalInput);
        }
    }
}