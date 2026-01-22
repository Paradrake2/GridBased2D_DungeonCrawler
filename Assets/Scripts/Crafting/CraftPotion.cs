using UnityEngine;

public class CraftPotion : MonoBehaviour
{
    public Inventory inventory;
    public Item itemA;
    public Item itemB;

    public void Craft()
    {
        Potion endPotion = ScriptableObject.CreateInstance<Potion>();
        PotionEffect newEffect = new PotionEffect();
        StatCollection endStatCollection = new StatCollection();
        // Combine stats from itemA
        StatCollection itemAStats = itemA.GetStats();
        foreach (var stat in itemAStats.Stats)
        {
            float existingValue = endStatCollection.GetStat(stat.StatType);
            endStatCollection.SetStat(stat.StatType, existingValue + stat.Value);
        }
        StatCollection itemBStats = itemB.GetStats();
        // Combine stats from itemB
        foreach (var stat in itemBStats.Stats)
        {
            float existingValue = endStatCollection.GetStat(stat.StatType);
            endStatCollection.SetStat(stat.StatType, existingValue + stat.Value);
        }
        newEffect.addedStats = endStatCollection;
        // debuff inflictors
        foreach (var inflictor in itemA.debuffInflictors)
        {
            newEffect.debuffInflictors.Add(inflictor);
        }
        foreach (var inflictor in itemB.debuffInflictors)
        {
            newEffect.debuffInflictors.Add(inflictor);
        }
        endPotion.effect = newEffect;
        // add end potion to inventory
        inventory.AddPotion(endPotion);
        // UI feedback
    }
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
