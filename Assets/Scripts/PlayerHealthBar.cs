using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Ѫ������")]
    public Transform fillBar;           // Ѫ�������Ų���
    public Transform followTarget;      // Ѫ�������Ŀ�����
    public Vector3 offset = new Vector3(0, 0, 0); // ���Ŀ��ƫ��
    public float followSpeed = 5f;      // �����ٶ�

    private Vector3 initialScale;
    private Vector3 initialLocalPos;
    private PlayerController player;

    private void Awake()
    {
        if (fillBar == null)
        {
            Debug.LogError("��� FillBar");
        }

        if (followTarget == null)
        {
            Debug.LogError("���� Inspector ָ�������Ŀ�����");
        }

        // ���Ի�ȡ PlayerController������ UpdateHealth
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("�Ҳ��� PlayerController����ȷ����������һ��");
        }

        if (fillBar != null)
        {
            initialScale = fillBar.localScale;
            initialLocalPos = fillBar.localPosition;
        }
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;

        // ƽ������Ŀ��
        Vector3 targetPos = followTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // ʼ�����������
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    /// <summary>
    /// ���� Player ��ǰѪ������Ѫ����ʾ
    /// </summary>
    public void UpdateHealth()
    {
        if (fillBar == null || player == null) return;

        float percent = Mathf.Clamp01((float)player.currentHealth / player.maxHealth);

        // �� Y ������Ѫ��
        Vector3 scale = initialScale;
        scale.y = initialScale.y * percent;
        fillBar.localScale = scale;

        // �����ϱ�Ե�̶�
        float offsetY = (initialScale.y - scale.y) / 2f;
        fillBar.localPosition = initialLocalPos - new Vector3(0, offsetY, 0);
    }
}
