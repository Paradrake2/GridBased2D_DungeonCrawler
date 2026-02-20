using UnityEngine;
using UnityEngine.UI;
public class ComponentPreview : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetComponent(SpellComponent component)
    {
        if (component != null && component.Icon != null)
        {
            icon.sprite = component.Icon;
            icon.enabled = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
    void Start()
    {
        
    }

}
