using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotUI : MonoBehaviour
{
    public Image itemIcon;
    public Text quantityText;
    public Button button;
    public void SetItemIcon(Sprite icon)
    {
        itemIcon.sprite = icon;
    }
    public void SetQuantityText(int quantity)
    {
        quantityText.text = quantity.ToString();
    }
    public void OnClick()
    {
        Debug.Log("Inventory slot clicked: " + itemIcon.sprite.name);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
