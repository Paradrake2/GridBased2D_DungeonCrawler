using UnityEngine;
using UnityEngine.UI;

public class EquipmentUISlot : MonoBehaviour
{
    public EquipmentSlot slot;
    public Equipment storedEquipment;
    public Color unequippedColor = new Color(0.27f, 0.27f, 0.27f);
    public Sprite icon;
    public Sprite defaultIcon;
    public Image iconImage;
    public Button button;
    public EquipmentManager equipmentManager;
    public EquipmentUIManager equipmentUIManager;

    public void SetIcon(Sprite newIcon)
    {
        icon = newIcon;
        iconImage.sprite = icon;
        iconImage.color = icon != null ? Color.white : unequippedColor;
    }

    public void SetEquipment(Equipment newEquipment)
    {
        storedEquipment = newEquipment;
        SetIcon(newEquipment.equipmentIcon);
    }
    public void RemoveEquipment()
    {
        storedEquipment = null;
        SetIcon(defaultIcon);
    }
    public void OnClick()
    {
        // unequip
        // if EquipmentUIManager.isUnequipping is true, then unequip the item in this slot
        Debug.Log("Equipment slot clicked: " + slot.ToString());
    }
    public void SetSlot(EquipmentSlot newSlot)
    {
        slot = newSlot;
    }
    void Start()
    {
        equipmentManager = FindAnyObjectByType<EquipmentManager>();
        if (equipmentManager == null)
        {
            Debug.LogError("EquipmentManager not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
