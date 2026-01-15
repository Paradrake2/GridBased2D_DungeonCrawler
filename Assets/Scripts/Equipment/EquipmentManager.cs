using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentSlotInfo
{
    public EquipmentSlot slot;
    public Equipment equippedItem;
}
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;
    public EquipmentUIManager equipmentUIManager;
    public Inventory inventory;
    public Player player;
    public List<EquipmentSlotInfo> equipment = new List<EquipmentSlotInfo>();
    public StatCollection GetEquipmentStats()
    {
        StatCollection totalStats = new StatCollection();
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            Equipment eq = slotInfo.equippedItem;
            {
                if (eq == null || eq.alreadyCounted) continue;
                StatCollection eqStats = eq.GetStats();
                foreach (var stat in eqStats.Stats)
                {
                    float currentValue = totalStats.GetStat(stat.GetStatID());
                    totalStats.SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), currentValue + stat.Value);
                }
                eq.alreadyCounted = true;
            }
        }
        return totalStats;
    }
    public StatCollection GetSpecificEquipmentStats(EquipmentSlotInfo slot)
    {
        StatCollection totalStats = new StatCollection();
        Equipment eq = slot.equippedItem;
        {
            if (eq == null) return totalStats;
            StatCollection eqStats = eq.GetStats();
            foreach (var stat in eqStats.Stats)
            {
                float currentValue = totalStats.GetStat(stat.GetStatID());
                totalStats.SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), currentValue + stat.Value);
            }
        }
        return totalStats;
    }
    public Sprite GetSpecificEquipmentIcon(EquipmentSlotInfo slot)
    {
        Equipment eq = slot.equippedItem;
        {
            if (eq == null) return null;
            return eq.equipmentIcon;
        }
    }
    public bool IsEquipmentSlotEmpty(EquipmentSlot slot)
    {
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            if (slotInfo.slot == slot)
            {
                return slotInfo.equippedItem == null;
            }
        }
        return true; // default to true if slot not found
    }
    public void SetAllEquipmentCountedFalse()
    {
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            Equipment eq = slotInfo.equippedItem;
            {
                if (eq == null) continue;
                eq.alreadyCounted = false;
            }
        }
    }
    public bool IsSlotEquipped(EquipmentSlot slot)
    {
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            if (slotInfo.slot == slot)
            {
                return slotInfo.equippedItem != null;
            }
        }
        return false; // default to false if slot not found
    }
    public void EquipItem(Equipment newEquipment)
    {
        if (newEquipment == null) return;
        inventory.RemoveEquipment(newEquipment);
        if (newEquipment.equipmentSlot == EquipmentSlot.Accessory)
        {
            // 1) First empty accessory slot
            for (int i = 0; i < equipment.Count; i++)
            {
                if (equipment[i].slot != EquipmentSlot.Accessory) continue;
                Debug.Log("1");
                if (equipment[i].equippedItem == null)
                {
                    equipment[i].equippedItem = newEquipment;
                    equipmentUIManager.UpdateEquipmentUISlot(newEquipment, false);
                    player.RecalculateAllValues();
                    return;
                }
            }

            // 2) If all accessory slots are full, decide behavior:
            //    Option A: replace the first accessory slot (FIFO-style)
            for (int i = 0; i < equipment.Count; i++)
            {
                if (equipment[i].slot != EquipmentSlot.Accessory) continue;
                Debug.Log("2");
                equipment[i].equippedItem = newEquipment;
                equipmentUIManager.UpdateEquipmentUISlot(newEquipment, false);
                player.RecalculateAllValues();

                return;
            }

            return;
        }
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            if (slotInfo.slot == newEquipment.equipmentSlot && slotInfo.equippedItem == null)
            {
                Debug.Log("3");
                slotInfo.equippedItem = newEquipment;
                equipmentUIManager.UpdateEquipmentUISlot(newEquipment, false);
                player.RecalculateAllValues();

                break;
            }
        }
    }
    public void Unequip(EquipmentSlot slot, Equipment equipmentToUnequip)
    {
        if (slot == EquipmentSlot.Accessory)
        {
            UnequipAccessory(equipmentToUnequip);
            return;
        }
        foreach (EquipmentSlotInfo slotInfo in equipment)
        {
            if (slotInfo.slot == slot)
            {
                if (slotInfo.equippedItem != null)
                {
                    inventory.AddEquipment(slotInfo.equippedItem);
                    slotInfo.equippedItem = null;
                    equipmentUIManager.UpdateEquipmentUISlot(null, true);
                    player.RecalculateAllValues();
                }
                break;
            }
        }
    }
    public Equipment UnequipAccessoryAt(int accessoryIndex)
    {
        int seen = 0;

        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].slot != EquipmentSlot.Accessory) continue;

            if (seen == accessoryIndex)
            {
                Equipment removed = equipment[i].equippedItem;
                equipment[i].equippedItem = null;
                inventory.AddEquipment(removed);
                equipmentUIManager.UpdateEquipmentUISlot(null, true);
                player.RecalculateAllValues();
                return removed;
            }

            seen++;
        }
        return null;
    }
    public bool UnequipAccessory(Equipment accessory)
    {
        if (accessory == null) return false;
        inventory.AddEquipment(accessory);
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].slot != EquipmentSlot.Accessory) continue;

            if (equipment[i].equippedItem == accessory)
            {
                equipment[i].equippedItem = null;
                equipmentUIManager.UpdateEquipmentUISlot(null, true);
                player.RecalculateAllValues();
                return true;
            }
        }

        return false;
    }
    public bool IsAccessorySlotEmpty(int accessoryIndex)
    {
        int seen = 0;

        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].slot != EquipmentSlot.Accessory) continue;

            if (seen == accessoryIndex)
                return equipment[i].equippedItem == null;

            seen++;
        }

        return true; // index not found => treat as empty
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        inventory = GetComponent<Inventory>();
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
