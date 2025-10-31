using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fireball hit: " + other.name);
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}
