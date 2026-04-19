using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Enemy Info")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text weaknessText;
    [SerializeField] private TMP_Text attributeDamageText;
    [SerializeField] private TMP_Text attributeDefenseText;

    [Header("Flee")]
    [SerializeField] private Button fleeButton;

    private PlayerCombat playerCombat;
    private Enemy currentEnemy;

    private void Awake()
    {
        if (fleeButton != null)
            fleeButton.onClick.AddListener(OnFleeClicked);
    }

    public void Show(Enemy enemy)
    {
        currentEnemy = enemy;
        if (playerCombat == null)
            playerCombat = FindAnyObjectByType<PlayerCombat>();
        if (panel != null) panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        currentEnemy = null;
        if (panel != null) panel.SetActive(false);
    }

    private void Update()
    {
        if (currentEnemy != null)
            RefreshHealth();
    }

    private void Refresh()
    {
        if (currentEnemy == null) return;

        var esh = currentEnemy.stats.esh;

        if (enemyNameText != null)
            enemyNameText.text = currentEnemy.stats.enemyName;

        RefreshHealth();

        if (defenseText != null)
            defenseText.text = $"Defense: {esh.defense}";

        if (damageText != null)
            damageText.text = $"Damage: {esh.damage}";

        if (attackSpeedText != null)
        {
            var atkSpdStat = StatDatabase.Instance != null ? StatDatabase.Instance.GetStat("AttackSpeed") : null;
            float atkSpd = atkSpdStat != null ? esh.stats.GetStat(atkSpdStat) : 0f;
            attackSpeedText.text = $"Attack Speed: {atkSpd}";
        }

        if (weaknessText != null)
        {
            if (esh.weakness != null && esh.weakness.attribute != null)
            {
                weaknessText.text = $"Weakness: {esh.weakness.attribute.displayName} (x{esh.weakness.multiplier})";
                weaknessText.color = esh.weakness.attribute.displayColor;
            }
            else
                weaknessText.text = "Weakness: None";
        }

        if (attributeDamageText != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var attr in esh.enemyAttributesList)
            {
                if (attr.attackAttribute != null && attr.attackAttributeValue != 0f)
                    sb.AppendLine($"{attr.attackAttribute.displayName}: {attr.attackAttributeValue}");
            }
            attributeDamageText.text = sb.Length > 0 ? sb.ToString().TrimEnd() : "None";
        }

        if (attributeDefenseText != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var attr in esh.enemyAttributesList)
            {
                if (attr.defenseAttribute != null && attr.defenseAttributeValue != 0f)
                    sb.AppendLine($"{attr.defenseAttribute.displayName}: {attr.defenseAttributeValue}");
            }
            attributeDefenseText.text = sb.Length > 0 ? sb.ToString().TrimEnd() : "None";
        }
    }

    private void RefreshHealth()
    {
        if (currentEnemy == null) return;

        float current = currentEnemy.stats.currentHealth;
        float max = currentEnemy.stats.esh.maxHealth;

        if (healthBar != null)
        {
            healthBar.interactable = false;
            healthBar.minValue = 0f;
            healthBar.maxValue = max > 0f ? max : 1f;
            healthBar.value = current;
        }

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void OnFleeClicked()
    {
        if (playerCombat != null)
            playerCombat.FleeCombat();
    }
}

