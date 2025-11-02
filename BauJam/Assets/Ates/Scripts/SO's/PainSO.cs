using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scripts", menuName = "Scriptable Objects/Card")]
public class PainSO : ScriptableObject
{
    public int painLevel;
    public int manaRequirement;
    public string disclaimer;
    public Sprite card;
    
    [Header("Kısıtlama Kontrolü")]
    [Tooltip("Bu kart, diğerleri alınana kadar kilitli kalacak son kart mıdır?")]
    public bool isFinalCard = false; // <-- BU SATIRI EKLEYİN
}
