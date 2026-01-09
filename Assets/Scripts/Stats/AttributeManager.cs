using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager instance;
    public List<AttributePairs> allAttributePairs;

    public StatType GetCorrespondingAttackAttribute(StatType defenseAttribute)
    {
        foreach (AttributePairs pair in allAttributePairs)
        {
            if (pair.GetDefenseAttribute() == defenseAttribute)
            {
                return pair.GetDamageAttribute();
            }
        }
        return null;
    }
    public StatType GetCorrespondingDefenseAttribute(StatType attackAttribute)
    {
        foreach (AttributePairs pair in allAttributePairs)
        {
            if (pair.GetDamageAttribute() == attackAttribute)
            {
                return pair.GetDefenseAttribute();
            }
        }
        return null;
    }
    public bool IsAttackAttribute(StatType attribute)
    {
        foreach (AttributePairs pair in allAttributePairs)
        {
            if (pair.GetDamageAttribute() == attribute)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsDefenseAttribute(StatType attribute)
    {
        foreach (AttributePairs pair in allAttributePairs)
        {
            if (pair.GetDefenseAttribute() == attribute)
            {
                return true;
            }
        }
        return false;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
