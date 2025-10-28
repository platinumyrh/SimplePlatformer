using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvironmentDetector
{
    private Transform playerTransform;
    private Collider2D playerCollider;
    private LayerMask wallMask;
    private LayerMask groundMask;
    public float skin = 0.02f; // 缓冲距离
    public float wallCheckDistance = 0.1f;
    public float ledgeCheckDistance = 0.5f;

    private int wallRays = 3;

    public PlayerEnvironmentDetector(Transform transform, Collider2D collider, LayerMask Wallmask,LayerMask GroundMask)
    {
        playerTransform = transform;
        playerCollider = collider;
        wallMask = Wallmask;
        groundMask = GroundMask;
    }

    // --- 墙面检测 ---
    public bool IsTouchingWall(Vector2 dir)
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(-skin * 2f);

        float verticalStep = bounds.size.y / (wallRays - 1);

        for (int i = 0; i < wallRays; i++)
        {
            Vector2 origin = new Vector2(bounds.min.x, bounds.min.y) + Vector2.up * verticalStep * i;
            if (dir.x > 0) origin.x = bounds.max.x;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance,wallMask );
            Debug.DrawRay(origin, dir * wallCheckDistance, Color.red);

            if (hit.collider != null) return true;
        }

        return false;
    }

    // --- 可爬悬崖检测 ---
    public bool CanClimbLedge(Vector2 dir)
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(-skin*0.5f);

        int ledgeRays = 3;
        float verticalStep = bounds.size.y / (ledgeRays - 1);

        for (int i = 0; i < ledgeRays; i++)
        {
            Vector2 origin = new Vector2(bounds.min.x, bounds.min.y) + Vector2.up * verticalStep * i;
            if (dir.x > 0) origin.x = bounds.max.x;

            // 水平检测墙体（墙体层）
            RaycastHit2D wallHit = Physics2D.Raycast(origin, dir, wallCheckDistance, groundMask);

            // 检测上方是否空（平台层）
            Vector2 topOrigin = origin + Vector2.up * bounds.size.y * 0.5f; // 从角色中上部开始检测
            RaycastHit2D topFree = Physics2D.Raycast(topOrigin, dir, ledgeCheckDistance, wallMask);

            Debug.DrawRay(origin, dir * wallCheckDistance, Color.blue);
            Debug.DrawRay(topOrigin, dir * ledgeCheckDistance, Color.green);

            if (wallHit.collider != null && topFree.collider == null)
                return true;
        }

        return false;
    }

    // --- 地面检测 ---
    public bool IsGrounded()
    {
        Bounds bounds = playerCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - skin);

        float rayLen = skin + 0.05f; // 稍微长一点，防止漏判
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLen, groundMask);

        Debug.DrawRay(origin, Vector2.down * rayLen, hit.collider != null ? Color.green : Color.red);

        return hit.collider != null;
    }
}
