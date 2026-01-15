using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftingSlot : MonoBehaviour
{
    public Equipment storedEquipment;
    public Image imageIcon;
    [SerializeField] private Sprite baseIcon;
    [SerializeField] private EquipmentCrafting equipmentCrafting;
    [SerializeField] private InventoryUI inventoryUI;
    public void SetEquipment(Equipment equipment)
    {
        storedEquipment = equipment;

        if (storedEquipment != null)
        {
            imageIcon.sprite = storedEquipment.equipmentIcon;
        }
        else
        {
            imageIcon.sprite = baseIcon;
        }
    }
    public void RemoveEquipment()
    {
        storedEquipment = null;
        imageIcon.sprite = baseIcon;
        inventoryUI.PopulateCraftingEquipmentInventory();
    }
    public void OnClick()
    {
        if (storedEquipment != null)
        {
            equipmentCrafting.ClearStoredEquipment();
            inventoryUI.PopulateCraftingEquipmentInventory();
            // also clear stats
        }
    }
    void Start()
    {
        
    }


}
