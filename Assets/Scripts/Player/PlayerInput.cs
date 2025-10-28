using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Horizontal => Input.GetAxisRaw("Horizontal");
    public bool JumpPressed => Input.GetButtonDown("Jump");
    public bool JumpHeld => Input.GetButton("Jump");
    public bool DashPressed => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
}
