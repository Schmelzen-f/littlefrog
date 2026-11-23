using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 20;

    private CircleCollider2D attackCollider;

    private void Awake()
    {
        attackCollider = GetComponent<CircleCollider2D>();
        
    }

    public void StartAttack()
    {
        attackCollider.enabled = true;
    }

    public void DiasbleAttack()
    {
        attackCollider.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHP>()?.TakeDamage(damage);
            Debug.Log("Player da tan cong");
        }
        attackCollider.enabled = false;
    }
}
