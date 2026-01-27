using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsShower : MonoBehaviour
{
    [SerializeField] private Image statIcon;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private StatType stat;
    private void SetStatIcon(Sprite icon)
    {
        statIcon.sprite = icon;
    }
    private void SetStatValueText(string value)
    {
        statValueText.text = $"{stat.displayName}: {value}";
        statValueText.color = stat.displayColor;
    }
    public void SetCustomText(string text, Color color)
    {
        statValueText.text = text;
        statValueText.color = color;
    }
    public StatType GetStatType()
    {
        return stat;
    }
    public string GetStatValueText()
    {
        return statValueText.text;
    }
    public Sprite GetStatIcon()
    {
        return statIcon.sprite;
    }
    public void Initialize(StatType stat, string value)
    {
        this.stat = stat;
        if(stat.icon != null) SetStatIcon(stat.icon);
        SetStatValueText(value);
    }
}
