using UnityEngine;

public class RotateComponentButton : MonoBehaviour
{
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    [SerializeField] private GameObject buttonVisual;
    public bool isActive = false;
    public void RotateComponent()
    {
        spellCrafterUI.RotateSelectedComponent();
    }
    public void SetActive(bool active)
    {
        isActive = active;
        buttonVisual.SetActive(active);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spellCrafterUI == null)
        {
            spellCrafterUI = FindAnyObjectByType<SpellCrafterUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
