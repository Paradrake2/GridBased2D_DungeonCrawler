using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellDFComponentListObject : MonoBehaviour
{
    public SpellComponent selectedSpellComponent;
    public DFComponentGroup dFComponentGroup;
    private SpellCrafterUI spellCrafterUI;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI componentText;
    [SerializeField] private int index;

    public void Initialize(DFComponentGroup group)
    {
        dFComponentGroup = group;
        index = 0;

        if (dFComponentGroup == null || dFComponentGroup.components == null || dFComponentGroup.components.Length == 0)
        {
            selectedSpellComponent = null;
            return;
        }

        selectedSpellComponent = dFComponentGroup.components[index].component;
        if (icon != null)
        {
            icon.sprite = dFComponentGroup.GetIcon();
        }
        if (componentText != null)
        {
            componentText.text = dFComponentGroup.GetGroupName();
        }
    }

    public void OnClick()
    {
        Debug.Log("Clicked on DF component group: " + dFComponentGroup.GetGroupName());

        if (spellCrafterUI == null) return;

        spellCrafterUI.SetSelectedCellComponent(selectedSpellComponent);
        spellCrafterUI.SetDFComponentGroup(dFComponentGroup);
    }

    void Start()
    {
        spellCrafterUI = FindAnyObjectByType<SpellCrafterUI>();
    }
}
