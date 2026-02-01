using UnityEngine;
using TMPro;
public class SpellDescriptionManager : MonoBehaviour
{
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
