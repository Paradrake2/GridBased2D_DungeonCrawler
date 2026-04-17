using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the player's Health, Mana, XP and Gold in a persistent HUD.
///
/// Unity setup:
///   1. Create a Screen Space - Overlay Canvas (or attach to an existing one).
///   2. Add a child GameObject with this script.
///   3. Build the following UI children and wire them up in the Inspector:
///
///   Health bar  — a UI Slider  (healthBar)
///   Mana bar    — a UI Slider  (manaBar)
///   XP bar      — a UI Slider  (xpBar)
///   Health text — a TMP label  e.g. "42 / 100"  (healthText)
///   Mana text   — a TMP label  e.g. "30 / 50"   (manaText)
///   XP text     — a TMP label  e.g. "250 / 500"  (xpText)
///   Gold text   — a TMP label  e.g. "Gold: 120"  (goldText)
///
///   All fields are optional — leave any unassigned to skip that element.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats playerStats;

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;

    [Header("Mana")]
    [SerializeField] private Slider manaBar;
    [SerializeField] private TMP_Text manaText;

    [Header("XP")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text xpText;

    [Header("Gold")]
    [SerializeField] private TMP_Text goldText;

    private IEnumerator Start()
    {
        if (player == null)
            player = FindAnyObjectByType<Player>();

        if (playerStats == null)
            playerStats = FindAnyObjectByType<PlayerStats>();

        if (player != null)
        {
            player.OnHealthChanged += RefreshHealth;
            player.OnManaChanged += RefreshMana;
        }

        if (playerStats != null)
        {
            playerStats.OnGoldChanged += RefreshGold;
            playerStats.OnXPChanged += RefreshXP;
        }

        // Wait one frame so all other Start() methods (including Player.Initialize) have run
        yield return null;

        // Sliders must not be interactable — otherwise Unity's EventSystem forwards
        // WASD/arrow key input directly to them, making them visually change on player movement.
        if (healthBar != null) healthBar.interactable = false;
        if (manaBar != null) manaBar.interactable = false;
        if (xpBar != null) xpBar.interactable = false;

        Refresh();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= RefreshHealth;
            player.OnManaChanged -= RefreshMana;
        }

        if (playerStats != null)
        {
            playerStats.OnGoldChanged -= RefreshGold;
            playerStats.OnXPChanged -= RefreshXP;
        }
    }

    private void Refresh()
    {
        RefreshHealth();
        RefreshMana();
        RefreshXP();
        RefreshGold();
    }

    private void RefreshHealth()
    {
        if (player == null) return;
        float currentHP = player.GetHealth();
        float maxHP = player.GetMaxHealth();
        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHP > 0f ? maxHP : 1f;
            healthBar.value = currentHP;
        }
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(maxHP)}";
    }

    private void RefreshMana()
    {
        if (player == null || playerStats == null) return;
        int currentMana = player.GetMana();
        int maxMana = playerStats.GetMana();
        if (manaBar != null)
        {
            manaBar.minValue = 0f;
            manaBar.maxValue = maxMana > 0 ? maxMana : 1f;
            manaBar.value = currentMana;
        }
        if (manaText != null)
            manaText.text = $"{currentMana} / {maxMana}";
    }

    private void RefreshXP()
    {
        if (playerStats == null) return;
        float currentXP = playerStats.GetCurrentExperience();
        float xpToNext = playerStats.GetExperienceToNextLevel();
        if (xpBar != null)
        {
            xpBar.minValue = 0f;
            xpBar.maxValue = xpToNext > 0f ? xpToNext : 1f;
            xpBar.value = currentXP;
        }
        if (xpText != null)
            xpText.text = $"XP: {Mathf.FloorToInt(currentXP)} / {Mathf.FloorToInt(xpToNext)}";
    }

    private void RefreshGold()
    {
        if (playerStats == null) return;
        if (goldText != null)
            goldText.text = $"Gold: {playerStats.GetGoldAmount()}";
    }
}
