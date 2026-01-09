using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Debuff", menuName = "Status Effects/Debuff")]
public class Debuff :ScriptableObject
{
    public string debuffName;
    public StatType associatedStat;
    public float duration = -1;
    public float tickInterval = 10f; // how often it ticks
    public float timeSinceLastTick = 0f;
    public bool canStack = false;
    public int maxStacks = 1;
    public int currentStacks = 1;

    public GameObject target;
    public virtual void ApplyDebuff(GameObject target, GameObject inflictor, float value = 0) {}
    public virtual IEnumerator TickEffect()
    {
        Debug.Log("Base Debuff TickEffect called. This should be overridden in derived classes.");
        return null;
    }
}
