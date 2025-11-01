using TMPro;
using UnityEngine;

public class ButtonHoverColor : MonoBehaviour
{
    public TMP_Text text;

    public void SetRed()
    {
        text.color = Color.red;
    }

    public void SetWhite()
    {
        text.color = Color.white;
    }
}
