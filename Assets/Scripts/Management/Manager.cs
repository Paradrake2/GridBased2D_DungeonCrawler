using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    public bool playerMoving = false;
    public bool playerCanMove = true;
    public int currentFloor = 1;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void IncreaseFloor()
    {
        currentFloor++;
    }
    public void ResetFloor()
    {
        currentFloor = 1;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
