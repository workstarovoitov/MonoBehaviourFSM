using System;
using UnityEngine;

public class StartAnimationBehaviour : StateMachineBehaviour
{
    [Tooltip("Set this to the state name or hash this behaviour is attached to.")]
    public string stateName;

    public Action<Animator, AnimatorStateInfo, int> OnStartAnimation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnStartAnimation?.Invoke(animator, stateInfo, layerIndex);
    }
}