using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallStickState : PlayerState
{
    public PlayerWallStickState(PlayerController player, StateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.Play("PlayerWallStick");
        player.velocity.y = 0f;
        player.rb.velocity = player.velocity; // 同步刚体
    }

    public override void LogicUpdate()
    {
        // 下滑效果：用 gravityScale 控制滑落
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * 0.2f * Time.deltaTime;

        // 跳离墙面
        if (player.input.JumpPressed)
        {
            player.velocity.x = -Mathf.Sign(player.transform.localScale.x) * player.moveSpeed * 1.2f;
            player.velocity.y = player.jumpForce;
            player.rb.velocity = player.velocity; // 同步刚体
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // 检测是否离开墙或落地
        bool wallLeft = player.detector.IsTouchingWall(Vector2.left);
        bool wallRight = player.detector.IsTouchingWall(Vector2.right);
        if (!(wallLeft || wallRight) || player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // 水平速度置零，保证贴墙停留
        player.velocity.x = 0f;
        player.rb.velocity = player.velocity;
    }
}
