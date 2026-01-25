using UnityEngine;
using UnityEngine.UI;

public class InventoryPotionSlotUI : MonoBehaviour
{
    [SerializeField] public Potion potion;
    [SerializeField] private Image icon;
    public Button button;

    public void OnClick()
    {
        Debug.Log("Inventory potion slot clicked: " + icon.sprite.name);
    }
    private void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.color = potion.color;
        icon.preserveAspect = true;
    }
    public Potion GetPotion()
    {
        return potion;
    }
    public void Initialize(Potion potion)
    {
        this.potion = potion;
        SetIcon(potion.icon);
    }
}
