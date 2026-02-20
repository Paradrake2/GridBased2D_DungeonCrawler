using UnityEngine;

[System.Serializable]
public class DFComponentGroupDirection
{
    public global::Directions direction;
    public SpellComponent component;
}

[CreateAssetMenu(fileName = "DFComponentGroup", menuName = "Spells/Dataflow/DFComponentGroup")]
public class DFComponentGroup : ScriptableObject
{
    public DFComponentGroupDirection[] components;
}
