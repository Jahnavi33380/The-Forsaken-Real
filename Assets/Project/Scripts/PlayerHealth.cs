using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int hMax = 20;               
    public int startingHealth = 5;     
    private int currentHealth;          
    public HUDController hud;

    void Start()
    {
        currentHealth = startingHealth;
        hud.SetHealth(hMax, currentHealth);
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("PlayerHealth.TakeDamage hit for: " + amount);
        currentHealth -= amount;
        hud.SetHealth(hMax, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, hMax);
        hud.SetHealth(hMax, currentHealth);
    }

    void Die()
    {
        GetComponent<ControllerPerson>().enabled = false;
        hud.ShowDeathScreen();
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
