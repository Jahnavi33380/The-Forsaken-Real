using UnityEngine;

public class FreezeSpell : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    void Start()
    {
        Debug.Log("Freeze spell STARTED at: " + transform.position);
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Debug.Log("FreezeSpell moving...");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("FreezeSpell hit: " + other.name);
        if (other.CompareTag("Enemy"))
        {
            FreezeEffect fe = other.GetComponent<FreezeEffect>();
            if (fe != null)
            {
                fe.Freeze();
                Debug.Log("FreezeSpell hit enemy: " + other.name);
            }
            else
            {
                Debug.LogWarning("No FreezeEffect found on: " + other.name);
            }
        }

        Destroy(gameObject);
    }
}
