using UnityEngine;

public class CraftPotion : MonoBehaviour
{
    public Inventory inventory;
    public Item itemA;
    public Item itemB;
    public GameObject itemASlot;
    public GameObject itemBSlot;
    public Potion basePotion;
    public void AddItem(Item item)
    {
        if (itemA == null)
        {
            itemA = item;
            itemASlot.GetComponent<IngredientSlot>().SetIngredient(itemA);
            Debug.Log("Item A added: " + itemA.itemName);
        }
        else if (itemB == null)
        {
            itemB = item;
            itemBSlot.GetComponent<IngredientSlot>().SetIngredient(itemB);
            Debug.Log("Item B added: " + itemB.itemName);
        }
        else
        {
            Debug.Log("Both item slots are already filled.");
        }
        InventoryUI inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI.PopulateItemInventory();
    }
    public void Craft()
    {
        Potion endPotion = Instantiate(basePotion);
        PotionEffect newEffect = new PotionEffect();
        StatCollection endStatCollection = new StatCollection();
        // Combine stats from itemA
        StatCollection itemAStats = itemA.GetStats();
        foreach (var stat in itemAStats.Stats)
        {
            float existingValue = endStatCollection.GetStat(stat.StatType);
            endStatCollection.SetStat(stat.StatType, existingValue + stat.Value);
        }
        StatCollection itemBStats = itemB.GetStats();
        // Combine stats from itemB
        foreach (var stat in itemBStats.Stats)
        {
            float existingValue = endStatCollection.GetStat(stat.StatType);
            endStatCollection.SetStat(stat.StatType, existingValue + stat.Value);
        }
        newEffect.addedStats = endStatCollection;
        // debuff inflictors
        foreach (var inflictor in itemA.debuffInflictors)
        {
            newEffect.debuffInflictors.Add(inflictor);
        }
        foreach (var inflictor in itemB.debuffInflictors)
        {
            newEffect.debuffInflictors.Add(inflictor);
        }
        endPotion.effect = newEffect;
        endPotion.color = itemA.GetColor();
        endPotion.potionName = itemA.itemName + " & " + itemB.itemName + " Potion";
        // add end potion to inventory
        inventory.AddPotion(endPotion);
        // UI feedback
        // remove used items 
        itemA = null;
        itemB = null;
        itemASlot.GetComponent<IngredientSlot>().SetIngredient(null);
        itemBSlot.GetComponent<IngredientSlot>().SetIngredient(null);
    }
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
