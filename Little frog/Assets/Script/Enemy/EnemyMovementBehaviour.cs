using UnityEngine;

public abstract class EnemyMovementBehaviour : MonoBehaviour
{
    // Quyết định hướng di chuyển
    public abstract Vector2 GetMovementDirection(
        Transform target,
        float attackRange
    );

    // Kiểm tra đã ở vị trí thích hợp để tấn công chưa
    public abstract bool IsInAttackPosition(
        Transform target,
        float attackRange
    );

    // Xác định hướng nhìn của Enemy
    public virtual Vector2 GetFacingDirection(Transform target)
    {
        if (target == null)
            return Vector2.zero;

        Vector2 direction =
            (Vector2)target.position - (Vector2)transform.position;

        return direction.normalized;
    }
}