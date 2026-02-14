using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySpellSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(Spell spell)
    {
        // Set the icon and any other UI elements based on the spell data
        GetComponent<SpellHotbarSlot>().SetSpell(spell);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // show tooltip with spell information
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // hide tooltip with spell information
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
