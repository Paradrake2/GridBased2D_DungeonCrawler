using UnityEngine;

public class CraftingStatsUI : MonoBehaviour
{
    public Transform statsParent;
    public GameObject statsPrefab;
    public EquipmentCrafting equipmentCrafting;
    public void GenerateStatsEquipment()
    {
        // Clear previous stats
        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject);
        }
        Equipment equipment = equipmentCrafting.GetEquipmentToBeCrafted();
        // Generate new stats
        StatCollection stats = equipment.GetStats();
        foreach (var stat in stats.Stats)
        {
            CraftingStatsUIObject obj = Instantiate(statsPrefab, statsParent).GetComponent<CraftingStatsUIObject>();
            obj.SetStatIcon(StatDatabase.Instance.GetStat(stat.GetStatID()).icon);
            obj.SetStatText(StatDatabase.Instance.GetStat(stat.GetStatID()).displayName + ": " + stat.Value.ToString());
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
