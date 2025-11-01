using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))] 
public class NPCController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TilePathfinder pathfinder;
    
    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 3f; 
    
    // HATA DÜZELTME: stopDistance sınıf düzeyinde tanımlandı
    [SerializeField] private float stopDistance = 0.05f; 
    
    private List<Vector3> currentPath = new List<Vector3>();
    private int currentPathIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        
        pathfinder = FindObjectOfType<TilePathfinder>(); 
        if (pathfinder == null) Debug.LogError("Sahneye TilePathfinder ekleyin!");
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnNPCWalkToLocation += StartMovementTo;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnNPCWalkToLocation -= StartMovementTo;
        }
    }
    
    public void StartMovementTo(Vector3 targetWorldPosition)
    {
        StopAllCoroutines(); 

        currentPath = pathfinder.FindPath(transform.position, targetWorldPosition);
        currentPathIndex = 0;

        if (currentPath.Count > 0)
        {
            StartCoroutine(FollowPathCoroutine());
        }
        else
        {
            Debug.LogWarning("Yol bulunamadı! Engel veya Tilemap ayarlarını kontrol edin.");
            // Yol bulunamazsa bile hareketi sonlandır
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isNPCMoving = false;
            }
        }
    }

    private IEnumerator FollowPathCoroutine()
    {
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPosition = currentPath[currentPathIndex];
            
            yield return StartCoroutine(MoveToSingleTarget(targetPosition));
            
            currentPathIndex++;
        }
        
        Debug.Log("NPC nihai hedefe ulaştı.");
    }

    private IEnumerator MoveToSingleTarget(Vector3 targetPosition)
    {
        // Artık sınıf seviyesindeki stopDistance kullanılıyor.
        while (Vector3.Distance(transform.position, targetPosition) > stopDistance) 
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Hedefe ulaşıldı
        rb.linearVelocity = Vector2.zero;
        transform.position = targetPosition; 

        // Bu bir ara noktaya ulaştı, FollowPathCoroutine devam edecek.
    }
}