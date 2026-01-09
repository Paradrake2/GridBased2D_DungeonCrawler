using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public Equipment testEquipment;
    public Player player;
    public void AddTestEquipmentToPlayer()
    {
        Equipment testingEquipment;
        testingEquipment = Instantiate(testEquipment);
        player.equipmentManager.equipment.Add(testingEquipment);
        player.UpdateFromEquipment();
        Debug.Log("Added test equipment: " + testingEquipment.name);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
