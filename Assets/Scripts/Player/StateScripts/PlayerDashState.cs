using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime;
    private Vector2 dashDirection;

    public PlayerDashState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }


    public override void Enter()
    {
        player.isInvincible = true;
        player.animator.Play("PlayerDash");
        dashTime = player.dashDuration;
        dashDirection = new Vector2(player.input.Horizontal, 0).normalized;

        if (dashDirection.x == 0)
        {
            dashDirection.x = Mathf.Sign(player.transform.localScale.x);
        }
        player.lastDashTime = Time.time;
    }

    public override void LogicUpdate()
    {
        dashTime-=Time.deltaTime;
        if (dashTime <= 0)
        {
            player.isInvincible = false;
            if (!player.isGrounded)
            {
                stateMachine.ChangeState(player.fallState);
            }

            else if(Mathf.Abs(player.input.Horizontal)>0.1)         
                stateMachine.ChangeState(player.runState);       
            else
                stateMachine.ChangeState(player.idleState);
        }
        player.velocity.x = dashDirection.x * player.dashSpeed;
        player.velocity.y = 0; // 冲刺期间可以选择固定 y
       
    }
    public override void PhysicsUpdate()
    {
        player.transform.Translate(player.velocity * Time.deltaTime);
    }
}

