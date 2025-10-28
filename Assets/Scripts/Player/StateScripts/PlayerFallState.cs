using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerController player, StateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.Play("PlayerFall");
    }

    public override void LogicUpdate()
    {
        // 水平移动
        player.velocity.x = player.input.Horizontal * player.moveSpeed;

        // 手动处理重力
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * Time.deltaTime;

        // 二段跳
        if (player.input.JumpPressed && player.jumpCount < player.maxJumpCount)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // 着地检测由 ApplyMovement() 的射线检测处理
        if (player.detector.IsGrounded())
        {
            if (Mathf.Abs(player.input.Horizontal) > 0.1f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        // Rigidbody2D 的速度会在 PlayerController.ApplyMovement() 里应用
    }
}
