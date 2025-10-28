using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float gravityScale = 20f;

    [Header("碰撞检测")]
    public LayerMask groundLayer;
    public LayerMask wallMask;

    [Header("动画")]
    public Animator animator;

    [Header("冲刺参数")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [HideInInspector] public float lastDashTime;

    [Header("血条设置")]
    public PlayerHealthBar healthBar;
    public int maxHealth = 5;
    public int currentHealth;

    [Header("跳跃参数")]
    public int maxJumpCount = 2;
    [HideInInspector] public int jumpCount = 0;

    [HideInInspector] public PlayerInput input;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isRewinding = false;

    public StateMachine stateMachine;
    public Rigidbody2D rb;

    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerJumpState jumpState;
    public PlayerDashState dashState;
    public PlayerFallState fallState;
    public PlayerEnvironmentDetector detector;
    public PlayerWallStickState wallStickState;
    public PlayerWallJumpState wallJumpState;
    public PlayerLedgeClimbState ledgeClimbState;
    public PlayerHitState hitState;

    public bool isInvincible;
    
    private TimeAnchor timeAnchor;

    private void Awake()
    {
        currentHealth = maxHealth;
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        stateMachine = new StateMachine();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        detector = new PlayerEnvironmentDetector(transform, collider, wallMask, groundLayer);

        // 初始化状态
        wallStickState = new PlayerWallStickState(this, stateMachine);
        wallJumpState = new PlayerWallJumpState(this, stateMachine);
        ledgeClimbState = new PlayerLedgeClimbState(this, stateMachine);
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        fallState = new PlayerFallState(this, stateMachine);
        hitState = new PlayerHitState(this, stateMachine);
        
        // 查找TimeAnchor
        timeAnchor = FindObjectOfType<TimeAnchor>();
    }

    private void Start()
    {
        if (timeAnchor != null)
        {
            timeAnchor.onRewindComplete += OnRewindComplete;
        }
        
        stateMachine.Initialize(idleState);
        
        if (healthBar != null)
            healthBar.UpdateHealth();
    }

    private void OnDestroy()
    {
        if (timeAnchor != null)
        {
            timeAnchor.onRewindComplete -= OnRewindComplete;
        }
    }

    private void Update()
    {
        // 如果正在回溯，跳过输入处理
        if (isRewinding) return;
        
        // 射线检测地面
        isGrounded = detector.IsGrounded();
        if (isGrounded && velocity.y < 0)
            velocity.y = 0;

        // 状态逻辑
        stateMachine.CurrentState.LogicUpdate();

        // 冲刺输入
        if (input.DashPressed && CanDash())
            stateMachine.ChangeState(dashState);

        // 手动回溯测试（按R键）
        if (Input.GetKeyDown(KeyCode.R) && timeAnchor != null)
        {
            timeAnchor.StartRewind();
        }
    }

    private void FixedUpdate()
    {
        if (isRewinding) return;
        
        stateMachine.CurrentState.PhysicsUpdate();
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        // 自行处理重力
        velocity.y -= gravityScale * Time.fixedDeltaTime;

        if (isRewinding)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 墙面检测
        if (velocity.x != 0 && detector.IsTouchingWall(Vector2.right * Mathf.Sign(velocity.x)))
        {
            velocity.x = 0;
        }

        // 地面检测（下降时）
        if (velocity.y < 0 && detector.IsGrounded())
        {
            velocity.y = 0;
            isGrounded = true;
        }

        // 将 velocity 应用到刚体
        rb.velocity = velocity;

        // 翻转角色
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(rb.velocity.x);
            transform.localScale = scale;
        }
    }

    private bool CanDash() => Time.time >= lastDashTime + dashCooldown;

    public void ResetJumpCount() => jumpCount = 0;

    public void TakeDamage(int damage)
    {
        if (isInvincible || isRewinding) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthBar != null)
            healthBar.UpdateHealth();

        stateMachine.ChangeState(hitState);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("玩家死亡");
        
        // 直接重载场景，不再尝试自动回溯
        Debug.Log("重载场景");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnRewindComplete()
    {
        // 回溯完成后恢复玩家状态
        if (timeAnchor != null)
        {
            currentHealth = timeAnchor.Anchor.hp;
            Debug.Log("回溯完成，恢复血量: " + currentHealth);
        }
        
        isRewinding = false;
        
        if (healthBar != null)
            healthBar.UpdateHealth();
            
        // 回到空闲状态
        stateMachine.ChangeState(idleState);
    }

    public void SetHealthWithoutNotification(int newHealth)
    {
        currentHealth = newHealth;
    }

    public void SetHealth(int newHealth)
    {
        currentHealth = newHealth;
        if (healthBar != null)
            healthBar.UpdateHealth();
    }

    // getter 方法
    public Vector3 GetPosition() => transform.position;
    public Vector2 GetVelocity() => velocity;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetJumpCount() => jumpCount;
    public string GetCurrentStateType() => stateMachine.CurrentState.GetType().Name;
    public bool GetIsInvincible() => isInvincible;
}