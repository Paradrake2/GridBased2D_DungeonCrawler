using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCrafterUI : MonoBehaviour
{
    public Transform spellCrafterPanel;
    public Transform spellGridPanel;
    public GameObject gridCellPrefab;
    public Transform componentListPanel;
    public GameObject componentButtonPrefab;
    public Transform spellTemplateListPanel;
    public GameObject spellTemplateButtonPrefab;
    public int gridSizeX = 9;
    public int gridSizeY = 9;

    public int usableGridSizeX = 3;
    public int usableGridSizeY = 3;

    private GameObject[,] gridCells;
    public SpellGridCell selectedGridCell;
    [Header("Spell Composition Data")]
    public SpellComposition spellComposition;
    [SerializeField] private SpellCrafter spellCrafter;

    public void ClearSpellComposition()
    {
        spellComposition = null;
        spellComposition = new SpellComposition();
        // Additional logic to clear the UI representation can be added here
    }
    public void GenerateGrid(int newUsableGridSizeX, int newUsableGridSizeY)
    {
        SetUsableGridSize(newUsableGridSizeX, newUsableGridSizeY);

        if (gridCells == null || gridCells.GetLength(0) != gridSizeX || gridCells.GetLength(1) != gridSizeY)
            gridCells = new GameObject[gridSizeX, gridSizeY];

        ClearGridPanelChildren();

        ClearGridCellReferences();

        Vector2Int core = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        int halfX = usableGridSizeX / 2;
        int halfY = usableGridSizeY / 2;

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject cellGO = Instantiate(gridCellPrefab, spellGridPanel);
                SpellGridCell cell = cellGO.GetComponent<SpellGridCell>();

                bool isActive =
                    Mathf.Abs(x - core.x) <= halfX &&
                    Mathf.Abs(y - core.y) <= halfY;

                cell.Initialize(x, y, isActive, this);
                gridCells[x, y] = cellGO;
            }
        }
    }
    public void PopulateComponentList()
    {
        SpellComponentDatabase database = Resources.Load<SpellComponentDatabase>("SpellComponentDatabase");
        if (database == null)
        {
            Debug.LogError("SpellComponentDatabase not found in Resources.");
            return;
        }
        List<SpellComponent> components = database.GetUnlockedComponents();
        // Clear existing buttons
        foreach (Transform child in componentListPanel)
        {
            Destroy(child.gameObject);
        }

        // Create new buttons
        foreach (SpellComponent component in components)
        {
            GameObject buttonGO = Instantiate(componentButtonPrefab, componentListPanel);
            SpellComponentListObject listObject = buttonGO.GetComponent<SpellComponentListObject>();
            listObject.Initialize(component, this);
        }
    }
    private void ClearGridPanelChildren()
    {
        if (spellGridPanel == null) return;

        for (int i = spellGridPanel.childCount - 1; i >= 0; i--)
            Destroy(spellGridPanel.GetChild(i).gameObject);
    }

    private void ClearGridCellReferences()
    {
        if (gridCells == null) return;

        for (int y = 0; y < gridSizeY; y++)
        for (int x = 0; x < gridSizeX; x++)
            gridCells[x, y] = null;
    }

    public void PopulateSpellTemplateList()
    {
        SpellTemplateDatabase templateDatabase = Resources.Load<SpellTemplateDatabase>("SpellTemplateDatabase");
        if (templateDatabase == null)
        {
            Debug.LogError("SpellTemplateDatabase not found in Resources.");
            return;
        }
        List<SpellTemplate> templates = templateDatabase.UnlockedSpellTemplates;

        // Clear existing buttons
        foreach (Transform child in spellTemplateListPanel)
        {
            Destroy(child.gameObject);
        }

        // Create new buttons
        foreach (SpellTemplate template in templates)
        {
            GameObject buttonGO = Instantiate(spellTemplateButtonPrefab, spellTemplateListPanel);
            SpellSelectorObject selectorObject = buttonGO.GetComponent<SpellSelectorObject>();
            selectorObject.Initialize(template, this);
        }
    }

    public void SetUsableGridSize(int sizeX, int sizeY)
    {
        // Clamp to grid
        sizeX = Mathf.Clamp(sizeX, 1, gridSizeX);
        sizeY = Mathf.Clamp(sizeY, 1, gridSizeY);

        // Force odd (so "centered" is unambiguous)
        if (sizeX % 2 == 0) sizeX -= 1;
        if (sizeY % 2 == 0) sizeY -= 1;

        usableGridSizeX = sizeX;
        usableGridSizeY = sizeY;
    }

    public bool TryGetCell(int x, int y, out SpellGridCell cell)
    {
        cell = null;
        if (gridCells == null) return false;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;

        GameObject cellGO = gridCells[x, y];
        if (cellGO == null)
            return false;

        cell = cellGO.GetComponent<SpellGridCell>();
        return cell != null;
    }

    public List<SpellGridCell> GetAdjacentCells(int x, int y)
    {
        List<SpellGridCell> adjacentCells = new List<SpellGridCell>(4);

        // Avoid Vector2 allocations + casts; use int offsets
        TryAddAdjacent(x, y + 1, adjacentCells);
        TryAddAdjacent(x + 1, y, adjacentCells);
        TryAddAdjacent(x, y - 1, adjacentCells);
        TryAddAdjacent(x - 1, y, adjacentCells);

        return adjacentCells;
    }
    public void SetSelectedCellComponent(SpellComponent spellComponent)
    {
        if (selectedGridCell == null) return;
        selectedGridCell.placedComponent = spellComponent;
        // update visual representation
        selectedGridCell.GetComponent<Image>().sprite = spellComponent.Icon ?? null;
        // update tracker for spell composition
        spellComposition.AddComponent(spellComponent);
        // check to see if spell composition meets requirements, if so get stats of spell and display them
        if (spellComposition.MeetsRequirements())
        {
            // Display spell stats or update UI accordingly
        }
    }
    private void TryAddAdjacent(int x, int y, List<SpellGridCell> results)
    {
        if (TryGetCell(x, y, out SpellGridCell c) && c.isActive)
            results.Add(c);
    }
    public void GenerateSpell()
    {
        if (spellComposition.MeetsRequirements())
        {
            Debug.Log("Spell composition meets requirements! Generating spell...");
            spellCrafter.CreateSpell(spellComposition);
        }
        else
        {
            Debug.Log("Spell composition does not meet requirements.");
        }
    }
    public void SelectSpellTemplate(SpellTemplate template)
    {
        if (template == null)
        {
            Debug.LogError("Selected SpellTemplate is null.");
            return;
        }

        // Clear existing composition
        ClearSpellComposition();

        // Set grid size based on template
        SetUsableGridSize(template.gridSizeX, template.gridSizeY);
        GenerateGrid(usableGridSizeX, usableGridSizeY);

        // Populate spell composition requirements
        spellComposition.requirements = template.SpellRequirements.requirements;

        Debug.Log($"Selected Spell Template: {template.TemplateName}");
    }
    void Start()
    {
        gridCells = new GameObject[gridSizeX, gridSizeY];
        GenerateGrid(usableGridSizeX, usableGridSizeY);
        PopulateSpellTemplateList();
        ClearSpellComposition();
    }

    void Update() { }
}
