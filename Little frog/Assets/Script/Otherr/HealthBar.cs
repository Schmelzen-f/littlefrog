using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
  
    public Image fill;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;


    }

    public void SetHealth(int health)
    {
        slider.value = health;

        
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
   
}