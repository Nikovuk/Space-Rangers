using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public float speed = 60f;
    public int damage = 1;
    public PlayerResources owner;
    public float pirateRetaliationDamage = 2f;
    public float traderRetaliationDamage = 1f;

    public void Initialize(PlayerResources laserOwner)
    {
        owner = laserOwner;
    }

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

        var asteroid = other.GetComponent<AsteroidScript>();
        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
            return;
        }

        NpcPirateShip pirate = other.GetComponentInParent<NpcPirateShip>();
        if (pirate != null)
        {
            pirate.ReceiveDamage(damage);
            if (owner != null)
            {
                owner.ReceiveDamage(pirateRetaliationDamage);
            }
            return;
        }

        NpcTraderShip trader = other.GetComponentInParent<NpcTraderShip>();
        if (trader != null)
        {
            trader.ReceiveDamage(damage);
            if (owner != null)
            {
                owner.ReceiveDamage(traderRetaliationDamage);
            }
            return;
        }

        Destroy(gameObject);
    }
}
