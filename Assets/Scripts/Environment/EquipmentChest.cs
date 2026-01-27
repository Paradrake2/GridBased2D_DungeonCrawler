using UnityEngine;

public class EquipmentChest : Chest
{
    public override void AcquireLoot()
    {
        Manager m = Manager.instance;
        LootManager lm = LootManager.Instance;
        Equipment loot = lm.GetEquipmentDropTableForFloor(Manager.instance.currentFloor).GetEquipment();
        Inventory inventory = FindFirstObjectByType<Inventory>();
        if (inventory != null && loot != null &&!isOpened)
        {
            inventory.AddEquipment(loot);
        }
        else
        {
            Debug.Log("No equipment acquired from the chest.");
        }
    }
}
