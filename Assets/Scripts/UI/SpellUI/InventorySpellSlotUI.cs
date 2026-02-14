using UnityEngine;

public class InventorySpellSlotUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(Spell spell)
    {
        // Set the icon and any other UI elements based on the spell data
        GetComponent<SpellHotbarSlot>().SetSpell(spell);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
