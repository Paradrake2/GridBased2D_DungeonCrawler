using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCrafterUI : MonoBehaviour
{
    public Transform spellCrafterPanel;
    public Transform spellGridPanel;
    public GameObject gridCellPrefab;
    public Transform componentListPanel;
    public GameObject componentButtonPrefab; // for regular components
    public GameObject DFComponentButtonPrefab; // for advanced (DF) components
    public Transform spellTemplateListPanel;
    public GameObject spellTemplateButtonPrefab;
    public int gridSizeX = 9;
    public int gridSizeY = 9;

    public int usableGridSizeX = 3;
    public int usableGridSizeY = 3;

    [Header("Grid UI")]
    [SerializeField] private SpellGridUI spellGridUI;

    public SpellGridCell selectedGridCell;
    [Header("Spell Composition Data")]
    public SpellComposition spellComposition;

    private SpellTemplate selectedTemplate;
    [SerializeField] private SpellCrafter spellCrafter;
    [SerializeField] private SpellDescriptionManager spellDescriptionManager;
    [SerializeField] private GameObject spellNameSelectorPrefab;
    [SerializeField] private bool isDFMode = false;
    [SerializeField] private ToggleDFModeButton toggleDFModeButton;
    [SerializeField] private RotateComponentButton rotateComponentButton;
    [SerializeField] private DFComponentGroup selectedDFComponentGroup;
    [SerializeField] private GameObject componentPreviewObject;
    [Header("Attribute Picker")]
    [SerializeField] private AttributePickerUI attributePickerUI;
    public void ClearSpellComposition()
    {
        spellComposition = null;
        spellComposition = new SpellComposition();
        // Additional logic to clear the UI representation can be added here
    }
    public void GenerateGrid(int newUsableGridSizeX, int newUsableGridSizeY)
    {
        SetUsableGridSize(newUsableGridSizeX, newUsableGridSizeY);
        EnsureSpellGridUI();

        if (spellGridUI == null) return;

        spellGridUI.Configure(spellGridPanel, gridCellPrefab, gridSizeX, gridSizeY);
        spellGridUI.GenerateGrid(usableGridSizeX, usableGridSizeY);
    }
    public void PopulateComponentList()
    {
        // Clear existing buttons
        foreach (Transform child in componentListPanel)
        {
            Destroy(child.gameObject);
        }
        if (isDFMode)
        {
            PopulateDFComponentList();
        } else
        {
            PopulateRegComponentList();
        }
    }
    public void RotateSelectedComponent()
    {
        if (selectedGridCell == null || !selectedGridCell.hasComponent) return;

        if (selectedDFComponentGroup == null) return;
        selectedDFComponentGroup.GoToNextComponent();
        selectedGridCell.placedComponent = selectedDFComponentGroup.currentComponent;
        UpdateComponentPreview(selectedDFComponentGroup.currentComponent);
        // Update the visual representation after rotation
        selectedGridCell.GetComponent<Image>().sprite = selectedGridCell.placedComponent.Icon ?? null;
        UpdateAttributePickerForSelection();
    }
    void PopulateRegComponentList()
    {
        SpellComponentDatabase database = Resources.Load<SpellComponentDatabase>("SpellComponentDatabase");
        if (database == null)
        {
            Debug.LogError("SpellComponentDatabase not found in Resources.");
            return;
        }
        List<SpellComponent> components = database.GetUnlockedComponentsByCategory(ComponentCategory.Regular);

        // Create new buttons
        foreach (SpellComponent component in components)
        {
            GameObject buttonGO = Instantiate(componentButtonPrefab, componentListPanel);
            SpellComponentListObject listObject = buttonGO.GetComponent<SpellComponentListObject>();
            listObject.Initialize(component, this);
        }
    }
    void PopulateDFComponentList()
    {
        SpellComponentDatabase database = Resources.Load<SpellComponentDatabase>("SpellComponentDatabase");
        if (database == null)
        {
            Debug.LogError("SpellComponentDatabase not found in Resources.");
            return;
        }
        List<DFComponentGroup> dfGroups = database.DFComponentGroups;
        foreach (DFComponentGroup group in dfGroups)
        {
            GameObject buttonGO = Instantiate(DFComponentButtonPrefab, componentListPanel);
            SpellDFComponentListObject listObject = buttonGO.GetComponent<SpellDFComponentListObject>();
            listObject.Initialize(group);
        }
    }
    public void ToggleDFMode(bool mode)
    {
        isDFMode = mode;
        PopulateComponentList();
        rotateComponentButton.SetActive(isDFMode);
        UpdateAttributePickerForSelection();
    }
    public void RefreshOutline()
    {
        if (spellGridUI == null) return;
        spellGridUI.RefreshOutline();
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
        if (spellGridUI == null) return false;
        return spellGridUI.TryGetCell(x, y, out cell);
    }

    public List<SpellGridCell> GetAdjacentCells(int x, int y)
    {
        if (spellGridUI == null) return new List<SpellGridCell>(0);
        return spellGridUI.GetAdjacentCells(x, y);
    }
    public void SetSelectedCellComponent(SpellComponent spellComponent)
    {
        if (selectedGridCell == null) return;

        // If the cell already had a component, remove it from the composition and from adjacent lists.
        // Without this, repeatedly placing on the same tile duplicates components in SpellComposition,
        // inflating cost/requirements and leaving "ghost" nodes for dataflow compilation.
        if (selectedGridCell.hasComponent)
        {
            SpellComponent oldComponent = selectedGridCell.placedComponent;
            if (oldComponent != null)
            {
                ClearAdjacentCellsOfComponent(selectedGridCell.x, selectedGridCell.y, oldComponent);
                spellComposition.RemoveComponent(oldComponent);
            }
        }

        SpellComponent newComponent = Instantiate(spellComponent);

        // check if compatible with adjacent components before setting
        List<SpellGridCell> adjacentCells = GetAdjacentCells(selectedGridCell.x, selectedGridCell.y);
        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell.hasComponent)
            {
                if (!newComponent.IsCompatibleWith(adjacentCell.placedComponent)) return; // not compatible
            }
        }
        selectedGridCell.placedComponent = newComponent;

        // Build adjacency for the new component (only add real neighbors).
        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell.hasComponent && adjacentCell.placedComponent != null)
                newComponent.AddAdjacentComponent(adjacentCell.placedComponent);
        }

        // update visual representation
        selectedGridCell.GetComponent<Image>().sprite = newComponent.Icon ?? null; // this will be updated later to have the bridge component display differently
        // if compatible, set component
        // update tracker for spell composition
        spellComposition.AddComponent(newComponent, selectedGridCell.x, selectedGridCell.y);

        // refresh all adjancent components' adjacent lists
        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell.hasComponent)
            {
                adjacentCell.placedComponent.AddAdjacentComponent(newComponent);
            }
        }

        UpdateSpellDescription();
        UpdateAttributePickerForSelection();
    }
    public void ClearAdjacentCellsOfComponent(int x, int y, SpellComponent component)
    {
        List<SpellGridCell> adjacentCells = GetAdjacentCells(x, y);
        foreach (var adjacentCell in adjacentCells)
        {
            if (adjacentCell.hasComponent)
            {
                adjacentCell.placedComponent.RemoveAdjacentComponent(component);
            }
        }
    }
    public void UpdateSpellDescription()
    {
        // check to see if spell composition meets requirements, if so get stats of spell and display them
        if (spellComposition.MeetsRequirements())
        {
            spellDescriptionManager.SetSpellDescription(true, spellComposition);
            // Display spell stats or update UI accordingly
        } else
        {
            spellDescriptionManager.SetSpellDescription(false, spellComposition);
        }
    }

    private void EnsureSpellGridUI()
    {
        if (spellGridUI != null) return;

        if (spellGridPanel != null)
            spellGridUI = spellGridPanel.GetComponent<SpellGridUI>();

        if (spellGridUI == null && spellGridPanel != null)
            spellGridUI = spellGridPanel.gameObject.AddComponent<SpellGridUI>();

        if (spellGridUI == null)
            spellGridUI = GetComponentInChildren<SpellGridUI>(true);
    }

    private void OnEnable()
    {
        EnsureSpellGridUI();
        if (spellGridUI == null) return;

        spellGridUI.SelectionChanged += HandleSelectionChanged;
        spellGridUI.CellCleared += HandleCellCleared;
    }

    private void OnDisable()
    {
        if (spellGridUI == null) return;

        spellGridUI.SelectionChanged -= HandleSelectionChanged;
        spellGridUI.CellCleared -= HandleCellCleared;
    }

    private void HandleSelectionChanged(SpellGridCell cell)
    {
        selectedGridCell = cell;
        PopulateComponentList();
        UpdateAttributePickerForSelection();
    }

    private void HandleCellCleared(SpellGridCell cell, SpellComponent oldComponent)
    {
        if (cell == null || oldComponent == null) return;

        ClearAdjacentCellsOfComponent(cell.x, cell.y, oldComponent);
        if (spellComposition != null)
            spellComposition.RemoveComponent(oldComponent);
        UpdateSpellDescription();
        UpdateAttributePickerForSelection();
    }

    private void UpdateAttributePickerForSelection()
    {
        if (attributePickerUI == null) return;

        if (selectedGridCell == null || !selectedGridCell.hasComponent)
        {
            attributePickerUI.Hide();
            return;
        }

        if (selectedGridCell.placedComponent is not DF_ConstantAttributeComponent constAttr)
        {
            attributePickerUI.Hide();
            return;
        }

        attributePickerUI.Show(constAttr.attribute, (StatType selected) =>
        {
            if (selected == null) return;
            constAttr.SetAttribute(selected);

            var img = selectedGridCell.GetComponent<Image>();
            if (img != null)
                img.sprite = constAttr.Icon;

            UpdateComponentPreview(constAttr);
            UpdateSpellDescription();
        });
    }
    public void GenerateSpell()
    {
        Debug.Log("Attempting to generate spell from composition...");
        if (spellComposition.MeetsRequirements())
        {
            Debug.Log("Spell composition meets requirements. Generating spell...");
            // pop up name spell screen, add to spellComposition
            GameObject spellNameSelectorGO = Instantiate(spellNameSelectorPrefab, transform);
            SpellNameSelector spellNameSelector = spellNameSelectorGO.GetComponent<SpellNameSelector>();
            spellNameSelector.Initialize((string spellName) =>
            {
                Debug.Log(spellName);
                spellComposition.spellName = spellName;
                Debug.Log(spellComposition.spellName);
                spellCrafter.AddSpellToInventory(spellComposition, selectedTemplate);
            });
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

        selectedTemplate = template;

        // Clear existing composition
        ClearSpellComposition();

        // Set grid size based on template
        SetUsableGridSize(template.gridSizeX, template.gridSizeY);
        GenerateGrid(usableGridSizeX, usableGridSizeY);

        // Populate spell composition requirements
        spellComposition.requirements = template.SpellRequirements.requirements;
        spellDescriptionManager.SetSpellDescription(false, spellComposition);
        Debug.Log($"Selected Spell Template: {template.TemplateName}");
    }
    public void SetDFComponentGroup(DFComponentGroup group)
    {
        selectedDFComponentGroup = group;
        if (group != null)
        {
            Debug.Log($"Selected DF Component Group: {group.GetGroupName()}");
            UpdateComponentPreview(group.currentComponent);
        }
    }
    public SpellCrafter GetSpellCrafter()
    {
        return spellCrafter;
    }
    public void UpdateComponentPreview(SpellComponent component)
    {
        if (componentPreviewObject == null) return;

        ComponentPreview preview = componentPreviewObject.GetComponent<ComponentPreview>();
        if (preview != null)
        {
            preview.SetComponent(component);
        }
    }
    void Start()
    {
        EnsureSpellGridUI();
        GenerateGrid(usableGridSizeX, usableGridSizeY);
        PopulateSpellTemplateList();
        ClearSpellComposition();
    }

    void Update() { }
}
