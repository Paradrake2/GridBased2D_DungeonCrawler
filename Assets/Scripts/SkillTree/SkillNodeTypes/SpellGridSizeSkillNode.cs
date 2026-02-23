using UnityEngine;

public class SpellGridSizeSkillNode : SkillNode
{
    [SerializeField] private int gridSizeIncrease = 1;
    public int GetGridSizeIncrease()
    {
        return gridSizeIncrease;
    }
    public override void AcquireSkill()
    {
        SpellGridUI spellGridUI = FindAnyObjectByType<SpellGridUI>();
        spellGridUI.SetUsableGridSize(spellGridUI.UsableGridSizeX + gridSizeIncrease*2, spellGridUI.UsableGridSizeY + gridSizeIncrease*2);
    }
}
