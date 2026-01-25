using System.Collections.Generic;
using UnityEngine;

public class SkillNode : ScriptableObject
{
    [SerializeField] protected string skillName;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected List<SkillNode> prerequisites;
    [SerializeField] protected int skillPointCost;
    [SerializeField] protected bool acquired = false;
    public virtual void AcquireSkill()
    {
        Debug.Log($"{skillName} activated.");
    }
    public string SkillName => skillName;
    public Sprite Icon => icon;
    public List<SkillNode> Prerequisites => prerequisites;
    public int SkillPointCost => skillPointCost;
    public bool Acquired => acquired;
    protected bool HasPrerequisites()
    {
        foreach (var prereq in prerequisites)
        {
            if (!prereq.acquired)
            {
                return false;
            }
        }
        return true;
    }
}
