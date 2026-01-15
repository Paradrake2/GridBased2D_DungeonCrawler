using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class EquipmentUIManager : MonoBehaviour
{
    public Transform equipmentStatsParent;
    [Header("Equipment/Stats")]
    [SerializeField] private GameObject helmetSlot;
    [SerializeField] private GameObject chestSlot;
    [SerializeField] private GameObject legsSlot;
    [SerializeField] private GameObject bootsSlot;
    [SerializeField] private GameObject gauntletSlot;
    [SerializeField] private GameObject shieldSlot;
    [SerializeField] private GameObject weaponSlot;
    [SerializeField] private GameObject accessorySlot1;
    [SerializeField] private GameObject accessorySlot2;
    [SerializeField] private GameObject accessorySlot3;
    [SerializeField] private GameObject buttonUnequipMode;
    public Transform statsParent;

    [SerializeField] private bool isUnequipping = false;
    private GameObject GetCorrectSlot(EquipmentSlot slot)
    {
        if (slot != EquipmentSlot.Accessory)
        {
            switch (slot)
            {
                case EquipmentSlot.Helmet:
                    return helmetSlot;
                case EquipmentSlot.Chest:
                    return chestSlot;
                case EquipmentSlot.Legs:
                    return legsSlot;
                case EquipmentSlot.Boots:
                    return bootsSlot;
                case EquipmentSlot.Gauntlets:
                    return gauntletSlot;
                case EquipmentSlot.Shield:
                    return shieldSlot;
                case EquipmentSlot.Weapon:
                    return weaponSlot;
                default:
                    return null;
            }
        }
        else
        {
            if (accessorySlot1.GetComponent<EquipmentUISlot>().storedEquipment == null)
            {
                return accessorySlot1;
            }
            else if (accessorySlot2.GetComponent<EquipmentUISlot>().storedEquipment == null)
            {
                return accessorySlot2;
            }
            else
            {
                return accessorySlot3;
            }
        }
    }
    public void UpdateEquipmentUISlot(Equipment slotInfo, bool isUnequipping, EquipmentUISlot slot = null)
    {
        if (!isUnequipping)
        {
            GameObject slotObject = GetCorrectSlot(slotInfo.equipmentSlot);
            EquipmentUISlot uiSlot = slotObject.GetComponent<EquipmentUISlot>();
            if (slotInfo != null)
            {
                Debug.Log("Updating UI Slot for equipment: " + slotInfo.equipmentName);
                uiSlot.SetEquipment(slotInfo);
            }
            else
            {
                uiSlot.RemoveEquipment();
            }
        } else
        {
            if (slot != null)
            {
                slot.RemoveEquipment();
            }
        }
    }
    public void ToggleUnequipMode()
    {
        isUnequipping = !isUnequipping;
        if (isUnequipping)
        {
            Debug.Log("Entered unequip mode.");
            buttonUnequipMode.GetComponent<Image>().color = Color.red;
        }
        else
        {
            buttonUnequipMode.GetComponent<Image>().color = Color.white;
            Debug.Log("Exited unequip mode.");
        }
        Debug.Log("Unequip mode: " + isUnequipping);
    }
    public bool IsUnequipping()
    {
        return isUnequipping;
    }
    public void PopulateEquipmentUI()
    {
        
    }
}
