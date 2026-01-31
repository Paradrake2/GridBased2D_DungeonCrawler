using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpellSelectorObject : MonoBehaviour
{
    public SpellTemplate containedTemplate;
    public Image icon;
    public TextMeshProUGUI templateNameText;
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    public void Initialize(SpellTemplate template, SpellCrafterUI ui)
    {
        spellCrafterUI = ui;
        SetTemplate(template);
    }
    public void SetTemplate(SpellTemplate template)
    {
        containedTemplate = template;
        icon.sprite = template.icon;
        templateNameText.text = template.templateName;
    }
    public void OnClick()
    {
        Debug.Log("Selected spell template: " + containedTemplate.templateName);
        // Here you can add logic to handle what happens when the spell template is selected
        spellCrafterUI.SelectSpellTemplate(containedTemplate);
    }
}
