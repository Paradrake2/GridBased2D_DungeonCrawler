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
    [SerializeField] private Sprite icon;
    [SerializeField] private string groupName;
    [SerializeField] private int index = 0;
    public SpellComponent currentComponent => components != null && components.Length > 0 ? components[index].component : null;
    public DFComponentGroupDirection[] components;
    public void GoToNextComponent()
    {
        if (components == null || components.Length == 0) return;
        index = (index + 1) % components.Length;
    }
    public Sprite GetIcon()
    {
        if (currentComponent != null && currentComponent.Icon != null)
            return currentComponent.Icon;
        return icon;
    }
    public string GetGroupName()
    {
        if (currentComponent != null)
            return $"{groupName} - {currentComponent.ComponentName}";
        return groupName;
    }
    private int GetIndexOfComponent(SpellComponent component)
    {
        if (components == null || components.Length == 0) return -1;
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].component == component)
                return i;
        }
        return -1;
    }
    private int GetCurrentIndex()
    {
        return index;
    }
}
