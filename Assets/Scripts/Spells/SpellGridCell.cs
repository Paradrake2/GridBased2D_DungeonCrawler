using UnityEngine;

public class SpellGridCell : MonoBehaviour
{
    public Sprite sprite;
    public int x;
    public int y;
    public bool isActive = false;
    public SpellComponent placedComponent = null;
    public bool hasComponent => placedComponent != null;
    [SerializeField] private GameObject overlayObject;
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    [SerializeField] private bool isSelectedGridCell = false;
    public void Initialize(int xPos, int yPos, bool active, SpellCrafterUI ui)
    {
        spellCrafterUI = ui;
        overlayObject.SetActive(false);
        x = xPos;
        y = yPos;
        isActive = active;
        overlayObject.SetActive(!isActive);
    }
    public void OnClick()
    {
        if (!isActive) return;
        if (isSelectedGridCell)
        {
            isSelectedGridCell = false;
            spellCrafterUI.selectedGridCell = null;
            Debug.Log($"Deselected Grid Cell at ({x}, {y})");
            return;
        }
        isSelectedGridCell = true;
        spellCrafterUI.selectedGridCell = this;
        spellCrafterUI.PopulateComponentList();
        Debug.Log($"Selected Grid Cell at ({x}, {y})");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
