using UnityEngine;

public class Laser2D : MonoBehaviour
{
    [Header("光束参数")]
    public Transform laserOrigin;      // 发射点
    public SpriteRenderer laserBody;   // 中间亮色部分
    public SpriteRenderer laserGlow;   // 外围淡色光晕
    public float maxLaserLength = 10f; // 最大光束长度
    public LayerMask obstacleMask;     // 碰撞层
    public float minLaserLength = 0.1f; // 最短光束，避免消失

    [Header("发光效果")]
    public float glowAlphaMin = 0.4f;  // 光晕最透明
    public float glowAlphaMax = 0.8f;  // 光晕最亮
    public float glowSpeed = 2f;       // 呼吸速度

    private Color glowOriginalColor;

    private void Awake()
    {
        if (laserGlow)
            glowOriginalColor = laserGlow.color;
    }

    private void Update()
    {
        UpdateLaserLength();
        AnimateGlow();
    }

    private void UpdateLaserLength()
    {
        // 射线检测第一个障碍物
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, transform.up, maxLaserLength, obstacleMask);
        float laserLength = maxLaserLength;

        if (hit.collider != null)
        {
            laserLength = hit.distance;
        }

        // 防止 scale 为 0
        laserLength = Mathf.Max(laserLength, minLaserLength);

        // 更新光束位置和缩放
        if (laserBody)
        {
            laserBody.transform.position = laserOrigin.position + transform.up * laserLength / 2f;
            laserBody.transform.localScale = new Vector3(laserBody.transform.localScale.x, laserLength, 1f);
        }

        if (laserGlow)
        {
            laserGlow.transform.position = laserOrigin.position + transform.up * laserLength / 2f;
            laserGlow.transform.localScale = new Vector3(laserGlow.transform.localScale.x, laserLength, 1f);
        }
    }

    private void AnimateGlow()
    {
        if (laserGlow)
        {
            // 使用 alpha 闪烁，不禁用 SpriteRenderer
            float alpha = Mathf.Lerp(glowAlphaMin, glowAlphaMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f);
            Color c = glowOriginalColor;
            c.a = alpha;
            laserGlow.color = c;
        }
    }
}
