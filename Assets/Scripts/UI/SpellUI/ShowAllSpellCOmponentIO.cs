using UnityEngine;

public class ShowAllSpellComponentIO : MonoBehaviour
{
    public Transform gridParent;
    bool isIOVisible = false;
    public void ToggleAllIO() // will be changed to be on hover
    {
        Debug.Log("Toggling IO visibility for all spell components.");
        isIOVisible = !isIOVisible;
        foreach (Transform cell in gridParent)
        {
            SpellGridCell spellGridCell = cell.gameObject.GetComponent<SpellGridCell>();
            if (spellGridCell != null && spellGridCell.hasComponent)
            {
                Debug.Log($"Toggling IO for {cell.gameObject.name} to {(isIOVisible ? "visible" : "hidden")}.");
                spellGridCell.ShowIO(isIOVisible);
            }
        }
    }
}
