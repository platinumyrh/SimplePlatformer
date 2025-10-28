using System.Collections;

using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float stunTime = 0.3f;       // 硬直时间
    private float invincibleTime = 1f;   // 无敌时间
    private Vector2 knockback;           // 击退力度
    private bool isFinished;             // 是否结束受击状态
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public PlayerHitState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        sr = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        player.animator.Play("PlayerHit");

        isFinished = false;
        knockback = new Vector2(-Mathf.Sign(player.transform.localScale.x) * 2f, 1f);
        player.velocity = knockback;

        player.StartCoroutine(HitRoutine());
        player.StartCoroutine(CameraShake(0.2f, 0.1f));
        player.StartCoroutine(FlashRoutine(invincibleTime, 0.1f)); // ← 新增闪烁协程
    }

    public override void LogicUpdate()
    {
        if (isFinished)
        {
            if (!player.isGrounded)
                stateMachine.ChangeState(player.fallState);
            else if (Mathf.Abs(player.input.Horizontal) > 0.1f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        // 你这里直接赋值 velocity 就行，ApplyMovement 会处理
    }

    private IEnumerator HitRoutine()
    {
        player.isInvincible = true;

        yield return new WaitForSeconds(stunTime);
        isFinished = true;

        yield return new WaitForSeconds(invincibleTime - stunTime);
        player.isInvincible = false;
    }

    private IEnumerator FlashRoutine(float duration, float interval)
    {
        float timer = 0f;
        while (timer < duration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(interval);
            timer += interval;
        }
        sr.enabled = true; // 最后确保恢复可见
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }
}
