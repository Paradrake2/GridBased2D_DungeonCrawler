using UnityEngine;

public class RotateComponentButton : MonoBehaviour
{
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    public void RotateComponent()
    {
        spellCrafterUI.RotateSelectedComponent();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
