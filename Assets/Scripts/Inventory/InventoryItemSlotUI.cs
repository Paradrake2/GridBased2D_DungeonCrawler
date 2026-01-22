using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemSlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Button button;
    private Item containedItem;
    [SerializeField] private bool equipmentCrafting = false;
    public void SetItemIcon(Sprite icon)
    {
        itemIcon.sprite = icon;
    }
    public void SetQuantityText(int quantity)
    {
        quantityText.text = quantity.ToString();
    }
    public void SetItem(Item item)
    {
        containedItem = item;
    }
    public void OnClick()
    {
        Debug.Log("Inventory slot clicked: " + itemIcon.sprite.name);
        if (equipmentCrafting)
        {
            // get list of available ingredient slots
            // find first empty ingredient slot and add the item there
        } else
        {
            // potion craft behaviour
        }
    }
    public void SetEquipmentCraftingMode(bool mode)
    {
        equipmentCrafting = mode;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
