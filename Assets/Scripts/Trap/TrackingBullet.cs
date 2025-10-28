using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    [Header("�ӵ�����")]
    public float speed = 6f;           // �ƶ��ٶ�
    public float rotationSpeed = 360f; // ��ת�ٶȣ��Ӿ���ת��
    public float lifetime = 5f;        // ����ʱ��
    public int damage = 1;

    [Header("׷��")]
    public Transform player;           // ��� Transform

    [Header("����Ч��")]
    public ParticleSystem trailParticles;

    private void Start()
    {
        Destroy(gameObject, lifetime); // ��ʱ���Զ�����
    }

    private void Update()
    {
        if (player != null)
        {
            // ׷����ҷ����������㣩
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        // ��ת�Ӿ�Ч��
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(1);
            Destroy(gameObject);
        }

        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
