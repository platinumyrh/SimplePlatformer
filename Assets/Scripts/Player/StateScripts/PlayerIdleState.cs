using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.Play("PlayerIdle");
        player.velocity.x = 0;
        player.ResetJumpCount();
    }

    public override void LogicUpdate()
    {
        if (Mathf.Abs(player.input.Horizontal) > 0.1f)
        {
            stateMachine.ChangeState(player.runState);
            
        }
        else if (player.input.JumpPressed  && player.jumpCount < player.maxJumpCount)
        {
          
            stateMachine.ChangeState(player.jumpState);
         
        }
        else if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
        }

        player.velocity.x = 0;
    }
}
