using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;
    public void SetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
    public void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }
    void Start()
    {
        
    }


}
