using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipmentSlotUI : MonoBehaviour
{
    public Image icon;
    public Button button;
    private Equipment containedEquipment;
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] EquipmentUIManager equipmentUIManager;
    [SerializeField] InventoryUI inventoryUI;
    public void OnClick()
    {
        equipmentManager.EquipItem(containedEquipment);
        equipmentUIManager.UpdateEquipmentUISlot(containedEquipment, false);
        inventoryUI.PopulateEquipmentInventory();
        Debug.Log("Inventory equipment slot clicked: " + icon.sprite.name);
    }
    public void SetEquipment(Equipment equipment)
    {
        containedEquipment = equipment;
    }
    public void Initialize(InventoryUI inventoryUI)
    {
        equipmentManager = FindAnyObjectByType<EquipmentManager>();
        equipmentUIManager = FindAnyObjectByType<EquipmentUIManager>();
        this.inventoryUI = inventoryUI;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
