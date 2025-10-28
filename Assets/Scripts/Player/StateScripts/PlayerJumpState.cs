using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player, StateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.jumpCount++;
        player.animator.Play(player.jumpCount == 1 ? "PlayerJump" : "PlayerAirJump");
        player.velocity.y = player.jumpForce;
    }

    public override void LogicUpdate()
    {
        // ˮƽ�ƶ�
        player.velocity.x = player.input.Horizontal * player.moveSpeed;

        // ����
        player.velocity.y += Physics2D.gravity.y * player.gravityScale * Time.deltaTime;

        // ������
        if (player.input.JumpPressed && player.jumpCount < player.maxJumpCount)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // ���䴦��
        if (player.velocity.y < 0f)
        {
            // ��������̨��
            if (player.detector.CanClimbLedge(Vector2.right * Mathf.Sign(player.transform.localScale.x)))
            {
                stateMachine.ChangeState(player.ledgeClimbState);
                return;
            }

            // ǽ�����
            bool wallLeft = player.detector.IsTouchingWall(Vector2.left);
            bool wallRight = player.detector.IsTouchingWall(Vector2.right);
            if (wallLeft || wallRight)
            {
                // ��������
                player.transform.localScale = new Vector3(
                    Mathf.Sign(wallRight ? 1f : -1f) * Mathf.Abs(player.transform.localScale.x),
                    player.transform.localScale.y,
                    player.transform.localScale.z
                );
                stateMachine.ChangeState(player.wallStickState);
                return;
            }

            // û��ǽҲû��������������
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
}
