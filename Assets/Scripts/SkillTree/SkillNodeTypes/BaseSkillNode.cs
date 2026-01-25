using UnityEngine;

[CreateAssetMenu(fileName = "BaseSkillNode", menuName = "SkillTree/BaseSkillNode")]
public class BaseSkillNode : SkillNode
{
    public override void AcquireSkill()
    {
        acquired = true;
    }
}
