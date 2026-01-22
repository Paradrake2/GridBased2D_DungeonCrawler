using UnityEngine;

public class EquipmentDrop : MonoBehaviour
{
    public EquipmentDropTable dropTable;
    public Equipment GetDroppedEquipment()
    {
        return dropTable.GetEquipment();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
