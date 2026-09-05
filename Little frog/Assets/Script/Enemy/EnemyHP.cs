using UnityEngine;

public class EnemyHP : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;    
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;


    }
}
