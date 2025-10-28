using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    [Header("子弹属性")]
    public float speed = 6f;           // 移动速度
    public float rotationSpeed = 360f; // 自转速度（视觉旋转）
    public float lifetime = 5f;        // 存在时间
    public int damage = 1;

    [Header("追踪")]
    public Transform player;           // 玩家 Transform

    [Header("粒子效果")]
    public ParticleSystem trailParticles;

    private void Start()
    {
        Destroy(gameObject, lifetime); // 到时间自动销毁
    }

    private void Update()
    {
        if (player != null)
        {
            // 追踪玩家方向（向量计算）
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        // 自转视觉效果
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
