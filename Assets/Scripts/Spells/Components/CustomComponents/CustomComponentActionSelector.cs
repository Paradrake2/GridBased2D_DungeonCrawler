using UnityEngine;

public enum CustomComponentAction
{
    GetWeakness,
    GetDefense,
    GetDamage,
    GetMaxHealth,
    GetDebuffs,
    GetBuffs,
}


public class CustomComponentActionSelector : MonoBehaviour
{
    public CustomSpellComponent selectedComponent;
    public CustomComponentAction action;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
