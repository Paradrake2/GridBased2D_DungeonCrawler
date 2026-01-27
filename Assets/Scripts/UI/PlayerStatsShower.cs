using UnityEngine;

public class PlayerStatsShower : MonoBehaviour
{
    public GameObject statObjectPrefab;
    [Header("Fields")]
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject manaPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject defensePrefab;
    [SerializeField] private GameObject attackSpeedPrefab;
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private GameObject goldPrefab;
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
        healthPrefab.GetComponent<StatsShower>().SetCustomText($"Health: {player.GetHealth()}/{player.GetMaxHealth()}", Color.red);
        // manaPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Mana"), player.GetMana().ToString());
        damagePrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Damage"), player.GetDamage().ToString());
        defensePrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("Defense"), player.GetDefense().ToString());
        attackSpeedPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("AttackSpeed"), player.GetAttackSpeed().ToString());
        experiencePrefab.GetComponent<StatsShower>().SetCustomText($"XP: {playerStats.GetCurrentExperience()}/{playerStats.GetExperienceToNextLevel()}", Color.yellow);
        goldPrefab.GetComponent<StatsShower>().SetCustomText($"Gold: {playerStats.GetGoldAmount()}", Color.yellow);
        //craftingEfficiencyPrefab.GetComponent<StatsShower>().Initialize(StatDatabase.Instance.GetStat("CraftingEfficiency"), player.GetCraftingEfficiency().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
