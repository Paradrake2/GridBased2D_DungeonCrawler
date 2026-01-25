using UnityEngine;

[CreateAssetMenu(fileName = "StatSkillNode", menuName = "SkillTree/StatSkillNode")]
public class StatSkillNode : SkillNode
{
    [SerializeField] private StatType givenStat;
    [SerializeField] private float statIncreaseAmount;
    public override void AcquireSkill()
    {
        Player player = FindFirstObjectByType<Player>();
        PlayerStats playerStats = player.stats;
        if (player == null || playerStats == null)
        {
            Debug.LogError("Player or PlayerStats not found.");
            return;
        }
        if (HasPrerequisites())
        {
            playerStats.AddStat(givenStat, statIncreaseAmount);
            player.RecalculateAllValues();
            acquired = true;
        }
        else
        {
            Debug.LogWarning($"Cannot acquire {skillName}. Prerequisites not met.");
        }
    }
}
