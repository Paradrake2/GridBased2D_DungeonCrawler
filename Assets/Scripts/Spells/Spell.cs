using UnityEngine;




[System.Serializable]
public class Spell : ScriptableObject
{
    [SerializeField] private string spellName;
    [SerializeField] private Sprite icon;
    public SpellBehaviour spellEffect;

    public virtual void CastSpell()
    {
        spellEffect.Cast();
    }
    public Sprite GetIcon()
    {
        return icon;
    }
    public void SetIcon(Sprite sprite)
    {
        icon = sprite;
    }
    public void SetSpellName(string name)
    {
        spellName = name;
    }
}
