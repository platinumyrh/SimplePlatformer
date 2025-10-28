using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("血条设置")]
    public Transform fillBar;           // 血条可缩放部分
    public Transform followTarget;      // 血条跟随的目标对象
    public Vector3 offset = new Vector3(0, 0, 0); // 相对目标偏移
    public float followSpeed = 5f;      // 跟随速度

    private Vector3 initialScale;
    private Vector3 initialLocalPos;
    private PlayerController player;

    private void Awake()
    {
        if (fillBar == null)
        {
            Debug.LogError("请绑定 FillBar");
        }

        if (followTarget == null)
        {
            Debug.LogError("请在 Inspector 指定跟随的目标对象");
        }

        // 尝试获取 PlayerController，用于 UpdateHealth
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("找不到 PlayerController，请确保场景里有一个");
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

        // 平滑跟随目标
        Vector3 targetPos = followTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 始终面向摄像机
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    /// <summary>
    /// 根据 Player 当前血量更新血条显示
    /// </summary>
    public void UpdateHealth()
    {
        if (fillBar == null || player == null) return;

        float percent = Mathf.Clamp01((float)player.currentHealth / player.maxHealth);

        // 沿 Y 轴缩短血条
        Vector3 scale = initialScale;
        scale.y = initialScale.y * percent;
        fillBar.localScale = scale;

        // 保持上边缘固定
        float offsetY = (initialScale.y - scale.y) / 2f;
        fillBar.localPosition = initialLocalPos - new Vector3(0, offsetY, 0);
    }
}
