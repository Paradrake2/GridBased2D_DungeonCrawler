using System.Collections.Generic;
using UnityEngine;

public class SpellHotbar : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int numberOfSlots = 10;
    public Dictionary<int, Spell> hotbarSpells = new Dictionary<int, Spell>();

    public bool TryAssignSpellToFirstEmptySlot(Spell spell)
    {
        if (spell == null) return false;

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (!hotbarSpells.TryGetValue(i, out var existing) || existing == null)
            {
                AssignSpellToSlot(i, spell);
                return true;
            }
        }
        return false;
    }
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
        if (slotIndex < 0 || slotIndex >= transform.childCount) return;
        SpellHotbarSlot slot = transform.GetChild(slotIndex).GetComponent<SpellHotbarSlot>();
        if (slot != null) slot.SetSpell(spell);
    }
    void Start()
    {
        CreateSlots();

        // If the inventory already has spells (e.g., crafted before this UI was opened),
        // populate them into the hotbar without requiring SpellCaster hacks.
        if (Inventory.Instance != null)
        {
            foreach (var spell in Inventory.Instance.GetSpellList())
                TryAssignSpellToFirstEmptySlot(spell);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
