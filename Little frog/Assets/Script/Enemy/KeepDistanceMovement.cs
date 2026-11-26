using System.Collections;
using UnityEngine;

public class KeepDistanceMovement : EnemyMovementBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float retreatDistance = 1.5f;

    [Header("Retreat Delay")]
    [SerializeField, Min(0f)] private float retreatDelay = 2f;

    private bool isRetreating = false;
    private Coroutine retreatCoroutine;

    public override Vector2 GetMovementDirection(
        Transform target,
        float attackRange
    )
    {
        if (target == null)
        {
            ResetRetreat();
            return Vector2.zero;
        }

        Vector2 toTarget =
            (Vector2)target.position - (Vector2)transform.position;

        float distance = toTarget.magnitude;

        // Player tiến quá gần
        if (distance <= retreatDistance)
        {
            // Bắt đầu Coroutine nếu chưa chạy
            if (!isRetreating && retreatCoroutine == null)
            {
                retreatCoroutine = StartCoroutine(RetreatAfterDelay());
            }

            // Đợi hết thời gian trước khi chạy trốn
            if (!isRetreating)
                return Vector2.zero;

            // Quay lưng chạy khỏi Player
            return -toTarget.normalized;
        }

        // Player đã ra khỏi vùng Retreat
        ResetRetreat();

        // Player ở xa: Chase
        if (distance > attackRange)
            return toTarget.normalized;

        // Đúng khoảng cách: đứng yên
        return Vector2.zero;
    }

    public override bool IsInAttackPosition(
        Transform target,
        float attackRange
    )
    {
        if (target == null)
            return false;

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        return distance > retreatDistance &&
               distance <= attackRange;
    }

    public override Vector2 GetFacingDirection(Transform target)
    {
        if (target == null)
            return Vector2.zero;

        Vector2 toTarget =
            (Vector2)target.position - (Vector2)transform.position;

        if (isRetreating)
            return -toTarget.normalized;

        return toTarget.normalized;
    }

    // ================= RETREAT COROUTINE =================

    private IEnumerator RetreatAfterDelay()
    {
        yield return new WaitForSeconds(retreatDelay);

        isRetreating = true;
        retreatCoroutine = null;
    }

    private void ResetRetreat()
    {
        if (retreatCoroutine != null)
        {
            StopCoroutine(retreatCoroutine);
            retreatCoroutine = null;
        }

        isRetreating = false;
    }
}