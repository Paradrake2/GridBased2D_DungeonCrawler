using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private string chestID;
    [SerializeField] private bool isOpened = false;
    [SerializeField] private Sprite closedIcon;
    [SerializeField] private Sprite openedIcon;
    [SerializeField] private Collider2D chestCollider;
    public virtual void AcquireLoot()
    {
        Debug.Log("Base chest loot acquired.");
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && !isOpened)
        {
            OpenChest();
        }
    }
    private void OpenChest()
    {
        isOpened = true;
        GetComponent<SpriteRenderer>().sprite = openedIcon;
        AcquireLoot();
    }
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = closedIcon;
    }

}
