using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebuffCollection
{
    [SerializeField] private List<Debuff> debuffs = new List<Debuff>();
    public IReadOnlyList<Debuff> Debuffs => debuffs;

    public void AddDebuff(Debuff debuff)
    {
        debuffs.Add(debuff);
    }

    public void RemoveDebuff(Debuff debuff)
    {
        debuffs.Remove(debuff);
    }

    public Debuff GetDebuffByName(string name)
    {
        return debuffs.Find(d => d.debuffName == name);
    }

    public List<Debuff> GetAllDebuffs()
    {
        return debuffs;
    }
    public void ClearDebuffs()
    {
        debuffs.Clear();
    }
}
