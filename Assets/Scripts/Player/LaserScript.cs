using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public float speed = 60f;
    public int damage = 1;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessHit(other.gameObject);
        Debug.Log("LaserCollided");
        if (other.CompareTag("Player")) return;
        else
            Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        ProcessHit(collision.gameObject);
        Debug.Log("LaserCollided");
        if (collision.gameObject.CompareTag("Player")) return;
        else
            Destroy(gameObject);
    }

    void ProcessHit(GameObject other)
    {
        if (other.CompareTag("Player")) return;

        var Asteroid = other.GetComponent<AsteroidScript>();
        if (Asteroid != null)
        {
            Asteroid.TakeDamage(damage);
            return;
        }
        Destroy(gameObject);
    }
}
