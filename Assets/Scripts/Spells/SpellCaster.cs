using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell selectedSpell;
    public void CastSpell()
    {
        Inventory inv = FindAnyObjectByType<Inventory>();
        List<Spell> spells = inv.GetSpellList();
        selectedSpell = spells[0];
        selectedSpell.CastSpell();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
