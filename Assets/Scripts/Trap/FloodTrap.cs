using UnityEngine;

public class FloodTrap : MonoBehaviour
{
    [Header("洪水参数")]
    public float floodSpeed = 0.1f;  // 水位上升速度
    public float maxFloodHeight = 10f;  // 最大水位
    public float currentFloodHeight = 0f;  // 当前水位

    [Header("Sprite")]
    public SpriteRenderer floodSprite;  // 用来显示洪水的 Sprite

    [Header("粒子系统")]
    public ParticleSystem floodParticles;  // 粒子系统

    [Header("固定位置")]
    public Vector3 fixedPosition;  // 固定位置，供修改

    private void Update()
    {
        // 控制水位上升
        if (currentFloodHeight < maxFloodHeight)
        {
            currentFloodHeight += floodSpeed * Time.deltaTime;
        }

        // 更新洪水Sprite的长度（通过改变scale的y值）
        UpdateFloodSprite();

        // 更新粒子位置，使其与水位同步
        UpdateParticlePosition();

        // 强制更新物体位置为固定位置
        transform.position = fixedPosition;
    }

    // 更新Sprite的高度（通过改变scale）
    private void UpdateFloodSprite()
    {
        if (floodSprite != null)
        {
            // 只更新Scale的y值，不会改变位置
            Vector3 scale = floodSprite.transform.localScale;
            scale.y = currentFloodHeight;  // 修改Y轴的scale来控制高度
            floodSprite.transform.localScale = scale;
        }
    }

    // 更新粒子位置，使其与水位同步
    private void UpdateParticlePosition()
    {
        if (floodParticles != null)
        {
            // 获取粒子系统的 Transform
            Transform particleTransform = floodParticles.transform;

            // 获取粒子系统当前的世界坐标
            Vector3 particlePosition = particleTransform.position;

            // 确保粒子位置不被重置，只更新Y轴
            particlePosition.y = currentFloodHeight;  // 粒子系统的Y位置与当前水位同步

            // 重新设置粒子系统的位置，但保持X和Z坐标不变
            particleTransform.position = new Vector3(particlePosition.x, particlePosition.y, particlePosition.z);
        }
    }

    // 触发器：玩家进入水域时触发
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 确保是玩家
        if (other.CompareTag("Player"))
        {
            // 获取玩家脚本并触发伤害
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();  // 设置伤害值
            }
        }
    }

    // 获取当前水位
    public float GetFloodHeight() => currentFloodHeight;
}
