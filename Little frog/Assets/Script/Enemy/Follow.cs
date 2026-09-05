/*using System;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour
{


    private Transform player;
    public float detectDistance = 5f;
    public float speed = 3f;
    private Vector3 start;

    public float timer = 0f;
    public float returnDelay = 2f;

    public Transform[] points;
    private int currentIndex = 0;
    private float waitCounter = 0f;
    public float waitTime = 2f;

    private bool hasLineOfSight = false;
    public LOSGenerator losGenerator;

    Vector2 targetPoint;
    List<Vector2> currentPoints;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



        start = transform.position;


        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            Debug.Log("đã tìm thấy game object có tag player và guarding");
            player = playerObject.transform;


        }
        else
        {
            Debug.LogError("không tìm thấy player và guarding");

        }






    }
    // Update is called once per frame
    void Update()
    {
        if (player == null) return;


        float Distance = Vector3.Distance(transform.position, player.position);



        if(hasLineOfSight && Distance < detectDistance)
        {
            timer = 0f;
            FollowPlayer();
        }
        else
        {
            timer += Time.deltaTime;

            if (returnDelay < timer)
            {
                guarding();
                //returnHome();
            }
        }

    }

    void FollowPlayer()
    {


        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);


    }

    void returnHome()
    {
        transform.position = Vector3.MoveTowards(transform.position, start, speed * Time.deltaTime);
    }

    /////////////////// đang nghiên cứu
    Vector2 ChooseBestPoint(List<Vector2> points, Vector2 enemyPos, Vector2 playerVelocity)
    {
        Vector2 bestPoint = points[0];
        float bestScore = float.MaxValue;

        foreach (Vector2 p in points)
        {
            float dist = Vector2.Distance(enemyPos, p);

            Vector2 dirToPoint = (p - enemyPos).normalized;
            float dot = Vector2.Dot(dirToPoint, playerVelocity.normalized);

            float score = dist - dot * 2f;

            if (score < bestScore)
            {
                bestScore = score;
                bestPoint = p;
            }
        }

        return bestPoint;
    }



    ///////////////////////////////////////

    void guarding()
    {

        float detectDistance = 0.1f;

        Transform target = points[currentIndex];
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectDistance)
        {
            waitCounter = 0f; // réset nè


            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );



        }
        else
        {
            waitCounter += Time.deltaTime; // đứng chờ

            if (waitTime < waitCounter)
            {
                currentIndex++;

                if (currentIndex >= points.Length)
                    currentIndex = 0;
            }

        }


    }



    private void FixedUpdate()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Player");
            if (hasLineOfSight)
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            }
        }
    }
}*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//using NUnit.Framework;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using UnityEngine.SocialPlatforms.Impl;

public class Follow : MonoBehaviour
{
    private Transform player;

    [Header("Detect")]
    public float detectDistance = 5f;

    [Header("Speed")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float searchSpeed = 3f;

    [Header("Search")]
    public float searchTime = 3f;

    private float searchTimer;

    [Header("Patrol")]
    public Transform[] points;
    private int currentIndex = 0;
    private float waitCounter = 0f;
    public float waitTime = 2f;

    private bool hasLineOfSight = false;

    public LOSGenerator losGenerator;
    public NodeManager nodeManager;
    private Node targetNode;
    public LayerMask obstacleLayer;

    Vector2 lastPlayerPos;
    Vector2 playerVelocity;


    //List<Vector2> currentPoints;






    private Rigidbody2D enemyRb;
    Rigidbody2D playerRb;

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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
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

        playerVelocity = ((Vector2)player.position - lastPlayerPos) / Time.deltaTime;
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

            /*transform.position = Vector2.MoveTowards(
                 transform.position,
                 target.position,
                 patrolSpeed * Time.deltaTime
             );*/
            Vector2 direction = (target.position - transform.position).normalized;

            enemyRb.linearVelocity = direction * chaseSpeed;    
        }
        else
        {
            enemyRb.linearVelocity = Vector2.zero;
            waitCounter += Time.deltaTime;

            if (waitCounter > waitTime)
            {
                currentIndex++;
                if (currentIndex >= points.Length)
                    currentIndex = 0;
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
        Debug.Log("CHASE");
        
    

        float dist = Vector2.Distance(transform.position, player.position);


        Debug.Log(dist);
        if (hasLineOfSight && dist < detectDistance)
        {
            /*transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );*/
            Vector2 direction = (player.position - transform.position).normalized;

            enemyRb.linearVelocity = direction * chaseSpeed;
        }
        else
        {
            // chuyển sang SEARCH
            currentState = State.SEARCH;
            searchTimer = searchTime;

            //currentPoints = null;------------------------------
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



        // nếu chưa có điểm → generate + chọn
        // if (currentPoints == null || currentPoints.Count == 0)




        //currentPoints = losGenerator.GeneratePoints();--------------------








        // di chuyển đến targetPoint
        /*if (currentPoints != null && currentPoints.Count > 0)
        {
            Debug.LogError("step 7");
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPoint,
                searchSpeed * Time.deltaTime
            );
            Debug.LogError("step 8");


        }*/



        
        // ================= CHỌN ĐIỂM =================
        if (locktarget == false)
        {
            
            ///////////////////////////////////////////////// đoán hướng di chuyển /////////////////////////
            /* Vector2 ChooseBestPoint(List<Vector2> points, Vector2 enemyPos, Vector2 playerVelocity)
             {
                 Vector2 bestPoint = points[0];
                 float bestScore = float.MaxValue;

                 foreach (Vector2 p in points)
                 {
                     float dist = Vector2.Distance(enemyPos, p);

                     Vector2 dirToPoint = (p - enemyPos).normalized;
                     float dot = Vector2.Dot(dirToPoint, playerVelocity.normalized);

                     float score = dist - dot * 2f;

                     if (score < bestScore)
                     {
                         bestScore = score;
                         bestPoint = p;
                     }
                 }

                 return bestPoint;
             }



             if (currentPoints.Count > 0)
             {
                 Debug.LogError("step 4");
                 Vector2 playerVelocity1 = playerRb != null ? playerVelocity : Vector2.zero;


                 Debug.LogError("step 5");

                 targetPoint = ChooseBestPoint(
                     currentPoints,
                     transform.position,
                     playerVelocity1
                 );



             }*/



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
                         dir.magnitude
                       
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


            /*transform.position = Vector2.MoveTowards(
                transform.position,
                targetNode.Position,
                searchSpeed * Time.deltaTime
            );*/
            //transform.position = targetNode.Position;

            Vector2 direction = (targetNode.Position - (Vector2)transform.position).normalized;

            enemyRb.linearVelocity = direction * chaseSpeed;

            // 3. Nếu tới nơi → reset để chọn node mới
            float dist11 = Vector2.Distance(transform.position, targetNode.Position);

            if (dist11 < 0.2f)
            {
                targetNode = null;
                locktarget = false;

                enemyRb.linearVelocity = Vector2.zero;
            }
        }



        // ================ LOS =================


    }

    void FixedUpdate()
    {
        Vector2 dir = (Vector2)player.position - (Vector2)transform.position;

        RaycastHit2D ray = Physics2D.Raycast(
            transform.position,
            dir.normalized,
            dir.magnitude
        );


        if (ray.collider != null)
        {
            Debug.Log("Hit: " + ray.collider.name);
        }
        else
        {
            Debug.Log("Hit: NULL");
        }


        // luôn cập nhật mỗi frame
        if (ray.collider != null && ray.collider.CompareTag("Player"))
        {
            hasLineOfSight = true;
        }
        else
        {
            hasLineOfSight = false;
        }

        Debug.DrawRay(
            transform.position,
            dir,
            hasLineOfSight ? Color.green : Color.red
        );
    }

}
/*void FixedUpdate()
{
    Vector2 origin = transform.position;
    Vector2 dir = (Vector2)player.position - origin;

    //Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

    //Vector2 origin = (Vector2)transform.position + dir * 0.5f; // chỉnh số này



    float distance = dir.magnitude;

    // chỉnh theo collider của enemy
    float radius = 0.5f;

    RaycastHit2D hit = Physics2D.CircleCast(
        origin,
        radius,
        dir.normalized,
        distance,
        obstacleLayer

    );




    if (hit.collider != null)
    {
        if (hit.collider.CompareTag("Player"))
        {
            hasLineOfSight = true;
        }
        else
        {
            hasLineOfSight = false;
        }
    }

    Debug.DrawRay(
        origin,
        dir,
        hasLineOfSight ? Color.green : Color.red
    );
}


}*/
