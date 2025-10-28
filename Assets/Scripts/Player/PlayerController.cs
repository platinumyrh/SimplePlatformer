using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float gravityScale = 20f;

    [Header("��ײ���")]
    public LayerMask groundLayer;
    public LayerMask wallMask;

    [Header("����")]
    public Animator animator;

    [Header("��̲���")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [HideInInspector] public float lastDashTime;

    [Header("Ѫ������")]
    public PlayerHealthBar healthBar;
    public int maxHealth = 5;
    public int currentHealth;

    [Header("��Ծ����")]
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

        // ��ʼ��״̬
        wallStickState = new PlayerWallStickState(this, stateMachine);
        wallJumpState = new PlayerWallJumpState(this, stateMachine);
        ledgeClimbState = new PlayerLedgeClimbState(this, stateMachine);
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        fallState = new PlayerFallState(this, stateMachine);
        hitState = new PlayerHitState(this, stateMachine);
        
        // ����TimeAnchor
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
        // ������ڻ��ݣ��������봦��
        if (isRewinding) return;
        
        // ���߼�����
        isGrounded = detector.IsGrounded();
        if (isGrounded && velocity.y < 0)
            velocity.y = 0;

        // ״̬�߼�
        stateMachine.CurrentState.LogicUpdate();

        // �������
        if (input.DashPressed && CanDash())
            stateMachine.ChangeState(dashState);

        // �ֶ����ݲ��ԣ���R����
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
        // ���д�������
        velocity.y -= gravityScale * Time.fixedDeltaTime;

        if (isRewinding)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // ǽ����
        if (velocity.x != 0 && detector.IsTouchingWall(Vector2.right * Mathf.Sign(velocity.x)))
        {
            velocity.x = 0;
        }

        // �����⣨�½�ʱ��
        if (velocity.y < 0 && detector.IsGrounded())
        {
            velocity.y = 0;
            isGrounded = true;
        }

        // �� velocity Ӧ�õ�����
        rb.velocity = velocity;

        // ��ת��ɫ
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
        Debug.Log("�������");
        
        // ֱ�����س��������ٳ����Զ�����
        Debug.Log("���س���");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnRewindComplete()
    {
        // ������ɺ�ָ����״̬
        if (timeAnchor != null)
        {
            currentHealth = timeAnchor.Anchor.hp;
            Debug.Log("������ɣ��ָ�Ѫ��: " + currentHealth);
        }
        
        isRewinding = false;
        
        if (healthBar != null)
            healthBar.UpdateHealth();
            
        // �ص�����״̬
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

    // getter ����
    public Vector3 GetPosition() => transform.position;
    public Vector2 GetVelocity() => velocity;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetJumpCount() => jumpCount;
    public string GetCurrentStateType() => stateMachine.CurrentState.GetType().Name;
    public bool GetIsInvincible() => isInvincible;
}