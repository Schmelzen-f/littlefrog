using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D bullet;


    private EnemyAttack enemyAttack;

    private void Awake()
    {
        bullet = GetComponent<Rigidbody2D>();
    }





    public void ShootAt(Transform target, EnemyAttack attack)
    {
        enemyAttack = attack;

        Vector2 moveDir =
            ((Vector2)target.position - bullet.position).normalized;

        bullet.linearVelocity = moveDir * speed;
    }






    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                enemyAttack.TryAttack(player);
            }

            Destroy(gameObject);
        }
    }



}