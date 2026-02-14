using UnityEngine;
using TMPro;
public class SpellDescriptionManager : MonoBehaviour
{
    public SpellCrafterUI spellCrafterUI;
    public TextMeshProUGUI spellDescriptionText;
    public void SetSpellDescription(bool meetsRequirements, SpellComposition composition)
    {
       // if (template == null || !meetsRequirements)
        //{
         //   spellDescriptionText.text = "No Spell Selected";
          //  return;
        //}
        if (!meetsRequirements && composition != null)
        {
            spellDescriptionText.text = "";
            foreach (var req in composition.requirements)
            {
                int count = composition.components.FindAll(c => c.ComponentType == req.requiredType).Count;
                spellDescriptionText.text += $"{req.requiredType}: {count}/{req.minimumCount}\n";
            }
            return;
        }
        if (composition.numOfCoresValid == false)
        {
            spellDescriptionText.text = "Invalid number of Core components. Exactly one Core is required.\n";
            return;
        }
        if (composition.numOfCostComponentsValid == false)
        {
            spellDescriptionText.text = "Too many Cost components for the selected Core.\n";
            return;
        }
        if (composition.CalculateSpellCost() > GameObject.FindFirstObjectByType<Player>().GetMagic())
        {
            spellDescriptionText.text = $"Not enough magic power to cast this spell. Required: {composition.CalculateSpellCost()}";
            return;
        }
        if (composition != null && meetsRequirements)
        {
            spellDescriptionText.text = "Spell is valid and ready to generate!";
            Spell spellPreview = spellCrafterUI.GetSpellCrafter().CreateSpell(composition);
            spellDescriptionText.text += $"\nDamage Multiplier: {spellPreview.spellEffect.GetDamageMult()}\n";
            spellDescriptionText.text += $"Heal Amount: {spellPreview.spellEffect.GetHealAmount()}\n";
            spellDescriptionText.text += $"Duration: {spellPreview.spellEffect.GetDuration()} seconds\n";
            spellDescriptionText.text += $"Cost Amount: {spellPreview.spellEffect.GetCostAmount()}\n";
            spellDescriptionText.text += $"Magic Cost: {composition.CalculateSpellCost()}\n";
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
