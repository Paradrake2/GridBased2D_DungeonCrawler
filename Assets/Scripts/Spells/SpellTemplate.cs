using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpellTemplate", menuName = "Spells/SpellTemplate")]
public class SpellTemplate : ScriptableObject
{
    public string templateName;
    public Sprite icon;
    [SerializeField] SpellComposition spellRequirements;
    public int gridSizeX;
    public int gridSizeY;
    public string TemplateName => templateName;
    public Sprite Icon => icon;
    public SpellComposition SpellRequirements => spellRequirements;
}
