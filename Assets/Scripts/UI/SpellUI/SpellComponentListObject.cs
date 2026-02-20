using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellComponentListObject : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI componentText;
    public SpellComponent spellComponent;
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    public void Initialize(SpellComponent component, SpellCrafterUI ui)
    {
        spellComponent = component;
        spellCrafterUI = ui;
        icon.sprite = component.Icon;
        componentText.text = $"{component.ComponentName} - Type: {component.ComponentType}";
    }
    public void OnClick()
    {
        Debug.Log("Clicked on component: " + spellComponent.ComponentName);
        // add neccessary ingredients to storage
        spellCrafterUI.SetSelectedCellComponent(spellComponent);
        spellCrafterUI.UpdateComponentPreview(spellComponent);
        
    }
}
