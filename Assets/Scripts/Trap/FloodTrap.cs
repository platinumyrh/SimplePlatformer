using UnityEngine;

public class FloodTrap : MonoBehaviour
{
    [Header("��ˮ����")]
    public float floodSpeed = 0.1f;  // ˮλ�����ٶ�
    public float maxFloodHeight = 10f;  // ���ˮλ
    public float currentFloodHeight = 0f;  // ��ǰˮλ

    [Header("Sprite")]
    public SpriteRenderer floodSprite;  // ������ʾ��ˮ�� Sprite

    [Header("����ϵͳ")]
    public ParticleSystem floodParticles;  // ����ϵͳ

    [Header("�̶�λ��")]
    public Vector3 fixedPosition;  // �̶�λ�ã����޸�

    private void Update()
    {
        // ����ˮλ����
        if (currentFloodHeight < maxFloodHeight)
        {
            currentFloodHeight += floodSpeed * Time.deltaTime;
        }

        // ���º�ˮSprite�ĳ��ȣ�ͨ���ı�scale��yֵ��
        UpdateFloodSprite();

        // ��������λ�ã�ʹ����ˮλͬ��
        UpdateParticlePosition();

        // ǿ�Ƹ�������λ��Ϊ�̶�λ��
        transform.position = fixedPosition;
    }

    // ����Sprite�ĸ߶ȣ�ͨ���ı�scale��
    private void UpdateFloodSprite()
    {
        if (floodSprite != null)
        {
            // ֻ����Scale��yֵ������ı�λ��
            Vector3 scale = floodSprite.transform.localScale;
            scale.y = currentFloodHeight;  // �޸�Y���scale�����Ƹ߶�
            floodSprite.transform.localScale = scale;
        }
    }

    // ��������λ�ã�ʹ����ˮλͬ��
    private void UpdateParticlePosition()
    {
        if (floodParticles != null)
        {
            // ��ȡ����ϵͳ�� Transform
            Transform particleTransform = floodParticles.transform;

            // ��ȡ����ϵͳ��ǰ����������
            Vector3 particlePosition = particleTransform.position;

            // ȷ������λ�ò������ã�ֻ����Y��
            particlePosition.y = currentFloodHeight;  // ����ϵͳ��Yλ���뵱ǰˮλͬ��

            // ������������ϵͳ��λ�ã�������X��Z���겻��
            particleTransform.position = new Vector3(particlePosition.x, particlePosition.y, particlePosition.z);
        }
    }

    // ����������ҽ���ˮ��ʱ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ȷ�������
        if (other.CompareTag("Player"))
        {
            // ��ȡ��ҽű��������˺�
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();  // �����˺�ֵ
            }
        }
    }

    // ��ȡ��ǰˮλ
    public float GetFloodHeight() => currentFloodHeight;
}
