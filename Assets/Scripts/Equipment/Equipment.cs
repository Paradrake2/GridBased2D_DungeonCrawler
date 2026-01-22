using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    Helmet,
    Chest,
    Legs,
    Boots,
    Gauntlets,
    Weapon,
    Shield,
    Accessory
}


[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public Sprite equipmentIcon;
    public EquipmentSlot equipmentSlot;
    public StatCollection stats = new StatCollection();
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    public string id;
    public bool persistent = false;
    public bool alreadyCounted = false; // prevent equipment from being counted multiple times in stats calculations
    [SerializeField] private int ingredientCapacity = 2; // number of ingredients that can be added to this equipment
    [SerializeField] private int remainingIngredientSlots = 2; // number of ingredient slots left
    [SerializeField] private List<Item> ingredients = new List<Item>();
    public List<Item> GetIngredients()
    {
        return ingredients;
    }
    public void AddIngredient(Item newIngredient)
    {
        ingredients.Add(newIngredient);
    }
    public void RemoveIngredient(Item ingredientToRemove) // this is only for debug purposes, in game once an ingredient is added it cannot be removed
    {
        ingredients.Remove(ingredientToRemove);
    }
    public void UpdatePersistance(bool value)
    {
        persistent = value;
    }
    public StatCollection GetStats()
    {
        return stats;
    }
    public List<PlayerDebuffInflictorHolder> GetDebuffInflictors()
    {
        return debuffInflictors;
    }
    public List<PlayerDebuffResistanceHolder> GetDebuffResistances()
    {
        return debuffResistances;
    }
    public void AddDebuffInflictor(PlayerDebuffInflictorHolder inflictor)
    {
        debuffInflictors.Add(inflictor);
    }
    public void AddDebuffResistance(PlayerDebuffResistanceHolder resistance)
    {
        debuffResistances.Add(resistance);
    }
    public int GetIngredientCapacity()
    {
        return ingredientCapacity;
    }
    public void SetIngredientCapacity(int newCapacity)
    {
        ingredientCapacity = newCapacity;
    }
    public int GetRemainingIngredientSlots()
    {
        return remainingIngredientSlots;
    }
    public void SetRemainingIngredientSlots(int newRemainingSlots)
    {
        remainingIngredientSlots = newRemainingSlots;
    }

}