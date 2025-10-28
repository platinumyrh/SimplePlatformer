using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 climbTarget;

    public PlayerLedgeClimbState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        float dir = Mathf.Sign(player.transform.localScale.x);
        // ���㶯����ʼλ�ã���ץǽ�ĵ㣩
        Vector2 preClimbPos = (Vector2)player.transform.position + new Vector2(dir * -0.05f, -0.1f);
        player.transform.position = preClimbPos;
        player.animator.Play("PlayerEdgeClimb");
        player.velocity = Vector2.zero;
        // ��������Ŀ��λ�ã�ʾ�⣬�ɸ���ʵ��Sprite/Collider������
       
        climbTarget = (Vector2)player.transform.position + new Vector2(dir * 0.2f, 0.5f);
      
    }

    public override void LogicUpdate()
    {
        // �̶�λ�ã����ƶ�
        player.velocity = Vector2.zero;

        // ��ȡ��ǰ����״̬��Ϣ
        AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);

        // �ж� PlayerEdgeClimb �������
        if (info.IsName("PlayerEdgeClimb") && info.normalizedTime >= 1f)
        {
            player.transform.position = climbTarget;
            stateMachine.ChangeState(player.idleState);
        }
    }
   
}
