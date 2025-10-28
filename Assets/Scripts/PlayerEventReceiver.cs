using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventReceiver : MonoBehaviour
{
    // 接收残留事件
    public void OnLedgeClimbEnd() { }

    // 带参数也可以防止报错
    public void OnLedgeClimbEnd(AnimationEvent e) { }
}
