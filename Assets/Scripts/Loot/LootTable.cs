using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem
{
    public Item item;
    public int dropChance;
}


[CreateAssetMenu(fileName = "LootTable", menuName = "Scriptable Objects/LootTable")]
public class LootTable : ScriptableObject
{
    public List<DropItem> dropItems;
    public int minItemsToDrop;
    public int maxItemsToDrop;
    public Item GetDroppedItem()
    {
        List<Item> droppedItems = new List<Item>();
        foreach (DropItem dropItem in dropItems)
        {
            for (int i = 0; i < dropItem.dropChance; i++)
            {
                droppedItems.Add(dropItem.item);
            }
        }
        int randomIndex = Random.Range(0, droppedItems.Count);
        return droppedItems[randomIndex];
    }
    public List<Item> GetMultipleDroppedItems()
    {
        List<Item> finalDroppedItems = new List<Item>();
        int itemsToDrop = Random.Range(minItemsToDrop, maxItemsToDrop + 1);
        for (int i = 0; i < itemsToDrop; i++)
        {
            finalDroppedItems.Add(GetDroppedItem());
        }
        return finalDroppedItems;
    }
}
