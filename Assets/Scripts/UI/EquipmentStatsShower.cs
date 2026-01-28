using UnityEngine;
using TMPro;
[System.Serializable]
public class StatObjectPair
{
    public StatType statType;
    public GameObject prefab;
}


public class EquipmentStatsShower : MonoBehaviour
{
    [Header("Base Fields")]
    [SerializeField] private StatObjectPair healthPrefab;
    [SerializeField] private StatObjectPair manaPrefab;
    [SerializeField] private StatObjectPair damagePrefab;
    [SerializeField] private StatObjectPair defensePrefab;
    [SerializeField] private StatObjectPair attackSpeedPrefab;
    [SerializeField] private StatObjectPair magicPrefab;
    [Header("Defense Attribute Fields")]
    [SerializeField] private StatObjectPair waterDefensePrefab;
    [SerializeField] private StatObjectPair fireDefensePrefab;
    [SerializeField] private StatObjectPair earthDefensePrefab;
    [SerializeField] private StatObjectPair windDefensePrefab;
    [SerializeField] private StatObjectPair lightDefensePrefab;
    [SerializeField] private StatObjectPair darkDefensePrefab;
    [Header("Damage Attribute Fields")]
    [SerializeField] private StatObjectPair waterDamagePrefab;
    [SerializeField] private StatObjectPair fireDamagePrefab;
    [SerializeField] private StatObjectPair earthDamagePrefab;
    [SerializeField] private StatObjectPair windDamagePrefab;
    [SerializeField] private StatObjectPair lightDamagePrefab;
    [SerializeField] private StatObjectPair darkDamagePrefab;
    [Header("Miscellaneous Fields")]
    [SerializeField] private StatObjectPair craftingEfficiencyPrefab;
    
    public void ShowEquipmentStats(Equipment equipment)
    {
        healthPrefab.prefab.GetComponent<StatsShower>().Initialize(healthPrefab.statType, equipment.stats.GetStat("Health").ToString());
        damagePrefab.prefab.GetComponent<StatsShower>().Initialize(damagePrefab.statType, equipment.stats.GetStat("Damage").ToString());
        defensePrefab.prefab.GetComponent<StatsShower>().Initialize(defensePrefab.statType, equipment.stats.GetStat("Defense").ToString());
        attackSpeedPrefab.prefab.GetComponent<StatsShower>().Initialize(attackSpeedPrefab.statType, equipment.stats.GetStat("AttackSpeed").ToString());
        magicPrefab.prefab.GetComponent<StatsShower>().Initialize(magicPrefab.statType, equipment.stats.GetStat("Magic").ToString());
        waterDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(waterDamagePrefab.statType, equipment.stats.GetStat("WaterDamage").ToString());
        fireDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(fireDamagePrefab.statType, equipment.stats.GetStat("FireDamage").ToString());
        earthDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(earthDamagePrefab.statType, equipment.stats.GetStat("EarthDamage").ToString());
        windDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(windDamagePrefab.statType, equipment.stats.GetStat("WindDamage").ToString());
        lightDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(lightDamagePrefab.statType, equipment.stats.GetStat("LightDamage").ToString());
        darkDamagePrefab.prefab.GetComponent<StatsShower>().Initialize(darkDamagePrefab.statType, equipment.stats.GetStat("DarkDamage").ToString());
        waterDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(waterDefensePrefab.statType, equipment.stats.GetStat("WaterDefense").ToString());
        fireDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(fireDefensePrefab.statType, equipment.stats.GetStat("FireDefense").ToString());
        earthDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(earthDefensePrefab.statType, equipment.stats.GetStat("EarthDefense").ToString());
        windDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(windDefensePrefab.statType, equipment.stats.GetStat("WindDefense").ToString());
        lightDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(lightDefensePrefab.statType, equipment.stats.GetStat("LightDefense").ToString());
        darkDefensePrefab.prefab.GetComponent<StatsShower>().Initialize(darkDefensePrefab.statType, equipment.stats.GetStat("DarkDefense").ToString());
        craftingEfficiencyPrefab.prefab.GetComponent<StatsShower>().Initialize(craftingEfficiencyPrefab.statType, equipment.stats.GetStat("CraftingEfficiency").ToString());
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
