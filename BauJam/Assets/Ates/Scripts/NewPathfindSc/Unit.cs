using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System; // Action için

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
    
    // Collider Yönetimi için yeni değişken
    private Collider2D currentTargetCollider; // <-- Sadece hareket eden hedef objenin collider'ını tutar
    
    private List<Node> currentPath; 
    private int targetIndex;         
    
    private void Start()
    {
        animator = GetComponent<Animator>();
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
        
        // KRİTİK GÜNCELLEME: Event parametresi PathTarget oldu
        gameManager.OnNPCWalkToLocation += OnWalkToLocationRequested;
    }

    private void OnDestroy() 
    {
        if (gameManager != null)
        {
            gameManager.OnNPCWalkToLocation -= OnWalkToLocationRequested;
        }
    }

    // GameManager'dan hedef objesi geldiğinde bu fonksiyon tetiklenir
    private void OnWalkToLocationRequested(PathTarget targetObject) // <-- PARAMETRE GÜNCELLENDİ
    {
        StopAllCoroutines(); 
        
        // 1. Önceki hedef collider'ı KAPAT
        if (currentTargetCollider != null)
        {
            currentTargetCollider.enabled = false;
        }

        // 2. Yeni hedef collider'ı al ve AÇ
        currentTargetCollider = targetObject.GetComponent<Collider2D>();
        
        if (currentTargetCollider != null)
        {
            currentTargetCollider.enabled = true; // <-- HAREKET BAŞLADIĞI AN AÇ
            Debug.Log($"Hedef Collider ({targetObject.name}) açıldı.");
        }


        // 3. Pathfinding'i başlat (konum, PathTarget objesinden alınır)
        pathfinder.FindPath(transform.position, targetObject.transform.position);
        
        if (grid.path != null && grid.path.Count > 0)
        {
            currentPath = grid.path;
            targetIndex = 0;
            StartCoroutine(FollowPath());
        } 
        else 
        {
            Debug.LogWarning("Yol bulunamadı! NPC hareket edemiyor.");
            gameManager.isNPCMoving = false;
            
            // Yol bulunamazsa collider'ı kapat
            if (currentTargetCollider != null)
            {
                currentTargetCollider.enabled = false; 
                currentTargetCollider = null; 
            }
        }
    }

    private IEnumerator FollowPath()
    {
        gameManager.isNPCMoving = true;
        SetWalking(true); 

        while (targetIndex < currentPath.Count)
        {
            Vector3 currentTargetNodePos = currentPath[targetIndex].worldPosition;
            Vector2 direction = (currentTargetNodePos - transform.position).normalized;
            
            SetDirection(direction.x, direction.y); 

            while (Vector2.Distance(transform.position, currentTargetNodePos) > minDistanceToNode)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    currentTargetNodePos, 
                    moveSpeed * Time.deltaTime
                );
                direction = (currentTargetNodePos - transform.position).normalized;
                SetDirection(direction.x, direction.y);
                
                yield return null; 
            }

            targetIndex++;
        }

        // SON: Hedefe ulaşıldı
        gameManager.isNPCMoving = false;
        SetWalking(false); 
        
        // 🚨 HEDEFE ULAŞILDI: COLLIDER'I KAPAT
        if (currentTargetCollider != null)
        {
            currentTargetCollider.enabled = false; 
            Debug.Log($"Hedef Collider ({currentTargetCollider.gameObject.name}) kapatıldı.");
            currentTargetCollider = null; // Temizle
        }
        
        grid.path = null;
        currentPath = null;
    }

    private void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isWalking);
        }
    }

    private void SetDirection(float horizontalInput, float verticalInput)
    {
        if (animator != null)
        {
            animator.SetFloat("moveX", horizontalInput);
            animator.SetFloat("moveY", verticalInput);
        }
    }
}