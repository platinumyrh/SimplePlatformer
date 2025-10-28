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

        // ���ݽ�ɫ���������������
        jumpDir = player.transform.localScale.x > 0 ? Vector2.left : Vector2.right;

        // �� velocity ���ó��ٶȣ����� + ��Ծ��
        player.velocity = new Vector2(jumpDir.x * player.moveSpeed, player.jumpForce);
    }

    public override void LogicUpdate()
    {
        // �ֶ���������
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * Time.deltaTime;

        // ײǽ����ؼ���� ApplyMovement ����

        // ��������ٶȣ���������״̬
        if (player.velocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        // ���ﲻ��Ҫ���⴦�� Rigidbody2D��ApplyMovement() �Ѿ�Ӧ�� velocity
    }
}
