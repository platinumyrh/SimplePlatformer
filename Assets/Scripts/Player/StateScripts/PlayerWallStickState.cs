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
        player.rb.velocity = player.velocity; // ͬ������
    }

    public override void LogicUpdate()
    {
        // �»�Ч������ gravityScale ���ƻ���
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * 0.2f * Time.deltaTime;

        // ����ǽ��
        if (player.input.JumpPressed)
        {
            player.velocity.x = -Mathf.Sign(player.transform.localScale.x) * player.moveSpeed * 1.2f;
            player.velocity.y = player.jumpForce;
            player.rb.velocity = player.velocity; // ͬ������
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // ����Ƿ��뿪ǽ�����
        bool wallLeft = player.detector.IsTouchingWall(Vector2.left);
        bool wallRight = player.detector.IsTouchingWall(Vector2.right);
        if (!(wallLeft || wallRight) || player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // ˮƽ�ٶ����㣬��֤��ǽͣ��
        player.velocity.x = 0f;
        player.rb.velocity = player.velocity;
    }
}
