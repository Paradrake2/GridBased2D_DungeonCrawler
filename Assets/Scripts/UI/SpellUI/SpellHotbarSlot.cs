using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellHotbarSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private SpellHotbar hotbar;
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

    // Update is called once per frame
    void Update()
    {
        
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
