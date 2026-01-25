using UnityEngine;

public class EquipmentChest : Chest
{
    [SerializeField] private EquipmentDropTable equipmentDropTable;

    public override void AcquireLoot()
    {
        Equipment loot = equipmentDropTable.GetEquipment();
        Inventory inventory = FindFirstObjectByType<Inventory>();
        if (inventory != null && loot != null)
        {
            inventory.AddEquipment(loot);
        }
        else
        {
            Debug.Log("No equipment acquired from the chest.");
        }
    }
}
