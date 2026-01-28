using UnityEngine;

public class EquipmentChest : Chest
{
    public EquipmentDropTable overrideDropTable;
    public EquipmentRarity lootRarity;
    public override void AcquireLoot()
    {
        Manager m = Manager.instance;
        LootManager lm = LootManager.Instance;
        Equipment loot = null;
        if (overrideDropTable != null)
        {
            loot = overrideDropTable.GetEquipment();
            Debug.Log("Acquired equipment from override drop table.");
        }
        else
        {
            loot = lm.GetEquipmentDropTableForFloor(Manager.instance.currentFloor, lootRarity).GetEquipment();
            Debug.Log("Acquired equipment from floor drop table.");
        }
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
