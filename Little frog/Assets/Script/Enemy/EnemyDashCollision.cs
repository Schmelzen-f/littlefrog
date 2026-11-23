
using UnityEngine;

public class EnemyDashCollision : MonoBehaviour
{
    [SerializeField] private SlimeDashAttack slimeDashAttack;
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private LayerMask obstacleLayer;

    private void Awake()
    {
        if (slimeDashAttack == null)
            slimeDashAttack = GetComponent<SlimeDashAttack>();

        if (enemyAttack == null)
            enemyAttack = GetComponent<EnemyAttack>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    private void HandleContact(Collider2D other)
    {
        if (slimeDashAttack == null ||
            !slimeDashAttack.IsDashing)
            return;

        // Chạm Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Buoc 3");
            enemyAttack.TryAttack();
            Debug.Log("Buoc 5");
            slimeDashAttack.EndDash();
            return;
        }

        // Chạm vật cản
        bool isObstacle =
            (obstacleLayer.value &
            (1 << other.gameObject.layer)) != 0;

        if (isObstacle)
        {
            
            slimeDashAttack.EndDash();
        }
    }
}
