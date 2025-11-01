using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scripts", menuName = "Scriptable Objects/Card")]
public class PainSO : ScriptableObject
{
    public int painLevel;
    public int manaRequirement;
    public string disclaimer;
    public Sprite card;
}
