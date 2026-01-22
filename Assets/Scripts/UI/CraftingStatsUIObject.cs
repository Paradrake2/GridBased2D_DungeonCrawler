using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CraftingStatsUIObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private Image statIcon;
    public void SetStatIcon(Sprite icon)
    {
        statIcon.sprite = icon;
    }
    public void SetStatText(string text)
    {
        statText.text = text;
    }
    public void Initialize(StatType stat, float value)
    {
        SetStatIcon(stat.icon);
        SetStatText(stat.displayName + ": " + value.ToString());
        statText.color = stat.displayColor;
    }
}
