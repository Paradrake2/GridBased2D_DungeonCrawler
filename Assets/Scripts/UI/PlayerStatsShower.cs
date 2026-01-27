using UnityEngine;

public class PlayerStatsShower : MonoBehaviour
{
    public GameObject statObjectPrefab;
    [Header("Base Fields")]
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject manaPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject defensePrefab;
    [SerializeField] private GameObject attackSpeedPrefab;
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private GameObject goldPrefab;
    [Header("Defense Attribute Fields")]
    [SerializeField] private GameObject waterDefensePrefab;
    [SerializeField] private GameObject fireDefensePrefab;
    [SerializeField] private GameObject earthDefensePrefab;
    [SerializeField] private GameObject windDefensePrefab;
    [SerializeField] private GameObject lightDefensePrefab;
    [SerializeField] private GameObject darkDefensePrefab;
    [Header("Damage Attribute Fields")]
    [SerializeField] private GameObject waterDamagePrefab;
    [SerializeField] private GameObject fireDamagePrefab;
    [SerializeField] private GameObject earthDamagePrefab;
    [SerializeField] private GameObject windDamagePrefab;
    [SerializeField] private GameObject lightDamagePrefab;
    [SerializeField] private GameObject darkDamagePrefab;
    [Header("Miscellaneous Fields")]
    [SerializeField] private GameObject craftingEfficiencyPrefab;

    [SerializeField] private Player player;
    [SerializeField] private PlayerStats playerStats;
    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }
        UpdateStats();
    }
    public void UpdateStats()
    {
        Debug.Log("Updating player stats UI");
        UpdateBaseStats();
        UpdateDefenseAttributeStats();
        UpdateDamageAttributeStats();
        //craftingEfficiencyPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("CraftingEfficiency"), player.GetCraftingEfficiency().ToString());
    }
    void UpdateBaseStats()
    {
        healthPrefab.GetComponent<StatsShower>().SetCustomText($"Health: {player.GetHealth()}/{player.GetMaxHealth()}", Color.red);
        // manaPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Mana"), player.GetMana().ToString());
        damagePrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Damage"), player.GetDamage().ToString());
        defensePrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Defense"), player.GetDefense().ToString());
        attackSpeedPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("AttackSpeed"), player.GetAttackSpeed().ToString());
        experiencePrefab.GetComponent<StatsShower>().SetCustomText($"XP: {playerStats.GetCurrentExperience()}/{playerStats.GetExperienceToNextLevel()}", Color.yellow);
        goldPrefab.GetComponent<StatsShower>().SetCustomText($"Gold: {playerStats.GetGoldAmount()}", Color.yellow);
    }
    void UpdateDefenseAttributeStats()
    {
        var defenseAttributes = player.attributeSet.defenseAttributes;
        foreach (var attr in defenseAttributes)
        {
            switch (attr.defenseAttribute.name)
            {
                case "WaterDefense":
                    waterDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                case "FireDefense":
                    fireDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                case "EarthDefense":
                    earthDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                case "WindDefense":
                    windDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                case "LightDefense":
                    lightDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                case "DarkDefense":
                    darkDefensePrefab.GetComponent<StatsShower>().Initialize(attr.defenseAttribute, attr.defenseAttributeValue.ToString());
                    break;
                // case damage reduction attributes here if needed
            }
        }
    }
    void UpdateDamageAttributeStats()
    {
        var attackAttributes = player.attributeSet.attackAttributes;
        foreach (var attr in attackAttributes)
        {
            switch (attr.attackAttribute.name)
            {
                case "WaterDamage":
                    waterDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                case "FireDamage":
                    fireDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                case "EarthDamage":
                    earthDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                case "WindDamage":
                    windDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                case "LightDamage":
                    lightDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                case "DarkDamage":
                    darkDamagePrefab.GetComponent<StatsShower>().Initialize(attr.attackAttribute, attr.attackAttributeValue.ToString());
                    break;
                // case other damage attributes here if needed
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
