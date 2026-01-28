using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryEquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Button button;
    private Equipment containedEquipment;
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] EquipmentUIManager equipmentUIManager;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] EquipmentCrafting equipmentCrafting;
    [SerializeField] EquipmentStatsShower equipmentStatsShower;
    [SerializeField] PlayerStatsShower playerStatsShower;
    public bool craftingMode = false;
    public void OnClick()
    {
        if (!craftingMode)
        {
            EquipMode();
        } else
        {
            CraftMode();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        equipmentStatsShower.ShowEquipmentStats(containedEquipment);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        playerStatsShower.UpdateStats();
    }
    void EquipMode()
    {
        if (equipmentManager.IsSlotEquipped(containedEquipment.equipmentSlot))
        {
            Debug.Log("Slot already has an equipped item.");
            return;
        }
        equipmentManager.EquipItem(containedEquipment);
        equipmentUIManager.UpdateEquipmentUISlot(containedEquipment, false);
        inventoryUI.PopulateEquipmentInventory();
        Debug.Log("Inventory equipment slot clicked: " + icon.sprite.name);
    }
    void CraftMode()
    {
        equipmentCrafting.SetStoredEquipment(containedEquipment);
    }
    public void SetEquipment(Equipment equipment)
    {
        containedEquipment = equipment;
    }
    public void Initialize(InventoryUI inventoryUI)
    {
        equipmentManager = FindAnyObjectByType<EquipmentManager>();
        equipmentUIManager = FindAnyObjectByType<EquipmentUIManager>();
        this.inventoryUI = inventoryUI;
    }
    public void SetCraftingMode(bool mode)
    {
        craftingMode = mode;
    }
    void Start()
    {
        equipmentCrafting = FindAnyObjectByType<EquipmentCrafting>();
        equipmentStatsShower = FindAnyObjectByType<EquipmentStatsShower>();
        playerStatsShower = FindAnyObjectByType<PlayerStatsShower>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
