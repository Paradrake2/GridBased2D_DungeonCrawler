using UnityEngine;

public class ShowAllSpellCOmponentIO : MonoBehaviour
{
    public Transform gridParent;

    public void ShowAllIO() // will be changed to be on hover
    {
        foreach (Transform cell in gridParent)
        {
            SpellGridCell spellGridCell = cell.GetComponent<SpellGridCell>();
            if (spellGridCell != null && spellGridCell.hasComponent)
            {
                spellGridCell.ShowIO(true);
            }
        }
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
