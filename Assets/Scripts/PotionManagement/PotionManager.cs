using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionEffectHolder
{
    public Potion potion;
    public int duration; // how many battles it lasts
}



public class PotionManager : MonoBehaviour
{
    public List<PotionEffectHolder> activePotions = new List<PotionEffectHolder>();

    public void DecreasePotionDurations()
    {
        for (int i = activePotions.Count - 1; i >= 0; i--)
        {
            activePotions[i].duration--;
            if (activePotions[i].duration <= 0)
            {
                activePotions.RemoveAt(i);
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
