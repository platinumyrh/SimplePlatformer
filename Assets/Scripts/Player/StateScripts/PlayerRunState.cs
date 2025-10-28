using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }


    public override void Enter()
    {
        player.animator.Play("PlayerRun");
        player.ResetJumpCount();
    }

    public override void LogicUpdate()
    {
        if (Mathf.Abs(player.input.Horizontal) < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (player.input.JumpPressed && player.jumpCount < player.maxJumpCount)
        {
           
            stateMachine.ChangeState(player.jumpState);
        }
        else if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
        }

        player.velocity.x = player.input.Horizontal * player.moveSpeed;
    }
}
