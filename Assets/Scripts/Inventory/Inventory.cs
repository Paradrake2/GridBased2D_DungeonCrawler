using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemSlot
{
    public Item item;
    public int quantity;
    public Item GetItem()
    {
        return item;
    }
    public int GetQuantity()
    {
        return quantity;
    }
}


public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public EquipmentManager equipmentManager;
    public List<InventoryItemSlot> items = new List<InventoryItemSlot>();
    public List<Equipment> storedEquipment = new List<Equipment>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        equipmentManager = GetComponent<EquipmentManager>();
    }
    public void AddItem(Item newItem, int quantity)
    {
        InventoryItemSlot existingSlot = items.Find(slot => slot.item == newItem);
        if (existingSlot != null)
        {
            existingSlot.quantity += quantity;
        }
        else
        {
            InventoryItemSlot newSlot = new InventoryItemSlot
            {
                item = newItem,
                quantity = quantity
            };
            items.Add(newSlot);
        }
    }
    public void AddEquipment(Equipment newEquipment)
    {
        storedEquipment.Add(newEquipment);
    }
    public void RemoveItem(Item itemToRemove, int quantity)
    {
        InventoryItemSlot existingSlot = items.Find(slot => slot.item == itemToRemove);
        if (existingSlot != null)
        {
            existingSlot.quantity -= quantity;
            if (existingSlot.quantity <= 0)
            {
                items.Remove(existingSlot);
            }
        }
        else
        {
            // Item not found in inventory
            Debug.LogWarning("Attempted to remove item not in inventory: " + itemToRemove.name);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
