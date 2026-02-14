using System.Collections.Generic;
using UnityEngine;

public class SpellHotbar : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int numberOfSlots = 10;
    public Dictionary<int, Spell> hotbarSpells = new Dictionary<int, Spell>();
    public void CreateSlots()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            SpellHotbarSlot slotComponent = slot.GetComponent<SpellHotbarSlot>();
            if (slotComponent != null)
            {
                slotComponent.InstantiateSlot(this, i);
            }
        }
    }
    public void AssignSpellToSlot(int slotIndex, Spell spell)
    {
        hotbarSpells[slotIndex] = spell;
        SpellHotbarSlot slot = transform.GetChild(slotIndex).GetComponent<SpellHotbarSlot>();
        if (slot != null)
        {
            slot.SetSpell(spell);
        }
    }
    void Start()
    {
        CreateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
