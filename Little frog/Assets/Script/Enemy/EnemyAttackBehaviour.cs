
using UnityEngine;

public abstract class EnemyAttackBehaviour : MonoBehaviour
{
    public abstract float AttackRange { get; }

    public abstract bool IsAttacking { get; }

    public abstract bool CanStartAttack { get; }

    public abstract void StartAttack(Transform target);
}
