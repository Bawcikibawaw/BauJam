using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))] 
public class NPCController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TilePathfinder pathfinder; // TilePathfinder'a referans
    
    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 3f; 
    
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
            // GameManager'daki event'e abone ol
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
    
    // GameManager'dan gelen event ile çağrılan fonksiyon
    public void StartMovementTo(Vector3 targetWorldPosition)
    {
        StopAllCoroutines(); 

        // 1. PathFinder'dan yolu bulmasını iste
        currentPath = pathfinder.FindPath(transform.position, targetWorldPosition);
        currentPathIndex = 0;

        // 2. Yolu bulduysa hareketi başlat
        if (currentPath.Count > 0)
        {
            StartCoroutine(FollowPathCoroutine());
        }
        else
        {
            Debug.LogWarning("Yol bulunamadı! Engel veya Tilemap ayarlarını kontrol edin.");
        }
    }

    private IEnumerator FollowPathCoroutine()
    {
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPosition = currentPath[currentPathIndex];
            
            // Tek bir Waypoint'e (hücre merkezine) yürü
            yield return StartCoroutine(MoveToSingleTarget(targetPosition));
            
            currentPathIndex++;
        }
        
        Debug.Log("NPC nihai hedefe ulaştı.");
    }

    private IEnumerator MoveToSingleTarget(Vector3 targetPosition)
    {
        float stopDistance = 0.05f; 
        while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        transform.position = targetPosition;
    }
}