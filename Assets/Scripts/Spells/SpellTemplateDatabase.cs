using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellTemplateDatabase", menuName = "Scriptable Objects/SpellTemplateDatabase")]
public class SpellTemplateDatabase : ScriptableObject
{
    public List<SpellTemplate> allSpellTemplates;
    public List<SpellTemplate> AllSpellTemplates => allSpellTemplates;
    public List<SpellTemplate> unlockedSpellTemplates;
    public List<SpellTemplate> UnlockedSpellTemplates => unlockedSpellTemplates;
    public List<SpellTemplate> GetAllTemplates()
    {
        return allSpellTemplates;
    }
    public List<SpellTemplate> GetUnlockedTemplates()
    {
        return unlockedSpellTemplates;
    }
    public List<SpellTemplate> GetTemplatesByName(string name)
    {
        return allSpellTemplates.FindAll(template => template.templateName == name);
    }

}
