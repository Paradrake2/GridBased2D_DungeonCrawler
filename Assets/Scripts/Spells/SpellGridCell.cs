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
    [SerializeField] private SpellGridUI spellGridUI;
    [SerializeField] private bool isSelectedGridCell = false;
    [SerializeField] private GameObject directionPrefab;

    [Header("Direction Indicator")]
    [SerializeField] private float directionOffset = 20f;
    [SerializeField] private int directionSortingOrder = 5000; // high so it draws above grid
    private List<GameObject> spawnedDirectionIndicators = new();
    private List<GameObject> inputDirectionIndicators = new();
    private List<GameObject> outputDirectionIndicators = new();
    private Coroutine coroutineSwitchIndicators;
    private bool isHovering;
    private bool isShowingIO = false;

    public void RefreshDirectionIndicatorsIfHovering()
    {
        if (!isHovering) return;

        RebuildDirectionIndicators();
    }

    public void Initialize(int xPos, int yPos, bool active, SpellGridUI ui)
    {
        spellGridUI = ui;

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
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        if (spellGridUI == null) return;
        spellGridUI.ToggleSelection(this);
    }

    private void HandleRightClick()
    {
        if (spellGridUI == null) return;
        spellGridUI.RequestClearCell(this);
    }

    public void UnselectCell()
    {
        isSelectedGridCell = false;
    }

    public void SetSelected(bool selected)
    {
        isSelectedGridCell = selected;
        ShowOutline(selected);
    }

    public void ClearPlacedComponentAndVisuals()
    {
        placedComponent = null;

        var img = GetComponent<Image>();
        if (img != null)
            img.sprite = null;

        ClearDirectionIndicators();
        SetSelected(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // When IO is being displayed explicitly, ignore real pointer hover events.
        // (ShowIO uses a null eventData to simulate hover and should still work.)
        if (isShowingIO && eventData != null) return;
        if (!isActive) return;
        if (!hasComponent) return;
        if (directionPrefab == null) return;

        isHovering = true;
        RebuildDirectionIndicators();
    }
    public void CallRebuildDirectionIndicators()
    {
        RebuildDirectionIndicators();
    }

    private void RebuildDirectionIndicators()
    {
        if (!isActive) return;
        if (!hasComponent) return;
        if (directionPrefab == null) return;

        ClearDirectionIndicators();

        SpellComponentDirections directions = placedComponent != null ? placedComponent.Directions : null;
        if (directions == null) return;

        List<SpellComponentDirectionPart> inputDirs = directions.inputDirections;
        //Debug.Log(inputDirs[0].directions);
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
        return dirPart.direction switch
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
    public void HideDirectionIndicators()
    {
        ClearDirectionIndicators();
        if (coroutineSwitchIndicators != null)
        {
            StopCoroutine(coroutineSwitchIndicators);
            coroutineSwitchIndicators = null;
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
        // When IO is being displayed explicitly, ignore real pointer exit events.
        // (ShowIO uses a null eventData to simulate exit and should still work.)
        if (isShowingIO && eventData != null) return;
        if (!isActive) return;

        isHovering = false;

        ClearDirectionIndicators();
        if (coroutineSwitchIndicators != null)
        {
            StopCoroutine(coroutineSwitchIndicators);
            coroutineSwitchIndicators = null;
        }
    }
    public void ShowIO(bool show)
    {
        isShowingIO = show;
        if (show)
            OnPointerEnter(null); // simulate hover to show indicators
        else
            OnPointerExit(null); // simulate exit to hide indicators
    }
}