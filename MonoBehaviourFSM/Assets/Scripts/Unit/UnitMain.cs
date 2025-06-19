using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class UnitMain : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float globalGravity = -9.81f;
    public float GlobalGravity => globalGravity;

    [SerializeField] private float gravityScale = 3.0f;
    public float GravityScale => gravityScale;
   
    [SerializeField] private float gravityScaleFalling = 3.0f;
    public float GravityScaleFalling => gravityScaleFalling;
    
    [SerializeField] private int maxJumps = 2;
    public int MaxJumps => maxJumps;

    // Subsystem references
    internal UnitAnimator uAnimator;
    internal UnitAnimationEventsProxy uAnimationProxy;
    internal UnitStateManager uState;
    internal UnitDirectionManager uDirection;
    internal UnitCollisions uCollisions;

    // Component references
    internal Rigidbody2D rb;
    internal BoxCollider2D bc;

    private void Awake()
    {
        // Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"No Rigidbody2D component found on {gameObject.name}");
        }
        else
        {
            rb.gravityScale = 0;
        }

        // BoxCollider2D
        bc = GetComponent<BoxCollider2D>();
        if (bc == null)
        {
            Debug.LogError($"No BoxCollider2D component found on {gameObject.name}");
        }

        // Subsystem Components
        uAnimator = GetComponent<UnitAnimator>();
        if (uAnimator == null)
        {
            Debug.LogWarning($"No UnitAnimator component found on {gameObject.name}");
        }

        uAnimationProxy = GetComponent<UnitAnimationEventsProxy>();
        if (uAnimationProxy == null)
        {
            Debug.LogWarning($"No UnitAnimationEventsProxy component found on {gameObject.name}");
        }

        uState = GetComponent<UnitStateManager>();
        if (uState == null)
        {
            Debug.LogWarning($"No UnitStateManager component found on {gameObject.name}");
        }
        else
        {
            uState.Initialize(this);
        }

        uDirection = GetComponent<UnitDirectionManager>();
        if (uDirection == null)
        {
            Debug.LogWarning($"No UnitDirectionManager component found on {gameObject.name}");
        }
        else
        {
            uDirection.Initialize(this);
        }

        uCollisions = GetComponent<UnitCollisions>();
        if (uCollisions == null)
        {
            Debug.LogWarning($"No UnitCollisions component found on {gameObject.name}");
        }
        else
        {
            uCollisions.Initialize(this);
        }

        GetComponent<IUnitController>()?.Initialize(this);
    }
}
