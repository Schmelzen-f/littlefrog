using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
   
    public int Enemydamage = 10;
    public float attackRange = 1.2f;

    public bool TryAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange
        );

        foreach (Collider2D hit in hits)
        {
            PlayerMovement player =
                hit.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                Debug.Log("Buoc 4");
                player.UDieFuckingIdiot(Enemydamage, transform.position);

                Debug.Log("Enemy da tan cong");

                return true;
            }
        }

        return false;
    }







    public bool TryAttack(PlayerMovement player)
    {
        if (player == null) return false;

        player.UDieFuckingIdiot(Enemydamage, transform.position);

        return true;
    }




}


