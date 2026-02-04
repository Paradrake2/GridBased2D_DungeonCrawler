using UnityEngine;




[System.Serializable]
public class Spell : ScriptableObject
{
    [SerializeField] private string spellName;
    [SerializeField] private Sprite icon;
    public SpellBehaviour spellEffect;

    public void CastSpell()
    {
        spellEffect.Cast();
    }
}
