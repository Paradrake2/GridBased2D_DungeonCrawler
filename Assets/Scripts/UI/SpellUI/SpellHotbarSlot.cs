using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpellHotbarSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private SpellHotbar hotbar;

    [Header("Cooldown UI (optional)")]
    [SerializeField] private Image cooldownOverlay;  // a filled Image (fill method: Radial360 or Vertical) set to 0 normally
    [SerializeField] private TMP_Text cooldownText;  // shows remaining seconds

    public Spell containedSpell;
    public void OnClick()
    {
        if (containedSpell != null)
        {
            containedSpell.CastSpell();
        }
    }
    public void SetSpell(Spell spell)
    {
        containedSpell = spell;
        UpdateUI();
    }
    public void ClearSpell()
    {
        containedSpell = null;
        UpdateUI();
    }
    public void InstantiateSlot(SpellHotbar hotbar, int index)
    {
        this.hotbar = hotbar;
        this.slotIndex = index;
    }
    void UpdateUI()
    {
        if (containedSpell != null)
        {
            iconImage.sprite = containedSpell.GetIcon();
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
    void Start()
    {
        button.onClick.AddListener(OnClick);
        UpdateUI();
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        if (containedSpell == null || containedSpell.spellEffect == null)
        {
            SetCooldownVisible(false);
            return;
        }

        bool onCooldown = containedSpell.spellEffect.IsOnCooldown();
        SetCooldownVisible(onCooldown);

        if (onCooldown)
        {
            float remaining = containedSpell.spellEffect.GetCooldownRemaining();
            float total = containedSpell.spellEffect.GetCooldownDuration();

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = total > 0f ? remaining / total : 0f;

            if (cooldownText != null)
                cooldownText.text = remaining > 0.95f ? Mathf.CeilToInt(remaining).ToString() : "";
        }
    }

    private void SetCooldownVisible(bool visible)
    {
        if (cooldownOverlay != null) cooldownOverlay.gameObject.SetActive(visible);
        if (cooldownText != null) cooldownText.gameObject.SetActive(visible);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Tooltip not implemented yet; keep safe.
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Tooltip not implemented yet; keep safe.
    }
}
