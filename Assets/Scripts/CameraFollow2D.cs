using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;           // 玩家 Transform
    [Header("偏移")]
    public Vector3 offset = new Vector3(0f, 1f, -10f); // 摄像机相对于玩家的偏移
    [Header("平滑")]
    public float smoothTime = 0.2f;    // 越小跟随越紧密
    private Vector3 velocity = Vector3.zero;

    

    void LateUpdate()
    {
        if (target == null) return;

        // 目标位置 = 玩家位置 + 偏移
        Vector3 targetPos = target.position + offset;

        // 平滑移动
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

       

        // 应用位置
        transform.position = smoothPos;
    }
}
