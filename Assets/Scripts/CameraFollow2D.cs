using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("����Ŀ��")]
    public Transform target;           // ��� Transform
    [Header("ƫ��")]
    public Vector3 offset = new Vector3(0f, 1f, -10f); // ������������ҵ�ƫ��
    [Header("ƽ��")]
    public float smoothTime = 0.2f;    // ԽС����Խ����
    private Vector3 velocity = Vector3.zero;

    

    void LateUpdate()
    {
        if (target == null) return;

        // Ŀ��λ�� = ���λ�� + ƫ��
        Vector3 targetPos = target.position + offset;

        // ƽ���ƶ�
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

       

        // Ӧ��λ��
        transform.position = smoothPos;
    }
}
