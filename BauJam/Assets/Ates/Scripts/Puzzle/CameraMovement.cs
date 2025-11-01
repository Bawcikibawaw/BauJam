using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Kameranın hareket hızı.")]
    public float moveSpeed = 5f;

    [Header("Sınır Ayarları (Dünya Koordinatları)")]
    [Tooltip("Kameranın gidebileceği en düşük X konumu.")]
    public float minX = -10f; 
    [Tooltip("Kameranın gidebileceği en yüksek X konumu.")]
    public float maxX = 10f;
    [Tooltip("Kameranın gidebileceği en düşük Y konumu.")]
    public float minY = -5f;
    [Tooltip("Kameranın gidebileceği en yüksek Y konumu.")]
    public float maxY = 5f;

    void Update()
    {
        // 1. Girdi Alma (WASD)
        // Unity'nin varsayılan Input Manager'ındaki "Horizontal" ve "Vertical" ayarlarını kullanır
        float inputX = Input.GetAxis("Horizontal"); // A (-1) veya D (1)
        float inputY = Input.GetAxis("Vertical");   // S (-1) veya W (1)

        // 2. Hareket Vektörünü Hesaplama
        // Hareket yönünü oluştur
        Vector3 movement = new Vector3(inputX, inputY, 0f);

        // Movement'ın uzunluğunu 1'de tutarak çapraz hareketlerde hızlanmayı engelle
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // 3. Konumu Hesaplama
        // DeltaTime ile kare hızından bağımsız, yumuşak hareket sağla
        Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;

        // 4. Konumu Sınırlandırma (Clamping)
        // Mathf.Clamp kullanarak yeni konumu belirlediğimiz min/max aralığında tut.
        float clampedX = Mathf.Clamp(newPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(newPosition.y, minY, maxY);

        // 5. Kamerayı Güncelleme
        // Kameranın Z konumu değişmemeli (2D/3D perspektifini korur)
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}