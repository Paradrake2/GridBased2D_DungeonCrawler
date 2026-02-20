using UnityEngine;

public class ToggleDFModeButton : MonoBehaviour
{
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    private bool isDFMode = false;
    public void ToggleDFMode()
    {
        isDFMode = !isDFMode;
        spellCrafterUI.ToggleDFMode(isDFMode);
    }
}
