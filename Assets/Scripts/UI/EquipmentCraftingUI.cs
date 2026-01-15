using System.Collections.Generic;
using UnityEngine;

public class EquipmentCraftingUI : MonoBehaviour
{
    public Transform equipmentStatsParent;
    public Transform transformIngredientsParent;
    public GameObject equipmentSlotPrefab;
    public GameObject equipmentIngredientPrefab;
    [SerializeField] private List<GameObject> ingredientSlots = new List<GameObject>();
    public void GenerateIngredientsUI(Equipment equipment)
    {
        if (equipment == null)
        {
            // Clear previous slots
            foreach (Transform child in transformIngredientsParent)
            {
                Destroy(child.gameObject);
            }
            ingredientSlots.Clear();
            return;
        }
        int numSlots = equipment.GetIngredientCapacity();
        // Clear previous slots
        foreach (Transform child in transformIngredientsParent)
        {
            Destroy(child.gameObject);
        }
        ingredientSlots.Clear();
        for (int i = 0; i < numSlots; i++)
        {
            GameObject slot =  Instantiate(equipmentIngredientPrefab, transformIngredientsParent);
            ingredientSlots.Add(slot);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
