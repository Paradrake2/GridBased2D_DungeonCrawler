using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Buff", menuName = "Status Effects/Buff")]
public class Buff : ScriptableObject
{
    public string buffName;
    public int duration; // number of battles the buff lasts for
    public StatCollection statCollection;
    public bool canStack = false;

    public virtual void ApplyBuff(GameObject target)
    {
        
    }
}
