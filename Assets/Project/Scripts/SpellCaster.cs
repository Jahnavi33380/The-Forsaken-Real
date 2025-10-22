using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public PlayerHealth playerHealth;
    public int healAmount = 5;
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode healKey = KeyCode.Mouse1;

    void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            CastFireball();
        }

        if (Input.GetKeyDown(healKey))
        {
            CastHeal();
        }
    }

    void CastFireball()
    {
        if (fireballPrefab && firePoint)
        {
            Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void CastHeal()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
    }
}
