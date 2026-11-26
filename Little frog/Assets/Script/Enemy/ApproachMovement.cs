using UnityEngine;

public class ApproachMovement : EnemyMovementBehaviour
{
    public override Vector2 GetMovementDirection(Transform target, float attackRange)
    {
        if (target == null)
            return Vector2.zero;

        Vector2 direction =
            (Vector2)target.position - (Vector2)transform.position;

      

        return direction.normalized;
    }

    public override bool IsInAttackPosition(Transform target, float attackRange)
    {
        if (target == null)
            return false;

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        return distance <= attackRange;
    }
}