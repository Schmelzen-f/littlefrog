using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    //[SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 6f;
    
    
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;

    // DASH VARIABLES
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.15f;

    private Vector2 dashDirection;
    private float dashTimer;

    [SerializeField] private float dashCooldown = 0.5f;
    private float dashCooldownTimer = 0f;

    // HEALTH BAR
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    //ATTACK
    public PlayerAttack playerattack;
    private bool hasStartedAttack = false;
    public Transform Attackarea;


    //KNOCKBACK
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.25f;

    private float knockbackTimer;
    private Vector2 knockbackDirection;
    private bool applyKnockback;

    enum PlayerState { IDLE, WALK, RUNNING, DASH, ATTACK, KNOCKBACK}



    private PlayerState currentState  = PlayerState.IDLE;
    private bool isSprinting = false;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }


    private void Update()
    {
    
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        

        switch (currentState)
        {
            case PlayerState.IDLE:
                Idle();
                break;

            case PlayerState.WALK:
                Walk();
                break;

            case PlayerState.RUNNING:
                Running();
                break;

            case PlayerState.DASH:
                Dash();
                break;

            case PlayerState.ATTACK:
                Attack();
                break;
            case PlayerState.KNOCKBACK:
                Knockback();
                break;
            
                

        }
    }



    public void UDieFuckingIdiot(int damage, Vector2 enemyPosition)
    {

        currentHealth -= damage;

        spriteRenderer.color = Color.red;


        knockbackDirection =
       ((Vector2)transform.position - enemyPosition).normalized;

        knockbackTimer = knockbackDuration;
        applyKnockback = true;

        currentState = PlayerState.KNOCKBACK;
    }

    

    void Idle()
    {
        Debug.Log("Idle");
        if (moveInput == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            currentState = PlayerState.WALK;
        }
    }


    void Walk()
    {
        Debug.Log("Walk");
        if (moveInput != Vector2.zero && isSprinting == false)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
        else if (moveInput != Vector2.zero && isSprinting == true)
        {
            animator.SetBool("isWalking", true);
            currentState = PlayerState.RUNNING;
        }
        else
        {
            animator.SetBool("isWalking", false);
            currentState = PlayerState.IDLE;
        }
    }

    void Running()
    {
        Debug.Log("Running");
        if (moveInput != Vector2.zero && isSprinting == true)
        {
            
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
        else if (moveInput != Vector2.zero && isSprinting == false)
        {
            animator.SetBool("isWalking", true);
            currentState = PlayerState.WALK;
        }
        else
        {
            animator.SetBool("isWalking", false);
            currentState = PlayerState.IDLE;
        }
    }

    void Dash()
    {
        dashTimer -= Time.deltaTime;
        rb.linearVelocity = dashDirection * dashSpeed;

        if (dashTimer <= 0f)
        {
            Physics2D.IgnoreLayerCollision(3, 6, false);
            if (moveInput == Vector2.zero)
            {
                currentState = PlayerState.IDLE;
            }
            else
            {
                currentState = PlayerState.WALK;
            }
        }

    }


    void Attack()
    {
        if (currentState == PlayerState.ATTACK)
        {
         
            hasStartedAttack = false;
            currentState = PlayerState.IDLE;
        }


        if (!hasStartedAttack)
        {
         
            playerattack.StartAttack();
            Debug.Log("HIT!!!");
            hasStartedAttack = true;
            
        }

        
        
    }



    void Knockback()
    {
        knockbackTimer -= Time.deltaTime;

        if (knockbackTimer <= 0f)
        {
            rb.linearVelocity = Vector2.zero;


            spriteRenderer.color = Color.white;


            if (moveInput == Vector2.zero)
                currentState = PlayerState.IDLE;
            else if (isSprinting)
                currentState = PlayerState.RUNNING;
            else
                currentState = PlayerState.WALK;

            animator.SetBool("isWalking", moveInput != Vector2.zero);
        }
    }




    void FixedUpdate()
    {
        if (currentState == PlayerState.DASH) return;


        if (currentState == PlayerState.KNOCKBACK)
        {
            if (applyKnockback)
            {
                rb.linearVelocity = Vector2.zero;

                rb.AddForce(
                    knockbackDirection * knockbackForce,
                    ForceMode2D.Impulse
                );

                applyKnockback = false;
            }

            return;

        }


            switch (currentState)
        {
            case PlayerState.IDLE:
                rb.linearVelocity = Vector2.zero;
                break;

            case PlayerState.WALK:
                rb.linearVelocity = moveInput * walkSpeed;
                Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
                Attackarea.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
                break;

            case PlayerState.RUNNING:
                rb.linearVelocity = moveInput * runSpeed;
                break;

        }
        
    }


    private void UpdateAttackDirection()
    {
        if (moveInput == Vector2.zero)
            return;

        lastMoveDirection = moveInput;

        Vector3 vector3 =
            Vector3.left * lastMoveDirection.x +
            Vector3.down * lastMoveDirection.y;

        Attackarea.rotation =
            Quaternion.LookRotation(
                Vector3.forward,
                vector3
            );
    }



    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);

        }
        UpdateAttackDirection();
        moveInput = context.ReadValue<Vector2>();
       
     

    }

   public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isSprinting = true;
        }

        if (context.canceled)
        {
            isSprinting = false;
        }
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (currentState == PlayerState.KNOCKBACK) return;

        if (dashCooldownTimer > 0f) return;

        if (moveInput != Vector2.zero)
        {
            dashDirection = moveInput.normalized;
        }
        else
        {
            dashDirection= new Vector2(
                animator.GetFloat("LastInputX"),
                animator.GetFloat("LastInputY")
            ).normalized;
        }


        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.right;
        }

        currentState = PlayerState.DASH;
        Physics2D.IgnoreLayerCollision(3, 6, true);
        dashTimer = dashDuration;


        dashCooldownTimer = dashCooldown;

    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (currentState == PlayerState.KNOCKBACK) return;

        currentState = PlayerState.ATTACK;


    }


}
