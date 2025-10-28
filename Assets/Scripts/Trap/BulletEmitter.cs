using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    [Header("�ӵ�")]
    public GameObject bulletPrefab;
    public Transform firePoint;         // ����㣨�ӵ�����λ�ã�
    public float fireInterval = 1f;     // ������

    [Header("����")]
    public Collider2D detectionArea;    // ���䷶Χ
    public SpriteRenderer alertSprite;  // ��ҽ��뾯��ʱ��ɫ
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
            Debug.Log("���䣡");
            TrackingBullet tb = bullet.GetComponent<TrackingBullet>();
            if (tb != null)
                tb.player = player; // ����׷��Ŀ��
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
