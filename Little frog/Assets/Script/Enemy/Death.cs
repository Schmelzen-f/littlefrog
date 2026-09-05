using UnityEngine;

public class Death : MonoBehaviour
{
    EnemyHP enemy;



    private void Start()
    {
        enemy = GetComponent<EnemyHP>();
    }



    void Update()
    {
        if (enemy.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

  
}
