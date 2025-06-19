using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles collision logic for UnitMain, including ground, wall, and platform checks.
/// </summary>
[RequireComponent(typeof(UnitMain))]
public class UnitCollisions : MonoBehaviour
{
    private UnitMain uMain;

    [Header("Settings")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask stairsLayer;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField, Range(5f, 60f)] private float slopeLimit = 60f;
    [SerializeField] private float bounce = 0.3f;
    [SerializeField] private float cliffOffset = 0.3f;

    private GameObject platform;
    public GameObject Platform => platform;

    private GameObject ledge;
    public GameObject Ledge => ledge;
    
    private GameObject wall;
    public GameObject Wall => wall;

    private int direction;
    private readonly HashSet<GameObject> ignoredPlatforms = new();
    private GameObject ignoredLedge = null;
    
    /// <summary>
    /// Initializes the collision manager with a reference to the main unit.
    /// </summary>
    public void Initialize(UnitMain unitMain)
    {
        if (unitMain == null)
        {
            Debug.LogError("UnitMain reference is null in UnitCollisions.");
            enabled = false;
            return;
        }
        uMain = unitMain;
        enabled = true;
    }

    public void IgnoreLedge()
    {
        ignoredLedge = ledge;
    }

    public void RestoreLedge()
    {
        ignoredLedge = null;
    }

    /// <summary>
    /// Ignores collision between the unit and the specified platform.
    /// </summary>
    public void IgnorePlatformCollision(GameObject platform)
    {
        if (platform == null) return;
        var myCollider = uMain.GetComponent<Collider2D>();
        var platformCollider = platform.GetComponent<Collider2D>();
        if (myCollider && platformCollider)
        {
            Physics2D.IgnoreCollision(myCollider, platformCollider, true);
            ignoredPlatforms.Add(platform);
        }
    }

    /// <summary>
    /// Restores collision between the unit and the specified platform.
    /// </summary>
    public void RestorePlatformCollision(GameObject platform)
    {
        if (platform == null) return;
        var myCollider = uMain.GetComponent<Collider2D>();
        var platformCollider = platform.GetComponent<Collider2D>();
        if (myCollider && platformCollider)
        {
            Physics2D.IgnoreCollision(myCollider, platformCollider, false);
            ignoredPlatforms.Remove(platform);
        }
    }

    /// <summary>
    /// Attempts to restore collisions with all previously ignored platforms if not overlapping.
    /// </summary>
    public void TryToRestorePlatformsCollision()
    {
        if (ignoredPlatforms.Count == 0) return;
        var toRestore = new List<GameObject>();
        foreach (var plat in ignoredPlatforms)
        {
            if (!IsOverlappingPlatform(plat))
            {
                toRestore.Add(plat);
            }
        }
        foreach (var plat in toRestore)
        {
            RestorePlatformCollision(plat);
        }
    }

    /// <summary>
    /// Checks if the unit is grounded on any valid surface.
    /// </summary>
    public bool IsGrounded()
    {
        direction = (int)uMain.uDirection.CurrentDirection;
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float halfWidth = bounds.size.x / 2;
        float yOffset = bounds.size.y / 2;

        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            Vector3.right * halfWidth * direction,
            Vector3.left * halfWidth * direction
        };

        foreach (var offset in offsets)
        {
            var origin = bounds.center + offset - Vector3.up * yOffset;
            var hits = Physics2D.RaycastAll(origin, Vector3.down, bounce + edgeRadius, collisionLayer | platformLayer);
            foreach (var hit in hits)
            {
                var newPlatform = hit.collider.gameObject;
                if (ignoredPlatforms.Contains(newPlatform)) continue;

                uMain.uState.OnPlatform = platformLayer == (platformLayer | (1 << newPlatform.layer));
                uMain.uState.SurfaceMaterial = newPlatform.tag;
                platform = newPlatform;
                return true;
            }
        }
        platform = null;
        return false;
    }

    /// <summary>
    /// Checks if the unit is currently overlapping the specified platform.
    /// </summary>
    public bool IsOverlappingPlatform(GameObject platform)
    {
        if (platform == null) return false;
        Vector2 boxSize = uMain.bc.size + Vector2.one * (uMain.bc.edgeRadius * 2);
        Vector2 origin = uMain.bc.bounds.center;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            origin,
            boxSize,
            0f,
            Vector2.right * (int)uMain.uDirection.CurrentDirection,
            0f,
            platformLayer
        );

        return hits.Any(hit => hit.transform.gameObject == platform);
    }

    /// <summary>
    /// Checks for a wall in front of the unit.
    /// </summary>
    public bool WallInFrontX()
    {
        wall = null;

        var hits = BoxCastWallAll(Vector2.right * (int)uMain.uDirection.CurrentDirection, collisionLayer);
        if (hits == null || hits.Length == 0) return false;

        foreach (var hit in hits)
        {
            if (IsOverlappingPlatform(hit.transform.gameObject)) continue;
            wall = hit.transform.gameObject;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks for a wall in front of the unit enough for sliding.
    /// </summary>
    public bool WallForGrabInFrontX()
    {
        if (!WallInFrontX()) return false;

        Bounds wallBounds = wall.GetComponent<Collider2D>().bounds;

        if (transform.position.y < wallBounds.min.y) return false; // Ensure the unit is above the wall

        return true;
    }

    /// <summary>
    /// Checks for a block in front of the unit.
    /// </summary>
    public bool BlockInFrontX()
    {
        return BoxCastWall(Vector2.right * (int)uMain.uDirection.CurrentDirection, blockLayer);
    }

    /// <summary>
    /// Checks for a cliff in front of the unit.
    /// </summary>
    public bool CliffInFrontX()
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float xOffset = bounds.size.x + cliffOffset;
        float yOffset = bounds.size.y / 2;
        Vector3 origin = bounds.center + Vector3.right * xOffset * (int)uMain.uDirection.CurrentDirection - Vector3.up * yOffset;
        return !Physics2D.Raycast(origin, Vector3.down, 0.5f, collisionLayer | platformLayer);
    }

    /// <summary>
    /// Checks for a wall behind the unit.
    /// </summary>
    public bool WallInBackX()
    {
        return BoxCastWall(Vector2.left * (int)uMain.uDirection.CurrentDirection, collisionLayer);
    }

    /// <summary>
    /// Checks for stairs above the unit.
    /// </summary>
    public RaycastHit2D StairOnTop()
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float yOffset = (bounds.size.y + 2 * edgeRadius) / 2;
        Vector3 origin = bounds.center + Vector3.up * yOffset;
        Vector2 size = new(bounds.size.x, 2 * bounce);
        return Physics2D.BoxCast(origin, size, 0f, Vector2.up, 0f, stairsLayer);
    }

    /// <summary>
    /// Checks for a wall above the unit.
    /// </summary>
    public RaycastHit2D WallOnTop()
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float yOffset = (bounds.size.y + 2 * edgeRadius) / 2;
        Vector3 origin = bounds.center + Vector3.up * yOffset;
        Vector2 size = new(bounds.size.x, 2 * bounce);
        return Physics2D.BoxCast(origin, size, 0f, Vector2.up, 0f, collisionLayer);
    }

    /// <summary>
    /// Checks for a wall at the top front of the unit.
    /// </summary>
    public bool WallInFrontTop()
    {
        return WallInFrontVertical(true);
    }

    /// <summary>
    /// Checks for a wall at the bottom front of the unit.
    /// </summary>
    public bool WallInFrontBottom()
    {
        return WallInFrontVertical(false);
    }

    /// <summary>
    /// Checks if there is a ledge in front of the unit.
    /// </summary>
    public bool IsLedgeInFront()
    {
        ledge = null;

        if (WallInFrontTop()) return false;

        var hits = BoxCastWallAll(Vector2.right * (int)uMain.uDirection.CurrentDirection, collisionLayer);
        if (hits == null || hits.Length == 0) return false;

        foreach (var hit in hits)
        {
            if (hit.transform.gameObject == ignoredLedge) continue; // Ignore the ledge we are currently on
            
            ledge = hit.transform.gameObject;
            return true;
        }

        return false;
    }

    public bool IsPlatformInFront()
    {
        ledge = null;

        if (WallInFrontTop()) return false;

        var hits = BoxCastWallAll(Vector2.right * (int)uMain.uDirection.CurrentDirection, platformLayer);
        if (hits == null || hits.Length == 0) return false;

        foreach (var hit in hits)
        {
            if (ignoredPlatforms.Contains(hit.transform.gameObject)) continue; // Ignore the ledge we are currently on
            if (hit.transform.gameObject == ignoredLedge) continue; // Ignore the ledge we are currently on

            ledge = hit.transform.gameObject;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if there is a wall between this unit and the target GameObject.
    /// </summary>
    public bool IsWallBetweenTarget(GameObject target)
    {
        if (target == null) return false;
        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, direction, distance, collisionLayer);
        foreach (RaycastHit2D hit in hitColliders)
        {
            if (hit.transform.gameObject != target)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Helper for front/back wall boxcasts.
    /// </summary>
    private bool BoxCastWall(Vector2 direction, LayerMask mask)
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float slopeRad = Mathf.Deg2Rad * (90 - slopeLimit);
        float yOffset = (bounds.size.y + 2 * edgeRadius) / 2;
        float xOffset = (bounds.size.x + 2 * edgeRadius) / 2;
        float height = (bounds.size.y + 2 * edgeRadius) - bounce / Mathf.Tan(slopeRad);
        Vector3 origin = bounds.center - Vector3.up * yOffset
            + Vector3.up * (yOffset + bounce / 2 / Mathf.Tan(slopeRad))
            + (Vector3)direction * xOffset;
        Vector2 size = new(bounce * 1.5f, height);
        return Physics2D.BoxCast(origin, size, 0f, direction, 0f, mask);
    }

    /// <summary>
    /// Helper for vertical wall checks at the front (top/bottom).
    /// </summary>
    private bool WallInFrontVertical(bool top)
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float yOffset = (bounds.size.y / 2 + edgeRadius + bounce);
        float xOffset = (bounds.size.x / 2 + edgeRadius + bounce);
        Vector3 origin = bounds.center
            + (top ? Vector3.up : Vector3.down) * yOffset
            + Vector3.right * (int)uMain.uDirection.CurrentDirection * xOffset;
        Vector2 size = new(2 * bounce, 2 * bounce);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.right * (int)uMain.uDirection.CurrentDirection, 0f, collisionLayer);
        return hit;
    }

    /// <summary>
    /// Helper for getting all hits in front wall boxcast.
    /// </summary>
    private RaycastHit2D[] BoxCastWallAll(Vector2 direction, LayerMask mask)
    {
        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float slopeRad = Mathf.Deg2Rad * (90 - slopeLimit);
        float yOffset = (bounds.size.y + 2 * edgeRadius) / 2;
        float xOffset = (bounds.size.x + 2 * edgeRadius) / 2;
        float height = (bounds.size.y + 2 * edgeRadius) - bounce / Mathf.Tan(slopeRad);
        Vector3 origin = bounds.center - Vector3.up * yOffset
            + Vector3.up * (yOffset + bounce / 2 / Mathf.Tan(slopeRad))
            + (Vector3)direction * xOffset;
        Vector2 size = new(bounce * 1.5f, height);
        return Physics2D.BoxCastAll(origin, size, 0f, direction, 0f, mask);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || uMain == null) return;

        var bounds = uMain.bc.bounds;
        var edgeRadius = uMain.bc.edgeRadius;
        float halfWidth = bounds.size.x / 2;
        float yOffset = bounds.size.y / 2;

        // Draw ground check rays
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(bounds.center + Vector3.right * halfWidth * direction - Vector3.up * yOffset, Vector3.down * (bounce + edgeRadius));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(bounds.center + Vector3.left * halfWidth * direction - Vector3.up * yOffset, Vector3.down * (bounce + edgeRadius));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, uMain.bc.size + Vector2.one * (edgeRadius * 2 ));

        // Front wall check box
        Gizmos.color = Color.red;
        float slopeRad = Mathf.Deg2Rad * (90 - slopeLimit);
        float yWallOffset = (bounds.size.y + 2 * edgeRadius) / 2;
        float xWallOffset = (bounds.size.x + 2 * edgeRadius) / 2;
        float wallHeight = (bounds.size.y + 2 * edgeRadius) - bounce / Mathf.Tan(slopeRad);
        Vector3 frontWallCheckBoxPosition = bounds.center - Vector3.up * yWallOffset
            + Vector3.up * (yWallOffset + bounce / 2 / Mathf.Tan(slopeRad))
            + Vector3.right * (int)uMain.uDirection.CurrentDirection * xWallOffset;
        Vector3 wallXCheckBoxSize = new(bounce, wallHeight, bounce);
        Gizmos.DrawWireCube(frontWallCheckBoxPosition, wallXCheckBoxSize);

        // Back wall check box
        Gizmos.color = Color.blue;
        Vector3 backWallCheckBoxPosition = bounds.center - Vector3.up * yWallOffset
            + Vector3.up * (yWallOffset + bounce / 2 / Mathf.Tan(slopeRad))
            + Vector3.left * (int)uMain.uDirection.CurrentDirection * xWallOffset;
        Gizmos.DrawWireCube(backWallCheckBoxPosition, wallXCheckBoxSize);

        // Stair check box
        Gizmos.color = Color.cyan;
        Vector3 stairCheckBoxPosition = bounds.center + Vector3.up * yWallOffset;
        Vector3 stairCheckBoxSize = new(bounds.size.x, 2 * bounce, bounce);
        Gizmos.DrawWireCube(stairCheckBoxPosition, stairCheckBoxSize);

        // WallForSlideInFront() boxes
        Gizmos.color = Color.magenta;
        Vector3 slideBoxSize = new(2 * bounce, 2 * bounce, bounce);
        Vector3 slideBoxOffset = Vector3.right * (int)uMain.uDirection.CurrentDirection * (bounds.size.x / 2 + edgeRadius + bounce);

        // Upper box
        Vector3 slideBoxUpPos = bounds.center + Vector3.up * (bounds.size.y / 2 + edgeRadius + bounce) + slideBoxOffset;
        Gizmos.DrawWireCube(slideBoxUpPos, slideBoxSize);

        // Lower box
        Vector3 slideBoxDownPos = bounds.center + Vector3.down * (bounds.size.y / 2 + edgeRadius + bounce) + slideBoxOffset;
        Gizmos.DrawWireCube(slideBoxDownPos, slideBoxSize);

        // Cliff check rays
        Gizmos.color = Color.gray;
        Gizmos.DrawRay(bounds.center + Vector3.right * (bounds.size.x + cliffOffset) * (int)uMain.uDirection.CurrentDirection - Vector3.up * yOffset, Vector3.down);

        Gizmos.DrawWireSphere(bounds.center + Vector3.right * (int)uMain.uDirection.CurrentDirection * 3 * edgeRadius, uMain.bc.size.x / 2f);
    }
#endif
}
