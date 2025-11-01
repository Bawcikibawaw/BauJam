using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSquareRotator : MonoBehaviour
{
    [Header("Otomatik D�n�� Ayarlar�")]
    [Tooltip("Karenin hedefe do�ru d�nme h�z� (derece/saniye)")]
    public float rotationSpeed = 180f;

    [Tooltip("Karenin d�nece�i Z a��lar�n�n listesi. �rne�in: -5, 10, -25, 40")]
    public List<float> targetAngles;

    // --- YEN� EKLENEN KISIM BA�LANGICI ---
    [Header("Oyuncu Kontrol Ayarlar�")]
    [Tooltip("Oyuncunun A/D tu�lar� ile uygulayaca�� d�n�� h�z�")]
    public float manualRotationSpeed = 200f;
    // --- YEN� EKLENEN KISIM SONU ---

    private RectTransform rectTransform;
    private int currentTargetIndex = 0;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (targetAngles.Count > 0)
        {
            SetNextTarget();
        }
        else
        {
            Debug.LogError("Hedef a�� listesi bo�! L�tfen Inspector'dan doldurun.");
            this.enabled = false;
        }
    }

    void Update()
    {
        // 1. OTOMAT�K D�N�� B�L�M� (Mevcut kod)
        if (currentTargetIndex < targetAngles.Count)
        {
            float targetZ = targetAngles[currentTargetIndex];
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);

            rectTransform.rotation = Quaternion.RotateTowards(
                rectTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(rectTransform.rotation, targetRotation) < 0.1f)
            {
                currentTargetIndex++;
                SetNextTarget();
            }
        }

        // --- YEN� EKLENEN KISIM BA�LANGICI ---
        // 2. MANUEL OYUNCU KONTROL� B�L�M�
        HandlePlayerInput();
        // --- YEN� EKLENEN KISIM SONU ---
    }

    // --- YEN� EKLENEN KISIM BA�LANGICI ---
    void HandlePlayerInput()
    {
        // E�er 'A' tu�una bas�l� tutuluyorsa
        if (Input.GetKey(KeyCode.A))
        {
            // Sola do�ru (pozitif Z y�n�) d�nd�r
            rectTransform.Rotate(0, 0, manualRotationSpeed * Time.deltaTime);
        }
        // E�er 'D' tu�una bas�l� tutuluyorsa
        else if (Input.GetKey(KeyCode.D))
        {
            // Sa�a do�ru (negatif Z y�n�) d�nd�r
            rectTransform.Rotate(0, 0, -manualRotationSpeed * Time.deltaTime);
        }
    }
    // --- YEN� EKLENEN KISIM SONU ---

    void SetNextTarget()
    {
        if (currentTargetIndex >= targetAngles.Count)
        {
            Debug.Log("T�m hedeflere ula��ld�. Obje yok ediliyor.");
            Destroy(gameObject);
            return;
        }

        float nextTargetAngle = targetAngles[currentTargetIndex];
        if (Mathf.Abs(nextTargetAngle) >= 90)
        {
            Debug.Log("Hedef a�� (" + nextTargetAngle + ") s�n�ra ula�t� veya ge�ti. Obje yok ediliyor.");
            Destroy(gameObject);
        }
    }
}