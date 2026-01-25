using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillNodeDatabase", menuName = "SkillTree/SkillNodeDatabase")]
public class SkillNodeDatabase : ScriptableObject
{
    public List<SkillNode> skillNodes;
    public SkillNode GetSkillNodeByName(string name)
    {
        return skillNodes.Find(node => node.SkillName == name);
    }
}
