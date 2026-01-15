using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Player player;
    public Transform inventoryParent;
    public GameObject itemSlotPrefab;
    public GameObject equipmentSlotPrefab;

    public void PopulateItemInventory() // crafting potions
    {
        ClearChildren(inventoryParent);
        foreach (InventoryItemSlot slot in inventory.items)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryParent);
            InventoryItemSlotUI slotUI = newSlot.GetComponent<InventoryItemSlotUI>();
            slotUI.SetItemIcon(slot.item.GetIcon());
            slotUI.SetQuantityText(slot.quantity);
            slotUI.SetItem(slot.item);
        }
    }
    public void PopulateEquipmentCraftingItemInventory() // augmenting equipment
    {
        ClearChildren(inventoryParent);
        foreach (InventoryItemSlot slot in inventory.items)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryParent);
            InventoryItemSlotUI slotUI = newSlot.GetComponent<InventoryItemSlotUI>();
            slotUI.SetItemIcon(slot.item.GetIcon());
            slotUI.SetQuantityText(slot.quantity);
            slotUI.SetItem(slot.item);
            slotUI.SetEquipmentCraftingMode(true);
        }
    }

    public void PopulateEquipmentInventory()
    {
        ClearChildren(inventoryParent);
        foreach (Equipment equipment in inventory.storedEquipment)
        {
            GameObject newSlot = Instantiate(equipmentSlotPrefab, inventoryParent);
            InventoryEquipmentSlotUI slotUI = newSlot.GetComponent<InventoryEquipmentSlotUI>();
            slotUI.Initialize(this);
            slotUI.icon.sprite = equipment.equipmentIcon;
            slotUI.SetEquipment(equipment);
            slotUI.SetCraftingMode(false);
        }
    }
    public void PopulateCraftingEquipmentInventory()
    {
        ClearChildren(inventoryParent);
        foreach (Equipment equipment in inventory.storedEquipment)
        {
            GameObject newSlot = Instantiate(equipmentSlotPrefab, inventoryParent);
            InventoryEquipmentSlotUI slotUI = newSlot.GetComponent<InventoryEquipmentSlotUI>();
            slotUI.Initialize(this);
            slotUI.icon.sprite = equipment.equipmentIcon;
            slotUI.SetEquipment(equipment);
            slotUI.SetCraftingMode(true);
        }
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    public void InitializeInventory()
    {
        inventory = FindAnyObjectByType<Inventory>();
        player = FindAnyObjectByType<Player>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found in the scene.");
            return;
        }
        ClearChildren(inventoryParent);
    }
    void Start()
    {
        InitializeInventory();
        //PopulateItemInventory();
        PopulateEquipmentInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
