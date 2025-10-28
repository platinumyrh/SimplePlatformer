using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;       // ƽ̨��·����
    public float speed = 2f;         // ƽ̨�ƶ��ٶ�
    private int targetIndex = 0;     // ��ǰĿ�������

    private Vector3 lastPosition;    // ��һ֡λ��
    private Vector3 velocity;        // ��ǰ�ٶ�

    private HashSet<Transform> passengers = new HashSet<Transform>(); // ����ƽ̨�ϵĽ�ɫ

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (points.Length == 0) return;

        // �ƶ���Ŀ���
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[targetIndex].position,
            speed * Time.deltaTime
        );

        // �ж��Ƿ񵽴�Ŀ���
        if (Vector2.Distance(transform.position, points[targetIndex].position) < 0.05f)
        {
            targetIndex++;
            if (targetIndex >= points.Length)
            {
                targetIndex = 0; // ѭ��
            }
        }

        // ����ƽ̨�ٶ�
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        // ��ƽ̨��λ��Ӧ�õ��˿�
        foreach (var p in passengers)
        {
            p.position += velocity * Time.deltaTime;
        }

        lastPosition = transform.position;
    }

    // �� Trigger �߽���˭��ƽ̨��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            passengers.Add(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            passengers.Remove(other.transform);
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
