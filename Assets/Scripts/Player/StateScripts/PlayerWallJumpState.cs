using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private Vector2 jumpDir;

    public PlayerWallJumpState(PlayerController player, StateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.Play("PlayerJump");

        // 根据角色朝向决定反方向跳
        jumpDir = player.transform.localScale.x > 0 ? Vector2.left : Vector2.right;

        // 给 velocity 设置初速度（横向 + 跳跃）
        player.velocity = new Vector2(jumpDir.x * player.moveSpeed, player.jumpForce);
    }

    public override void LogicUpdate()
    {
        // 手动处理重力
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * Time.deltaTime;

        // 撞墙或落地检测由 ApplyMovement 处理

        // 如果向下速度，进入下落状态
        if (player.velocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        // 这里不需要额外处理 Rigidbody2D，ApplyMovement() 已经应用 velocity
    }
}
