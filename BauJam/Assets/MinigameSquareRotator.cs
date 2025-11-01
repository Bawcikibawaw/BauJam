using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSquareRotator : MonoBehaviour
{
    [Header("Otomatik Dönüþ Ayarlarý")]
    [Tooltip("Karenin hedefe doðru dönme hýzý (derece/saniye)")]
    public float rotationSpeed = 180f;

    [Tooltip("Karenin döneceði Z açýlarýnýn listesi. Örneðin: -5, 10, -25, 40")]
    public List<float> targetAngles;

    // --- YENÝ EKLENEN KISIM BAÞLANGICI ---
    [Header("Oyuncu Kontrol Ayarlarý")]
    [Tooltip("Oyuncunun A/D tuþlarý ile uygulayacaðý dönüþ hýzý")]
    public float manualRotationSpeed = 200f;
    // --- YENÝ EKLENEN KISIM SONU ---

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
            Debug.LogError("Hedef açý listesi boþ! Lütfen Inspector'dan doldurun.");
            this.enabled = false;
        }
    }

    void Update()
    {
        // 1. OTOMATÝK DÖNÜÞ BÖLÜMÜ (Mevcut kod)
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

        // --- YENÝ EKLENEN KISIM BAÞLANGICI ---
        // 2. MANUEL OYUNCU KONTROLÜ BÖLÜMÜ
        HandlePlayerInput();
        // --- YENÝ EKLENEN KISIM SONU ---
    }

    // --- YENÝ EKLENEN KISIM BAÞLANGICI ---
    void HandlePlayerInput()
    {
        // Eðer 'A' tuþuna basýlý tutuluyorsa
        if (Input.GetKey(KeyCode.A))
        {
            // Sola doðru (pozitif Z yönü) döndür
            rectTransform.Rotate(0, 0, manualRotationSpeed * Time.deltaTime);
        }
        // Eðer 'D' tuþuna basýlý tutuluyorsa
        else if (Input.GetKey(KeyCode.D))
        {
            // Saða doðru (negatif Z yönü) döndür
            rectTransform.Rotate(0, 0, -manualRotationSpeed * Time.deltaTime);
        }
    }
    // --- YENÝ EKLENEN KISIM SONU ---

    void SetNextTarget()
    {
        if (currentTargetIndex >= targetAngles.Count)
        {
            Debug.Log("Tüm hedeflere ulaþýldý. Obje yok ediliyor.");
            Destroy(gameObject);
            return;
        }

        float nextTargetAngle = targetAngles[currentTargetIndex];
        if (Mathf.Abs(nextTargetAngle) >= 90)
        {
            Debug.Log("Hedef açý (" + nextTargetAngle + ") sýnýra ulaþtý veya geçti. Obje yok ediliyor.");
            Destroy(gameObject);
        }
    }
}