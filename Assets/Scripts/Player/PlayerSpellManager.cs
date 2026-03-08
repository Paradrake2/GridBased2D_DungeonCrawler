using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    public Player player;
    [SerializeField] private StatCollection spellStats = new StatCollection();
    [SerializeField] private PlayerAttributeSet tempAttributeSet = new PlayerAttributeSet();
    [SerializeField] private SpellBehaviour currentSpellBehaviour;
    [SerializeField] private float pendingSpellFlatDamage;
    [SerializeField] private bool inCombat = false;
    [SerializeField] private float pendingHealAmount = 0f;
    




    


    // called by Player at Start() to set reference to Player (circular dependency)
    public void Initialize(Player player)
    {
        this.player = player;
    }

    public StatCollection GetSpellStats()
    {
        return spellStats;
    }
    public void SetSpellStats(StatCollection stats)
    {
        spellStats = stats;
    }
    public void ClearSpellStats()
    {
        spellStats.Clear();
    }

    public float GetPendingSpellFlatDamage()
    {
        return pendingSpellFlatDamage;
    }

    public void SetPendingSpellFlatDamage(float damage)
    {
        pendingSpellFlatDamage = damage;
    }

    public void ClearPendingSpellFlatDamage()
    {
        pendingSpellFlatDamage = 0f;
    }
    public PlayerAttributeSet GetTempPlayerAttributeSet()
    {
        return tempAttributeSet;
    }
    public void SetTempPlayerAttributeSet(PlayerAttributeSet set)
    {
        tempAttributeSet = set;
    }
    public void ClearTempAttributeSet()
    {
        tempAttributeSet.ClearAllAttributes();
    }
    public void ClearTempAttributeAttackStats()
    {
        tempAttributeSet.ClearAttackAttributes();
    }
    public void ClearTempAttributeDefenseStats()
    {
        tempAttributeSet.ClearDefenseAttributes();
    }
    public void SetSpellBehaviour(SpellBehaviour sb)
    {
        currentSpellBehaviour = sb;
    }
    public SpellBehaviour GetSpellBehaviour()
    {
        return currentSpellBehaviour;
    }
    public void SetInCombat(bool value)
    {
        inCombat = value;
    }
    public bool IsInCombat()
    {
        return inCombat;
    }
    public float GetPendingHealAmount()
    {
        return pendingHealAmount;
    }
    public void SetPendingHealAmount(float amount)
    {
        pendingHealAmount = amount;
    }
}
