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
        componentText.text = component.ComponentName;
    }
    public void OnClick()
    {
        Debug.Log("Clicked on component: " + spellComponent.ComponentName);
        spellCrafterUI.SetSelectedCellComponent(spellComponent);
        // Here you can add logic to handle what happens when the component is clicked
    }
}
