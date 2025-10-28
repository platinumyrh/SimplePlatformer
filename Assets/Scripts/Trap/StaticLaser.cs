using UnityEngine;

public class StaticLaser : MonoBehaviour
{
    [Header("����Sprite")]
    public SpriteRenderer laserBody;   // �м�����ɫ
    public SpriteRenderer laserGlow;   // ��Χ����ɫ����

    [Header("������˸����")]
    public float glowAlphaMin = 0.4f;  // ��͸��
    public float glowAlphaMax = 0.8f;  // ����
    public float glowSpeed = 2f;       // �����ٶ�

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

        // alpha ��˸�������� SpriteRenderer
        float alpha = Mathf.Lerp(glowAlphaMin, glowAlphaMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f);
        Color c = glowOriginalColor;
        c.a = alpha;
        laserGlow.color = c;
    }
}
