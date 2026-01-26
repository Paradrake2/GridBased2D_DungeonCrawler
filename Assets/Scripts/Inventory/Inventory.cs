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
    public List<Potion> storedPotions = new List<Potion>();

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
    public void AddPotion(Potion newPotion)
    {
        storedPotions.Add(newPotion);
    }
    public void RemovePotion(Potion potionToRemove)
    {
        if (storedPotions.Contains(potionToRemove))
        {
            storedPotions.Remove(potionToRemove);
        }
        else
        {
            // Potion not found in inventory
            Debug.LogWarning("Attempted to remove potion not in inventory: " + potionToRemove.potionName);
        }
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
    public void RemoveEquipment(Equipment equipmentToRemove)
    {
        if (storedEquipment.Contains(equipmentToRemove))
        {
            storedEquipment.Remove(equipmentToRemove);
        }
        else
        {
            // Equipment not found in inventory
            Debug.LogWarning("Attempted to remove equipment not in inventory: " + equipmentToRemove.equipmentName);
        }
    }
    public void RemoveEquipmentByID(string equipmentID)
    {
        Equipment equipmentToRemove = storedEquipment.Find(equip => equip.GetID() == equipmentID);
        if (equipmentToRemove != null)
        {
            storedEquipment.Remove(equipmentToRemove);
        }
        else
        {
            // Equipment not found in inventory
            Debug.LogWarning("Attempted to remove equipment not in inventory with ID: " + equipmentID);
        }
    }
    void Start()
    {
        foreach (Equipment equipment in storedEquipment)
        {
            if (equipment.GetID() == null || equipment.GetID() == "")
            {
                equipment.SetID();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
