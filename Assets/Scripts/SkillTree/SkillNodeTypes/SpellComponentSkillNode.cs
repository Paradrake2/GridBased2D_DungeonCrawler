using UnityEngine;

public class SpellComponentSkillNode : SkillNode
{
    [SerializeField] private SpellComponent component;
    public SpellComponent GetComponent()
    {
        return component;
    }
    public override void AcquireSkill()
    {
        SpellComponentDatabase componentDatabase = Resources.Load<SpellComponentDatabase>("SpellComponentDatabase");
        componentDatabase.UnlockedSpellComponents.Add(component);
    }
}
