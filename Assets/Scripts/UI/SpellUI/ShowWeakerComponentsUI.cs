using UnityEngine;

public class ShowWeakerComponentsUI : MonoBehaviour
{
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    [SerializeField] private bool showingWeaker = false;
    [SerializeField] private bool isEnabled = false;
    public void ToggleWeakerComponents()
    {
        showingWeaker = !showingWeaker;
        if (showingWeaker)
        {
            spellCrafterUI.PopulateRegComponentListWeaker();
        }
        else
        {
            spellCrafterUI.PopulateComponentList();
        }
    }
    public void OnClick()
    {
        if (!isEnabled) return;
        ToggleWeakerComponents();
    }
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        // Optionally, you could also change the button's appearance here to reflect its state
    }
}
