using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public GameObject fbPrefab;
    public GameObject freezeSpellPrefab;
    public Transform firePointObject;
    public GameObject healEffect;
    public PlayerHealth playerHealth;
    public int healAmount = 5;
    public KeyCode fireKey = KeyCode.Alpha1;
    public KeyCode healKey = KeyCode.Alpha2;
    public KeyCode freezeKey = KeyCode.F;

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Fireball key pressed!");
            CastFireball();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Heal key pressed!");
            CastHeal();
        }
        if (Input.GetKeyDown(freezeKey))
        {
            Debug.Log("Freeze key pressed!");
            CastFreeze();
        }
    }

    void CastFireball()
    {
        Debug.Log("CastFireball() called");
        if (fbPrefab && firePointObject)
        {
            Instantiate(fbPrefab, firePointObject.position, firePointObject.rotation);
        }
        else
        {
            Debug.LogWarning("Fireball prefab or firePointObject not assigned!");
        }
    }




    void CastHeal()
    {
        Debug.Log("CastHeal() called");
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            if (healEffect != null)
            {
                GameObject healFX = Instantiate(healEffect, playerHealth.transform.position, Quaternion.identity);
                Destroy(healFX, 3f);
            }
            else
            {
                Debug.LogWarning("Heal effect prefab is missing!");
            }
        }
        else
        {
            Debug.LogWarning("PlayerHealth is not assigned!");
        }
    }


    void CastFreeze()
    {
        if (freezeSpellPrefab && firePointObject)
            Instantiate(freezeSpellPrefab, firePointObject.position, firePointObject.rotation);
            Debug.Log("Freeze spell instantiated!");
    }

}
