using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public Item storedIngredient;
    public Image icon;
    [SerializeField] private Sprite baseIcon;
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
    }
    public void Instantiate()
    {
        icon.sprite = baseIcon;
    }
}
