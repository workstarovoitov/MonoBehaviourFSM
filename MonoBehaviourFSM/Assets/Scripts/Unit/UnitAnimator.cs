using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    internal Animator animator = null;
    public Animator Animator => animator;

    // Queues for parameter changes
    private string trigger;
    private readonly Dictionary<string, bool> boolQueue = new();
    private readonly Dictionary<string, float> floatQueue = new();
    private readonly Dictionary<string, int> intQueue = new();

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator) Debug.LogError("No animator found inside " + gameObject.name);
    }

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator) Debug.LogError("No animator found inside " + gameObject.name);
    }

    private void LateUpdate()
    {
        // Set triggers
        if (!string.IsNullOrEmpty(trigger))
        {
            ResetAllAnimatorTriggers();
            animator.SetTrigger(trigger);
            trigger = string.Empty; // Clear the trigger after setting it   
        }

        // Set bools
        foreach (var kvp in boolQueue)
        {
            animator.SetBool(kvp.Key, kvp.Value);
        }
        boolQueue.Clear();

        // Set floats
        foreach (var kvp in floatQueue)
        {
            animator.SetFloat(kvp.Key, kvp.Value);
        }
        floatQueue.Clear();

        // Set ints
        foreach (var kvp in intQueue)
        {
            animator.SetInteger(kvp.Key, kvp.Value);
        }
        intQueue.Clear();
    }
    private void ResetAllAnimatorTriggers()
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    // Queue a trigger to be set in LateUpdate
    public void SetAnimatorTrigger(string name)
    {
        trigger = name;
    }

    // Queue a bool to be set in LateUpdate
    public void SetAnimatorBool(string name, bool state)
    {
        boolQueue[name] = state;
    }

    // Queue a float to be set in LateUpdate
    public void SetAnimatorFloat(string name, float value)
    {
        floatQueue[name] = value;
    }

    // Queue an int to be set in LateUpdate
    public void SetAnimatorInt(string name, int value)
    {
        intQueue[name] = value;
    }

    public void EnableAnimator(bool enable)
    {
        animator.enabled = enable;
    }
}