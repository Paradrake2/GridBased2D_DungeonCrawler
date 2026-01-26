using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public Item storedIngredient;
    public Image icon;
    [SerializeField] private Sprite baseIcon;
    [SerializeField] private bool isSlotA;
    [SerializeField] private bool isSlotB;
    [SerializeField] private bool equipmentCrafting;
    public void SetIngredient(Item ingredient)
    {
        storedIngredient = ingredient;
        if (storedIngredient != null)
        {
            icon.sprite = storedIngredient.GetIcon();
        }
        else
        {
            icon.sprite = baseIcon;
        }
    }
    public void OnClick()
    {
        Debug.Log("Ingredient slot clicked: " + icon.sprite.name);
        if (storedIngredient != null && !equipmentCrafting)
        {
            // remove ingredient from slot and add back to inventory
            Inventory inventory = FindAnyObjectByType<Inventory>();
            inventory.AddItem(storedIngredient, 1);
            SetIngredient(null);
            InventoryUI inventoryUI = FindAnyObjectByType<InventoryUI>();
            inventoryUI.PopulateItemInventory();
            if (isSlotA)
            {
                CraftPotion craftPotion = FindAnyObjectByType<CraftPotion>();
                craftPotion.itemA = null;
            }
            else if (isSlotB)
            {
                CraftPotion craftPotion = FindAnyObjectByType<CraftPotion>();
                craftPotion.itemB = null;
            }
        }
        else if (storedIngredient != null &&equipmentCrafting)
        {
            EquipmentCrafting equipmentCrafting = FindAnyObjectByType<EquipmentCrafting>();
            equipmentCrafting.RemoveIngredient(storedIngredient);
            SetIngredient(null);
            EquipmentCraftingUI equipmentCraftingUI = FindAnyObjectByType<EquipmentCraftingUI>();
            equipmentCraftingUI.UpdateIngredientSlots(equipmentCrafting.GetIngredients());
        }
    }
    public void Instantiate()
    {
        icon.sprite = baseIcon;
    }
    public void SetEquipmentCraftingMode(bool mode)
    {
        equipmentCrafting = mode;
    }
}
