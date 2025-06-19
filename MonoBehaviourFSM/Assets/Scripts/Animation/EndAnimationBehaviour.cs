using UnityEngine;
using System;

public class EndAnimationBehaviour : StateMachineBehaviour
{
    [Tooltip("Set this to the state name or hash this behaviour is attached to.")]
    public string stateName;

    public Action<Animator, AnimatorStateInfo, int> OnEndAnimation;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnEndAnimation?.Invoke(animator, stateInfo, layerIndex);
    }
}