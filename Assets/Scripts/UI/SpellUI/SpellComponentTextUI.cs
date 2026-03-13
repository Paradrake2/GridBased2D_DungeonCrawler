using UnityEngine;
using TMPro;

// might not be used
public class SpellComponentTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string newText)
    {
        text.text = newText;
    }
    public void ClearText()
    {
        text.text = "";
    }
}
