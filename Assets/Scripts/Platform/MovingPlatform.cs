using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;       // 平台的路径点
    public float speed = 2f;         // 平台移动速度
    private int targetIndex = 0;     // 当前目标点索引

    private Vector3 lastPosition;    // 上一帧位置
    private Vector3 velocity;        // 当前速度

    private HashSet<Transform> passengers = new HashSet<Transform>(); // 踩在平台上的角色

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (points.Length == 0) return;

        // 移动到目标点
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[targetIndex].position,
            speed * Time.deltaTime
        );

        // 判断是否到达目标点
        if (Vector2.Distance(transform.position, points[targetIndex].position) < 0.05f)
        {
            targetIndex++;
            if (targetIndex >= points.Length)
            {
                targetIndex = 0; // 循环
            }
        }

        // 计算平台速度
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        // 把平台的位移应用到乘客
        foreach (var p in passengers)
        {
            p.position += velocity * Time.deltaTime;
        }

        lastPosition = transform.position;
    }

    // 用 Trigger 边界检测谁在平台上
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
