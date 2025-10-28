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
        // ˮƽ�ƶ�
        player.velocity.x = player.input.Horizontal * player.moveSpeed;

        // �ֶ���������
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * Time.deltaTime;

        // ������
        if (player.input.JumpPressed && player.jumpCount < player.maxJumpCount)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // �ŵؼ���� ApplyMovement() �����߼�⴦��
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
        // Rigidbody2D ���ٶȻ��� PlayerController.ApplyMovement() ��Ӧ��
    }
}
