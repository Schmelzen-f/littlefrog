

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;



public class Follow : MonoBehaviour
{
    private Transform player;

    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;

    //Nhức đầu
    private EnemyAttackBehaviour attackBehaviour;
    private EnemyMovementBehaviour movementBehaviour;


    [Header("Detect")]
    public float detectDistance = 5f;
    [SerializeField] private LayerMask lineOfSightMask;

    [Header("Speed")]
    public float patrolSpeed = 1f;
    public float chaseSpeed = 1.5f;
    public float searchSpeed = 1.5f;

    [Header("Search")]
    public float searchTime = 3f;

    private float searchTimer;

    [Header("Patrol")]
    public Transform[] points;
    private int currentIndex = 0;
    private float waitCounter = 0f;
    public float waitTime = 2f;

    private bool hasLineOfSight = false;
    

    
    public NodeManager nodeManager;
    private Node targetNode;
    public LayerMask obstacleLayer;
    
    Vector2 lastPlayerPos;
    









    private Rigidbody2D enemyRb;


    enum State
    {
        PATROL,
        CHASE,
        SEARCH
       
    }

    State currentState = State.PATROL;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();

        attackBehaviour = GetComponent<EnemyAttackBehaviour>();

        movementBehaviour = GetComponent<EnemyMovementBehaviour>();



        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            //playerRb = player.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("không tìm thấy player");
        }
    }

    void Update()
    {
        if (player == null) return;



        switch (currentState)
        {
            case State.PATROL:
                Patrol();
                break;

            case State.CHASE:
                Chase();
                break;

            case State.SEARCH:
                Search();
                break;
            
        }

        lastPlayerPos = player.position;


    }

    // ================= PATROL =================
    void Patrol()
    {
        Debug.Log("Patrol");
       
        Transform target = points[currentIndex];
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > 0.2f)
        {
            waitCounter = 0f;
            Vector2 direction = (target.position - transform.position).normalized;

            enemyRb.linearVelocity = direction * patrolSpeed;


            lastMoveDirection = direction;
            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);
            animator.SetBool("isWalking", true);
        }
        else
        {
            enemyRb.linearVelocity = Vector2.zero;


            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
            animator.SetBool("isWalking", false);



 

            if (points.Length > 1)
            {
                waitCounter += Time.deltaTime;

                if (waitCounter >= waitTime)
                {
                    waitCounter = 0f;

                    currentIndex++;

                    if (currentIndex >= points.Length)
                        currentIndex = 0;
                }
            }
        }
       
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        Debug.Log(hasLineOfSight);

        if (hasLineOfSight && distToPlayer < detectDistance)
        {
            currentState = State.CHASE;
        }
    }




    // ================= CHASE =================

    void Chase()
    {
        // Attack Behaviour đang xử lý đòn đánh
        if (attackBehaviour != null &&
            attackBehaviour.IsAttacking)
            return;

        float dist = Vector2.Distance(
            transform.position,
            player.position
        );

        if (hasLineOfSight && dist < detectDistance)
        {
            if (movementBehaviour == null)
            {
                enemyRb.linearVelocity = Vector2.zero;
                animator.SetBool("isWalking", false);
                return;
            }

            float attackRange = attackBehaviour != null
                ? attackBehaviour.AttackRange
                : 0f;

            // Kiểm tra đã ở vị trí thích hợp để tấn công chưa
            if (movementBehaviour.IsInAttackPosition(player, attackRange) && attackBehaviour != null && attackBehaviour.CanStartAttack)
            {
                enemyRb.linearVelocity = Vector2.zero;

                animator.SetFloat("LastInputX", lastMoveDirection.x);
                animator.SetFloat("LastInputY", lastMoveDirection.y);
                animator.SetBool("isWalking", false);

                attackBehaviour.StartAttack(player);
               
                return;
            }

            // Lấy hướng di chuyển từ MovementBehaviour
            Vector2 direction =
                movementBehaviour.GetMovementDirection(player, attackRange);
         
            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();

                enemyRb.linearVelocity = direction * chaseSpeed;

                // Cập nhật hướng nhìn
                Vector2 facingDirection =
                    movementBehaviour.GetFacingDirection(player);

                if (facingDirection.sqrMagnitude > 0.001f)
                {
                    facingDirection.Normalize();

                    lastMoveDirection = facingDirection;

                    animator.SetFloat("InputX", facingDirection.x);
                    animator.SetFloat("InputY", facingDirection.y);
                }

                animator.SetBool("isWalking", true);
            }
            else
            {
                enemyRb.linearVelocity = Vector2.zero;

                animator.SetFloat("LastInputX", lastMoveDirection.x);
                animator.SetFloat("LastInputY", lastMoveDirection.y);

                animator.SetBool("isWalking", false);
            }
        }
        else
        {
            enemyRb.linearVelocity = Vector2.zero;

            animator.SetBool("isWalking", false);

            currentState = State.SEARCH;
            searchTimer = searchTime;
        }
    }
















    // ================= SEARCH =================
    bool locktarget = false;
    void Search()
    {


        


        searchTimer -= Time.deltaTime;
        float dist1 = Vector2.Distance(transform.position, player.position);
        // thấy lại player → quay lại chase

        if (hasLineOfSight && dist1 < detectDistance)
        {
            locktarget = false;
            currentState = State.CHASE;
            return;
        }

        // hết thời gian → quay về patrol
        if (searchTimer <= 0)
        {
            currentState = State.PATROL;
            return;
        }



        // ================= CHỌN ĐIỂM =================
        if (locktarget == false)
        {
            
           



            List<Node> nodes = nodeManager.GetVisibleNodes();

            if (nodes.Count == 0)
            {

                return;
            }

            float bestScore = Mathf.Infinity;

            foreach (Node node in nodes)
            {
                float distEnemy = Vector2.Distance(transform.position, node.Position);
                float distPlayer = Vector2.Distance(node.Position, player.position);

                if (distEnemy <= detectDistance)
                {
                   
                    Vector2 dir = node.Position - (Vector2)transform.position;

                     RaycastHit2D hit = Physics2D.Raycast(
                         transform.position,
                         dir.normalized,
                         dir.magnitude,
                         obstacleLayer
                     );
            
                    if (hit.collider == null)
                    {
                        float score = distEnemy + distPlayer;

                        if (score < bestScore)
                        {
                            bestScore = score;
                            targetNode = node;
                        }
                    }
                }


            }

            Debug.Log(targetNode);
            locktarget = true;
        }
        // 2. Di chuyển tới node
        if (targetNode != null)
        {

            Vector2 direction = (targetNode.Position - (Vector2)transform.position).normalized;

            enemyRb.linearVelocity = direction * chaseSpeed;


            lastMoveDirection = direction;
            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);
            animator.SetBool("isWalking", true);


            // 3. Nếu tới nơi → reset để chọn node mới
            float dist11 = Vector2.Distance(transform.position, targetNode.Position);

            if (dist11 < 0.5f)
            {
                targetNode = null;
                locktarget = false;

                enemyRb.linearVelocity = Vector2.zero;


                animator.SetFloat("LastInputX", lastMoveDirection.x);
                animator.SetFloat("LastInputY", lastMoveDirection.y);
                animator.SetBool("isWalking", false);
            }
        }
    }

    


    void FixedUpdate()
    {
       
            if (player == null) return;


        



        Vector2 origin = transform.position;
            Vector2 dir = (Vector2)player.position - origin;

            RaycastHit2D ray = Physics2D.Raycast(
                origin,
                dir.normalized,
                dir.magnitude,
                lineOfSightMask
            );

            if (ray.collider != null)
            {
                Debug.Log("Hit: " + ray.collider.name);
            }
            else
            {
                Debug.Log("Hit: NULL");
            }

            hasLineOfSight =
                ray.collider != null && ray.collider.CompareTag("Player");

            Debug.DrawRay(
                origin,
                dir,
                hasLineOfSight ? Color.green : Color.red
            );
        
    }

}

