using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellGridUI : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private Transform spellGridPanel;
    [SerializeField] private GameObject gridCellPrefab;

    [Header("Grid Size")]
    [SerializeField] private int gridSizeX = 9;
    [SerializeField] private int gridSizeY = 9;
    [SerializeField] private int usableGridSizeX = 3;
    [SerializeField] private int usableGridSizeY = 3;

    private SpellGridCell[,] gridCells;

    public SpellGridCell SelectedCell { get; private set; }

    public event Action<SpellGridCell> SelectionChanged;
    public event Action<SpellGridCell, SpellComponent> CellCleared;

    public void Configure(Transform panel, GameObject cellPrefab, int sizeX, int sizeY)
    {
        if (panel != null) spellGridPanel = panel;
        if (cellPrefab != null) gridCellPrefab = cellPrefab;
        if (sizeX > 0) gridSizeX = sizeX;
        if (sizeY > 0) gridSizeY = sizeY;
    }

    public void GenerateGrid(int newUsableGridSizeX, int newUsableGridSizeY)
    {
        if (spellGridPanel == null)
        {
            Debug.LogError("SpellGridUI missing spellGridPanel reference.");
            return;
        }

        if (gridCellPrefab == null)
        {
            Debug.LogError("SpellGridUI missing gridCellPrefab reference.");
            return;
        }

        SetUsableGridSize(newUsableGridSizeX, newUsableGridSizeY);

        if (gridCells == null || gridCells.GetLength(0) != gridSizeX || gridCells.GetLength(1) != gridSizeY)
            gridCells = new SpellGridCell[gridSizeX, gridSizeY];

        ClearGridPanelChildren();
        ClearGridCellReferences();

        SelectedCell = null;
        SelectionChanged?.Invoke(null);

        Vector2Int core = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        int halfX = usableGridSizeX / 2;
        int halfY = usableGridSizeY / 2;

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject cellGO = Instantiate(gridCellPrefab, spellGridPanel);
                SpellGridCell cell = cellGO.GetComponent<SpellGridCell>();
                if (cell == null)
                {
                    Debug.LogError("Grid cell prefab is missing SpellGridCell component.");
                    Destroy(cellGO);
                    continue;
                }

                bool isActive =
                    Mathf.Abs(x - core.x) <= halfX &&
                    Mathf.Abs(y - core.y) <= halfY;

                cell.Initialize(x, y, isActive, this);
                gridCells[x, y] = cell;
            }
        }
    }

    public void SetUsableGridSize(int sizeX, int sizeY)
    {
        sizeX = Mathf.Clamp(sizeX, 1, gridSizeX);
        sizeY = Mathf.Clamp(sizeY, 1, gridSizeY);

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

        cell = gridCells[x, y];
        return cell != null;
    }

    public List<SpellGridCell> GetAdjacentCells(int x, int y)
    {
        List<SpellGridCell> adjacentCells = new List<SpellGridCell>(4);
        TryAddAdjacent(x, y + 1, adjacentCells);
        TryAddAdjacent(x + 1, y, adjacentCells);
        TryAddAdjacent(x, y - 1, adjacentCells);
        TryAddAdjacent(x - 1, y, adjacentCells);
        return adjacentCells;
    }

    private void TryAddAdjacent(int x, int y, List<SpellGridCell> results)
    {
        if (TryGetCell(x, y, out SpellGridCell c) && c.isActive)
            results.Add(c);
    }

    public void ToggleSelection(SpellGridCell cell)
    {
        if (cell == null) return;
        if (!cell.isActive) return;

        if (SelectedCell == cell)
        {
            SelectedCell.SetSelected(false);
            SelectedCell = null;
            SelectionChanged?.Invoke(null);
            return;
        }

        if (SelectedCell != null)
            SelectedCell.SetSelected(false);

        SelectedCell = cell;
        SelectedCell.SetSelected(true);
        RefreshOutline();
        SelectionChanged?.Invoke(SelectedCell);
    }

    public void RefreshOutline()
    {
        if (gridCells == null) return;

        foreach (var cell in gridCells)
        {
            if (cell == null) continue;

            if (cell == SelectedCell)
            {
                cell.ShowOutline(true);
            }
            else
            {
                cell.ShowOutline(false);
                cell.UnselectCell();
            }
        }
    }

    public void RequestClearCell(SpellGridCell cell)
    {
        if (cell == null) return;
        if (!cell.isActive) return;

        SpellComponent oldComponent = cell.placedComponent;
        if (oldComponent != null)
            CellCleared?.Invoke(cell, oldComponent);

        cell.ClearPlacedComponentAndVisuals();

        if (SelectedCell == cell)
        {
            SelectedCell.SetSelected(false);
            SelectedCell = null;
            SelectionChanged?.Invoke(null);
        }
        RefreshOutline();
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
    public int UsableGridSizeX => usableGridSizeX;
    public int UsableGridSizeY => usableGridSizeY;
}
