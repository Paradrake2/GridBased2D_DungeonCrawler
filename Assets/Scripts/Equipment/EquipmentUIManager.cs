using System.Collections.Generic;
using UnityEngine;




public class EquipmentUIManager : MonoBehaviour
{
    public Transform equipmentParent;
    public GameObject helmetSlot;
    public GameObject chestSlot;
    public GameObject legsSlot;
    public GameObject bootsSlot;
    public GameObject gauntletSlot;
    public GameObject shieldSlot;
    public GameObject weaponSlot;
    public GameObject accessorySlot1;
    public GameObject accessorySlot2;
    public GameObject accessorySlot3;
    public bool isUnequipping = false;
    public GameObject GetCorrectSlot(EquipmentSlot slot)
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
    public void UpdateEquipmentUISlot(Equipment slotInfo)
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
    }
    public void PopulateEquipmentUI()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
