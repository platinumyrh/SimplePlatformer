using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    [Header("子弹")]
    public GameObject bulletPrefab;
    public Transform firePoint;         // 发射点（子弹生成位置）
    public float fireInterval = 1f;     // 发射间隔

    [Header("警戒")]
    public Collider2D detectionArea;    // 警戒范围
    public SpriteRenderer alertSprite;  // 玩家进入警戒时变色
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;

    private float fireTimer = 0f;
    private Transform player;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                FireBullet();
                fireTimer = 0f;
            }
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Debug.Log("发射！");
            TrackingBullet tb = bullet.GetComponent<TrackingBullet>();
            if (tb != null)
                tb.player = player; // 设置追踪目标
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
            if (alertSprite != null) alertSprite.color = alertColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerInRange = false;
            if (alertSprite != null) alertSprite.color = normalColor;
        }
    }
}
