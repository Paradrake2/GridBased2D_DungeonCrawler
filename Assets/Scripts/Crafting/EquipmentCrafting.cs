using System.Collections.Generic;
using UnityEngine;

public class EquipmentCrafting : MonoBehaviour
{
    public Equipment storedEquipment;
    public CraftingStatsUI craftingStatsUI;
    public EquipmentCraftingUI equipmentCraftingUI;
    public InventoryUI inventoryUI;
    public Transform equipmentStatsParent;
    [SerializeField] private List<Item> addedIngredients = new List<Item>();
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject equipmentSlotPrefab;

    public void SetStoredEquipment(Equipment equipment)
    {
        if (storedEquipment != null)
        {
            inventory.AddEquipment(storedEquipment);
        }
        storedEquipment = equipment;
        equipmentSlotPrefab.GetComponent<EquipmentCraftingSlot>().SetEquipment(equipment);
        craftingStatsUI.GenerateStatsEquipment();
        inventory.RemoveEquipment(equipment);
        inventoryUI.PopulateEquipmentCraftingItemInventory();
        equipmentCraftingUI.GenerateIngredientsUI(storedEquipment);
    }
    public void ClearStoredEquipment()
    {
        if (storedEquipment != null)
        {
            inventory.AddEquipment(storedEquipment);
        }
        storedEquipment = null;
        equipmentSlotPrefab.GetComponent<EquipmentCraftingSlot>().RemoveEquipment();
        // clear ingredients
        ClearIngredients();
        inventoryUI.PopulateEquipmentCraftingItemInventory();
        equipmentCraftingUI.GenerateIngredientsUI(null);
    }
    // used when cancelling crafting or removing equipment
    public void ClearIngredients()
    {
        for (int i = addedIngredients.Count - 1; i >= 0; i--)
        {
            Item ingredient = addedIngredients[i];
            inventory.AddItem(ingredient, 1);
        }
        addedIngredients.Clear(); // just in case
        inventoryUI.PopulateEquipmentCraftingItemInventory();
    }

    public void AddIngredient(Item ingredient)
    {
        addedIngredients.Add(ingredient);
        inventory.RemoveItem(ingredient, 1);
        inventoryUI.PopulateEquipmentCraftingItemInventory();
        storedEquipment.AddIngredient(ingredient);
    }

    public void RemoveIngredient(Item ingredient)
    {
        addedIngredients.Remove(ingredient);
        inventory.AddItem(ingredient, 1);
        inventoryUI.PopulateEquipmentCraftingItemInventory();
        storedEquipment.RemoveIngredient(ingredient);
    }
    public void Craft()
    {
        if (storedEquipment != null && addedIngredients.Count > 0)
        {
            Equipment newEquipment = GetEquipmentToBeCrafted();
            newEquipment.SetID();
            inventory.AddEquipment(newEquipment);
            inventory.RemoveEquipmentByID(storedEquipment.GetID());
            // After crafting, clear ingredients
            addedIngredients.Clear();
            storedEquipment = null;
            equipmentSlotPrefab.GetComponent<EquipmentCraftingSlot>().RemoveEquipment();
            equipmentCraftingUI.GenerateIngredientsUI(null);
        }
        else
        {
            Debug.Log("Cannot craft: Missing equipment or ingredients.");
        }
    }
    public Equipment GetEquipmentToBeCrafted() // for preview/crafting purposes
    {
        Equipment equipment = Instantiate(storedEquipment);
        // Crafting logic here
        foreach (Item ingredient in addedIngredients)
        {
            foreach (var stat in ingredient.GetStats().Stats)
            {
                float currentValue = equipment.GetStats().GetStat(stat.GetStatID());
                equipment.GetStats().SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), currentValue + stat.Value);
            }
        }
        return equipment;
    }
    public List<Item> GetIngredients()
    {
        return addedIngredients;
    }
    void Initialize()
    {
        inventory = Inventory.Instance;
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
