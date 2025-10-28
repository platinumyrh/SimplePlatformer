using UnityEngine;

public class Laser2D : MonoBehaviour
{
    [Header("��������")]
    public Transform laserOrigin;      // �����
    public SpriteRenderer laserBody;   // �м���ɫ����
    public SpriteRenderer laserGlow;   // ��Χ��ɫ����
    public float maxLaserLength = 10f; // ����������
    public LayerMask obstacleMask;     // ��ײ��
    public float minLaserLength = 0.1f; // ��̹�����������ʧ

    [Header("����Ч��")]
    public float glowAlphaMin = 0.4f;  // ������͸��
    public float glowAlphaMax = 0.8f;  // ��������
    public float glowSpeed = 2f;       // �����ٶ�

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
        // ���߼���һ���ϰ���
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, transform.up, maxLaserLength, obstacleMask);
        float laserLength = maxLaserLength;

        if (hit.collider != null)
        {
            laserLength = hit.distance;
        }

        // ��ֹ scale Ϊ 0
        laserLength = Mathf.Max(laserLength, minLaserLength);

        // ���¹���λ�ú�����
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
            // ʹ�� alpha ��˸�������� SpriteRenderer
            float alpha = Mathf.Lerp(glowAlphaMin, glowAlphaMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f);
            Color c = glowOriginalColor;
            c.a = alpha;
            laserGlow.color = c;
        }
    }
}
