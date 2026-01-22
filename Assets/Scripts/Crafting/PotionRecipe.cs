using UnityEngine;

[CreateAssetMenu(fileName = "PotionRecipe", menuName = "Scriptable Objects/PotionRecipe")]
public class PotionRecipe : ScriptableObject
{
    public string recipeName;
    public Sprite icon;

    public Potion resultPotion;
}
