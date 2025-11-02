using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System;

public class CardDisplay : MonoBehaviour
{
    // 1. Şablonun üzerindeki UI Bileşenleri
    [Header("UI Elementleri")]
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI manaText;  
    public TextMeshProUGUI disclaimerText; 
    public Image cardImage;
    public Button purchaseButton; 
    
    // NOT: isFinalCard bool'u artık PainSO'dan çekilecektir.
    
    private PainSO currentCardData; 
    private int totalCardsInShop = 0; // Dükkandaki toplam kart sayısını tutar (Final dahil)

    void Start()
    {
        // Toplam kart sayısını GameManager'dan al (Sadece bir kez al)
        if (GameManager.Instance != null && GameManager.Instance.availableTargets != null)
        {
             // DİKKAT: totalCardsInShop, dükkandaki toplam kart sayısı olmalıdır.
             totalCardsInShop = GameManager.Instance.availableTargets.Count; 
        }
        
        // Başlangıç durumunu ayarla (Eğer Start'ta DisplayCard çağrılmıyorsa)
        // Eğer kart verisi DisplayCard ile daha sonra yükleniyorsa, bu satır opsiyoneldir.
        // UpdatePurchaseButtonState(); 
    }
    
    void OnEnable()
    {
        // Event'e abone ol (Başka bir kart satın alındığında durumu güncellemek için)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCardCountUpdated += UpdatePurchaseButtonState;
        }
        // Eğer DisplayCard metodundan sonra OnEnable çağrılırsa durumu güncelle
        UpdatePurchaseButtonState(); 
    }

    void OnDisable()
    {
        // Temizleme: Aboneliği kaldır
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCardCountUpdated -= UpdatePurchaseButtonState;
        }
    }


    public void DisplayCard(PainSO cardData)
    {
        currentCardData = cardData;
        
        if (cardData == null)
        {
            Debug.LogError("DisplayCard metoduna geçersiz (null) kart verisi gönderildi!");
            return;
        }

        // 2. UI Yükleme
        levelText.text = cardData.painLevel.ToString();
        manaText.text = cardData.manaRequirement.ToString();
        
        // Kartın metnini varsayılan olarak yükle
        disclaimerText.text = cardData.disclaimer; 

        // CardImage yükleme
        if (cardData.card != null) 
        {
              cardImage.sprite = cardData.card;
        }
        
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners(); 
            purchaseButton.onClick.AddListener(AttemptPurchase); 
        }
        
        // Veri yüklendikten sonra durumu güncelle
        UpdatePurchaseButtonState();
    }
    
    // Satın alma iznini kontrol eden ve butonu güncelleyen metot (Event tarafından çağrılır)
    private void UpdatePurchaseButtonState()
    {
        if (purchaseButton == null || GameManager.Instance == null || currentCardData == null) return;
        
        // KRİTİK KONTROL: Kontrolü SO'dan çekilen değere göre yap
        if (currentCardData.isFinalCard) 
        {
            // Eğer son kart ise, GameManager'dan izin al
            bool canPurchase = GameManager.Instance.CanPurchaseFinalCard(totalCardsInShop);
            
            purchaseButton.interactable = canPurchase;
            
            // Görsel geri bildirim
            if (!canPurchase)
            {
                disclaimerText.text = "Önce diğer tüm kartları almalısın!";
                // Butonu gri yapma
                if (purchaseButton.GetComponent<Image>() != null)
                {
                    purchaseButton.GetComponent<Image>().color = Color.gray;
                }
            }
            else
            {
                 // Kilit açıldı
                 disclaimerText.text = currentCardData.disclaimer;
                 if (purchaseButton.GetComponent<Image>() != null)
                 {
                    purchaseButton.GetComponent<Image>().color = Color.white; // Rengi normale döndür
                 }
            }
        }
        else
        {
            // Diğer kartlar için her zaman aktif
            purchaseButton.interactable = true;
            disclaimerText.text = currentCardData.disclaimer; 
            if (purchaseButton.GetComponent<Image>() != null)
            {
                purchaseButton.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void AttemptPurchase()
    {
        if (currentCardData == null) return;

        // 🚨 KRİTİK KONTROL: Satın alma lojiğini güvence altına alır
        if (currentCardData.isFinalCard) 
        {
            if (GameManager.Instance == null) return;
            
            // Final Kart ise ve kilitliyse, işlemi durdur.
            if (!GameManager.Instance.CanPurchaseFinalCard(totalCardsInShop))
            {
                Debug.LogWarning("Final Kartı kilitli! Satın alma engellendi.");
                UpdatePurchaseButtonState(); // Görsel durumu tekrar güvence altına al
                return; // Burası satın alma işlemini durduran yerdir.
            }
        }
        
        // Satın alma mantığı GameManager'a devredilir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BuyCard(currentCardData);
            
            // Event, tüm diğer kartların (final kart dahil) durumunu otomatik güncelleyecektir.
        }
    }
}