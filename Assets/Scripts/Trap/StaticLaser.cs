using UnityEngine;

public class StaticLaser : MonoBehaviour
{
    [Header("光束Sprite")]
    public SpriteRenderer laserBody;   // 中间亮红色
    public SpriteRenderer laserGlow;   // 外围淡红色光晕

    [Header("发光闪烁参数")]
    public float glowAlphaMin = 0.4f;  // 最透明
    public float glowAlphaMax = 0.8f;  // 最亮
    public float glowSpeed = 2f;       // 呼吸速度

    private Color glowOriginalColor;

    private void Awake()
    {
        if (laserGlow)
            glowOriginalColor = laserGlow.color;
    }

    private void Update()
    {
        AnimateGlow();
    }

    private void AnimateGlow()
    {
        if (!laserGlow) return;

        // alpha 闪烁，不禁用 SpriteRenderer
        float alpha = Mathf.Lerp(glowAlphaMin, glowAlphaMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f);
        Color c = glowOriginalColor;
        c.a = alpha;
        laserGlow.color = c;
    }
}
