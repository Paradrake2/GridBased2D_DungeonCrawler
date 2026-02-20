using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellGridCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite sprite;
    public int x;
    public int y;
    public bool isActive = false;

    public SpellComponent placedComponent = null;
    public bool hasComponent => placedComponent != null;

    [SerializeField] private GameObject overlayObject;
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private SpellCrafterUI spellCrafterUI;
    [SerializeField] private bool isSelectedGridCell = false;
    [SerializeField] private GameObject directionPrefab;

    [Header("Direction Indicator")]
    [SerializeField] private float directionOffset = 20f;
    [SerializeField] private int directionSortingOrder = 5000; // high so it draws above grid
    private readonly List<GameObject> spawnedDirectionIndicators = new();
    private List<GameObject> inputDirectionIndicators = new();
    private List<GameObject> outputDirectionIndicators = new();
    private Coroutine coroutineSwitchIndicators;
    private bool isHovering;

    public void Initialize(int xPos, int yPos, bool active, SpellCrafterUI ui)
    {
        spellCrafterUI = ui;

        x = xPos;
        y = yPos;
        isActive = active;

        if (overlayObject != null)
            overlayObject.SetActive(!isActive);

        isSelectedGridCell = false;
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }
    public void ShowOutline(bool show)
    {
        if (outlineObject != null)
            outlineObject.SetActive(show);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearAndUnselect();
        }
    }

    private void HandleLeftClick()
    {
        if (isSelectedGridCell)
        {
            isSelectedGridCell = false;
            if (spellCrafterUI != null && spellCrafterUI.selectedGridCell == this)
                spellCrafterUI.selectedGridCell = null;

            ShowOutline(false);

            Debug.Log($"Deselected Grid Cell at ({x}, {y})");
            return;
        }
        isSelectedGridCell = true;
        ShowOutline(true);
        if (spellCrafterUI != null)
        {
            spellCrafterUI.selectedGridCell = this;
            spellCrafterUI.RefreshOutline();
            spellCrafterUI.PopulateComponentList();
        }

        Debug.Log($"Selected Grid Cell at ({x}, {y})");
    }
    public void UnselectCell()
    {
        isSelectedGridCell = false;
    }
    private void ClearAndUnselect() // this is called on right-click
    {
        // Clear placed component + visuals
        if (spellCrafterUI != null)
        {
            spellCrafterUI.ClearAdjacentCellsOfComponent(x, y, placedComponent);
            if (spellCrafterUI.spellComposition != null)
                spellCrafterUI.spellComposition.RemoveComponent(placedComponent);
        }
        placedComponent = null;

        var img = GetComponent<Image>();
        if (img != null)
            img.sprite = null;

        // Also clear direction indicators (if any)
        ClearDirectionIndicators();
        ShowOutline(false);

        // Unselect
        isSelectedGridCell = false;
        if (spellCrafterUI != null)
        {
            if (spellCrafterUI.selectedGridCell == this)
                spellCrafterUI.selectedGridCell = null;

            spellCrafterUI.RefreshOutline();
            spellCrafterUI.UpdateSpellDescription();
        }
        Debug.Log($"Cleared & Unselected Grid Cell at ({x}, {y})");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActive) return;
        if (!hasComponent) return;
        if (directionPrefab == null) return;

        isHovering = true;
        ClearDirectionIndicators();

        SpellComponentDirections directions = placedComponent.Directions;
        if (directions == null) return;

        List<SpellComponentDirectionPart> inputDirs = directions.inputDirections;
        List<SpellComponentDirectionPart> outputDirs = directions.outputDirections;

        if (inputDirs != null)
        {
            foreach (var dirPart in inputDirs)
            {
                if (dirPart == null || !dirPart.isActive) continue;
                SpawnDirectionIndicator(dirPart, isInput: true);
            }
        }

        if (outputDirs != null)
        {
            foreach (var dirPart in outputDirs)
            {
                if (dirPart == null || !dirPart.isActive) continue;
                SpawnDirectionIndicator(dirPart, isInput: false);
                
            }
        }
        if (coroutineSwitchIndicators != null)
            StopCoroutine(coroutineSwitchIndicators);
        coroutineSwitchIndicators = StartCoroutine(SwitchInputOutputIndicators());
    }

    private void SpawnDirectionIndicator(SpellComponentDirectionPart dirPart, bool isInput)
    {
        GameObject dirIndicator = Instantiate(directionPrefab, transform);
        if (isInput)
            inputDirectionIndicators.Add(dirIndicator);
        else
            outputDirectionIndicators.Add(dirIndicator);
        // Setup indicator
        dirIndicator.transform.SetAsLastSibling();

        var canvas = dirIndicator.GetComponent<Canvas>();
        if (canvas == null) canvas = dirIndicator.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = directionSortingOrder;

        Vector2 dir = GetDirectionVector(dirPart);
        dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;

        var rt = dirIndicator.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = dir * directionOffset;

            float angle = GetAngle(dir, isInput);
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // Fallback (non-UI prefab)
            dirIndicator.transform.localPosition = (Vector3)(dir * directionOffset);
            float angle = GetAngle(dir, isInput);
            dirIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        Image img = dirIndicator.GetComponent<Image>();
        if (img != null)
        {
            img.color = isInput ? Color.green : Color.red;
            img.raycastTarget = false; // don't block clicks/hover
        }

        spawnedDirectionIndicators.Add(dirIndicator);
    }
    private float GetAngle(Vector2 dir, bool isInput)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (isInput)
            angle += 180f; // point towards center
        return angle - 90f; // adjust for arrow sprite orientation
    }
    private Vector2 GetDirectionVector(SpellComponentDirectionPart dirPart)
    {
        if (dirPart.direction != Vector2.zero)
            return dirPart.direction;

        // Fallback to enum if Vector2 wasn't filled in
        return dirPart.directions switch
        {
            Directions.Up => Vector2.up,
            Directions.Down => Vector2.down,
            Directions.Left => Vector2.left,
            Directions.Right => Vector2.right,
            _ => Vector2.up
        };
    }
    private IEnumerator SwitchInputOutputIndicators()
    {
        while (isHovering && (inputDirectionIndicators.Count > 0 || outputDirectionIndicators.Count > 0))
        {
            foreach (var indicator in inputDirectionIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(false);
            }

            foreach (var indicator in outputDirectionIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
            foreach (var indicator in inputDirectionIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(true);
            }

            foreach (var indicator in outputDirectionIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void ClearDirectionIndicators()
    {
        for (int i = 0; i < spawnedDirectionIndicators.Count; i++)
        {
            if (spawnedDirectionIndicators[i] != null)
                Destroy(spawnedDirectionIndicators[i]);
        }
        spawnedDirectionIndicators.Clear();
        inputDirectionIndicators.Clear();
        outputDirectionIndicators.Clear();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActive) return;

        isHovering = false;

        ClearDirectionIndicators();
        if (coroutineSwitchIndicators != null)
        {
            StopCoroutine(coroutineSwitchIndicators);
            coroutineSwitchIndicators = null;
        }
    }
    public void RotateComponent()
    {
        
    }
}