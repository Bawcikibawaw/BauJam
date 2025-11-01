using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))] 
public class NPCController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TilePathfinder pathfinder;
    
    // 🚨 YENİ: Animator bileşeni referansı
    private Animator animator; 


    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private float stopDistance = 0.05f; 


    [Header("Duruş Ayarları")]
    [Tooltip("NPC'nin NİHAİ hedefine ulaştıktan sonra, yeni hedef seçilmeden önce bekleyeceği süre (saniye).")]
    [SerializeField] private float finalPauseDuration = 1.0f; // Nihai duruş gecikmesi


    private List<Vector3> currentPath = new List<Vector3>();
    private int currentPathIndex = 0;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        
        // 🚨 YENİ: Animator bileşenini al
        animator = GetComponent<Animator>(); 


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

        // Not: Burada FindPath'in 2 parametreli versiyonu kullanılıyor.
        currentPath = pathfinder.FindPath(transform.position, targetWorldPosition); 
        currentPathIndex = 0;

        if (currentPath.Count > 0)
        {
            StartCoroutine(FollowPathCoroutine());
        }
        else
        {
            Debug.LogWarning("Yol bulunamadı! Engel veya Tilemap ayarlarını kontrol edin.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isNPCMoving = false;
            }
        }
    }


    private IEnumerator FollowPathCoroutine()
    {
        // 1. Ara Noktaları Takip Et
        
        // 🚨 YENİ: Yürümeyi başlat
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPosition = currentPath[currentPathIndex];
            
            yield return StartCoroutine(MoveToSingleTarget(targetPosition));
            
            currentPathIndex++;
        }
        
        Debug.Log("NPC nihai hedefe ulaştı. Yeni hedef seçimi için bekleniyor...");
        
        // 🚨 YENİ: Hedefe ulaşılınca animasyonu durdur
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            // Durduğu yöne bakması için son yön değerleri korunur
        }
        
        // NİHAİ HEDEF GECİKMESİ
        yield return new WaitForSeconds(finalPauseDuration); 
        
        // İşlem Tamamlandı Bildirimi
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNPCMoving = false; 
        }

    }


    private IEnumerator MoveToSingleTarget(Vector3 targetPosition)
    {
        // 1. Hedefe yürüme döngüsü
        while (Vector3.Distance(transform.position, targetPosition) > stopDistance) 
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            
            // 🚨 ANİMASYON GÜNCELLEME: Yürüme yönünü Animator'a gönder
            if (animator != null)
            {
                 animator.SetFloat("moveX", direction.x);
                 animator.SetFloat("moveY", direction.y);
            }
            
            // HAREKET KODU (Orijinal haliyle bırakıldı)
            rb.linearVelocity = direction * moveSpeed; 
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime); 
            yield return null;
        }
        
        // 2. Hedefe ulaşıldı
        rb.linearVelocity = Vector2.zero; // Anında durdur
        transform.position = targetPosition; // Hedefe tam kilitleme

        // Bu bir ara nokta olduğu için burada bekleme YOK.
    }
}