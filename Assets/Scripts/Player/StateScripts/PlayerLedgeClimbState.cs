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
        // 计算动画起始位置（手抓墙的点）
        Vector2 preClimbPos = (Vector2)player.transform.position + new Vector2(dir * -0.05f, -0.1f);
        player.transform.position = preClimbPos;
        player.animator.Play("PlayerEdgeClimb");
        player.velocity = Vector2.zero;
        // 计算攀爬目标位置（示意，可根据实际Sprite/Collider调整）
       
        climbTarget = (Vector2)player.transform.position + new Vector2(dir * 0.2f, 0.5f);
      
    }

    public override void LogicUpdate()
    {
        // 固定位置，不移动
        player.velocity = Vector2.zero;

        // 获取当前动画状态信息
        AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);

        // 判断 PlayerEdgeClimb 播放完成
        if (info.IsName("PlayerEdgeClimb") && info.normalizedTime >= 1f)
        {
            player.transform.position = climbTarget;
            stateMachine.ChangeState(player.idleState);
        }
    }
   
}
