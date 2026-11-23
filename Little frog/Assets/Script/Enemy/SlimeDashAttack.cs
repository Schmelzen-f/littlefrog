
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyAttack))]
public class SlimeDashAttack : EnemyAttackBehaviour
{

    private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float prepareTime = 0.7f;
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float dashDistance = 2.5f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("References")]
    
    [SerializeField] private EnemyAttack enemyAttack;

    private Rigidbody2D enemyRb;
    private Transform target;

    private float prepareTimer;
    private float cooldownTimer;

    private Vector2 lastMoveDirection = Vector2.down;
    private Vector2 dashDirection;
    private Vector2 dashStartPosition;

    private enum AttackState
    {
        IDLE,
        PREPARING,
        DASHING
    }

    private AttackState currentAttackState = AttackState.IDLE;

    // ================= PROPERTIES =================

    public override float AttackRange => enemyAttack.attackRange;

    public override bool IsAttacking =>
        currentAttackState != AttackState.IDLE;

    public override bool CanStartAttack =>
        currentAttackState == AttackState.IDLE &&
        cooldownTimer <= 0f;

    public bool IsDashing =>
        currentAttackState == AttackState.DASHING;


    // ================= INITIALIZE =================

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (enemyAttack == null)
            enemyAttack = GetComponent<EnemyAttack>();
    }


    // ================= UPDATE =================

    private void Update()
    {
        // Countdown cooldown
        if (cooldownTimer > 0f)
        {
            cooldownTimer = Mathf.Max(
                0f,
                cooldownTimer - Time.deltaTime
            );
        }

        // Chỉ xử lý timer khi đang chuẩn bị
        if (currentAttackState != AttackState.PREPARING)
            return;

        if (target == null)
        {
            CancelPrepare();
            return;
        }

        prepareTimer -= Time.deltaTime;

        if (prepareTimer <= 0f)
        {
            Debug.Log("Buoc 2");
            BeginDashAttack();
        }
    }


    // ================= START ATTACK =================

    public override void StartAttack(Transform attackTarget)
    {
        Debug.Log("StartAttack");
        if (!CanStartAttack || attackTarget == null)
            return;

        target = attackTarget;

        currentAttackState = AttackState.PREPARING;

        prepareTimer = prepareTime;

        enemyRb.linearVelocity = Vector2.zero;

        //animator.SetBool("isWalking", false);
        //animator.SetBool("isPreparingAttack", true);
        animator.SetBool("isDashing", true);

        // Quay mặt về phía Player
        Vector2 direction =
            (target.position - transform.position);

        if (direction.sqrMagnitude > 0.001f)
        {
            Debug.Log("dcmmm");
            lastMoveDirection = direction.normalized;
        }

        UpdateFacing(lastMoveDirection);

        Debug.Log("Buoc 1");
    }


    // ================= CANCEL PREPARE =================

    private void CancelPrepare()
    {
        if (currentAttackState != AttackState.PREPARING)
            return;

        enemyRb.linearVelocity = Vector2.zero;

        currentAttackState = AttackState.IDLE;
        animator.SetBool("isDashing", false);
        //animator.SetBool("isPreparingAttack", false);

        target = null;
    }


    // ================= BEGIN DASH =================

    private void BeginDashAttack()
    {

        if (target == null)
        {
            CancelPrepare();
            return;
        }
        Debug.Log("BeginDashAttack");

        // Khóa hướng tại thời điểm bắt đầu Dash
        Vector2 direction =
            (target.position - transform.position);

        if (direction.sqrMagnitude < 0.001f)
        {
           
            direction = lastMoveDirection;
        }
        dashDirection = direction.normalized;

        dashStartPosition = enemyRb.position;

        currentAttackState = AttackState.DASHING;

        //animator.SetBool("isPreparingAttack", false);
        animator.SetBool("isDashing", true);
        //animator.SetBool("isWalking", false);

        lastMoveDirection = dashDirection;

        UpdateFacing(dashDirection);

        enemyRb.linearVelocity = dashDirection * dashSpeed;
    }


    // ================= DASH MOVEMENT =================

    private void FixedUpdate()
    {
        if (currentAttackState != AttackState.DASHING)
            return;

        float traveledDistance = Vector2.Dot(
            enemyRb.position - dashStartPosition,
            dashDirection
        );

        float remainingDistance =
            dashDistance - traveledDistance;

        // Đã đi đủ khoảng cách
        if (remainingDistance <= 0.01f)
        {
            EndDash();
        }
        else
        {
            // Giới hạn vận tốc ở bước cuối
            float currentDashSpeed = Mathf.Min(
                dashSpeed,
                remainingDistance / Time.fixedDeltaTime
            );

            enemyRb.linearVelocity =
                dashDirection * currentDashSpeed;
        }
        Debug.Log("Fix");
    }


    // ================= END DASH =================

    public void EndDash()
    {
        if (currentAttackState != AttackState.DASHING)
            return;

        enemyRb.linearVelocity = Vector2.zero;

        currentAttackState = AttackState.IDLE;

       
        animator.SetBool("isDashing", false);
        
        // Bắt đầu cooldown sau khi Dash kết thúc
        cooldownTimer = dashCooldown;

        //target = null;
    }


    // ================= ANIMATION FACING =================

    private void UpdateFacing(Vector2 direction)
    {
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);

        animator.SetFloat("LastInputX", direction.x);
        animator.SetFloat("LastInputY", direction.y);
    }
}
