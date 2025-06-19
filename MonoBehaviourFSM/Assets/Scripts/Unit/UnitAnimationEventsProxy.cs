using UnityEngine;

public class UnitAnimationEventsProxy : MonoBehaviour
{
    public event System.Action OnStartJump;
    
    public event System.Action OnEndAnimation;

    public event System.Action OnFootstep;
   
    public void AnimationEvent_StartJump()
    {
        OnStartJump?.Invoke();
    }

    public void AnimationEvent_Footstep()
    {
        OnFootstep?.Invoke();
    }
  
    public void AnimationEvent_EndAnimation()
    {
        OnEndAnimation?.Invoke();
    }
}
