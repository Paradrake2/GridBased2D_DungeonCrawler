using UnityEngine;

public enum InventoryScreen
{
    EquipmentStats,
    SkillTree,
    Crafting
}


public class UIManager : MonoBehaviour
{
    public Transform UITransform;
    public Transform EquipmentStatsTransform;
    public Transform InventoryTransform;
    public Transform SkillTreeTransform;
    public Transform CraftingTransform;
    public bool isUIOpen = false;
    public InventoryUI inventoryUI;
    [Header("Crafting")]
    [SerializeField] private Transform potionCraftingTransform;
    [SerializeField] private Transform equipmentCraftingTransform;
    [SerializeField] private GameObject toggleButton;
    public bool isDirty = false; // set when inventory changes
    private bool invType = true; // false = item, true = equipment
    [SerializeField] private InventoryScreen currentScreen = InventoryScreen.EquipmentStats;
    public void ToggleUI()
    {
        isUIOpen = !isUIOpen;
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerMovement.SetCanMove(!isUIOpen);
        UITransform.gameObject.SetActive(isUIOpen);
        PlayerStatsShower statsShower = FindAnyObjectByType<PlayerStatsShower>();
        statsShower.UpdateStats();
        if (isDirty)
        {
            switch (currentScreen)
            {
                case InventoryScreen.EquipmentStats:
                    OpenEquipmentStats();
                    break;
                case InventoryScreen.SkillTree:
                    OpenSkillTree();
                    break;
                case InventoryScreen.Crafting:
                    OpenCrafting();
                    break;
            }
            isDirty = false;
        }
    }
    public void OpenSkillTree()
    {
        SkillTreeTransform.gameObject.SetActive(true);
        CraftingTransform.gameObject.SetActive(false);
        EquipmentStatsTransform.gameObject.SetActive(false);
        InventoryTransform.gameObject.SetActive(false);
        currentScreen = InventoryScreen.SkillTree;
    }
    public void OpenCrafting()
    {
        CraftingTransform.gameObject.SetActive(true);
        SkillTreeTransform.gameObject.SetActive(false);
        EquipmentStatsTransform.gameObject.SetActive(false);
        InventoryTransform.gameObject.SetActive(true);
        inventoryUI.PopulateItemInventory();
        currentScreen = InventoryScreen.Crafting;
        ItemInvOpen();
    }
    public void EquipmentInvOpen() // used in crafting to select equipment
    {
        inventoryUI.PopulateCraftingEquipmentInventory();
        potionCraftingTransform.gameObject.SetActive(false);
        equipmentCraftingTransform.gameObject.SetActive(true);
        //toggleButton.SetActive(true);
    }
    public void ItemInvOpen() // used in crafting for potions
    {
        inventoryUI.PopulateItemInventory();
        equipmentCraftingTransform.gameObject.SetActive(false);
        potionCraftingTransform.gameObject.SetActive(true);
        //toggleButton.SetActive(false);
    }
    public void ToggleInventoryType()
    {
        invType = !invType;
        if (invType)
        {
            inventoryUI.PopulateCraftingEquipmentInventory();
        } else
        {
            inventoryUI.PopulateItemInventory();
        }
        
    }
    public void OpenEquipmentStats()
    {
        EquipmentStatsTransform.gameObject.SetActive(true);
        SkillTreeTransform.gameObject.SetActive(false);
        CraftingTransform.gameObject.SetActive(false);
        InventoryTransform.gameObject.SetActive(true);
        inventoryUI.PopulateEquipmentInventory();
        PlayerStatsShower statsShower = FindAnyObjectByType<PlayerStatsShower>();
        statsShower.UpdateStats();
        currentScreen = InventoryScreen.EquipmentStats;
    }
    void Start()
    {
        UITransform.gameObject.SetActive(isUIOpen);
        EquipmentStatsTransform.gameObject.SetActive(false);
        SkillTreeTransform.gameObject.SetActive(false);
        CraftingTransform.gameObject.SetActive(false);
        inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI.InitializeInventory();
        OpenEquipmentStats();
    }
}
