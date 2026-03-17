using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public float speed = 60f;
    public int damage = 1;
    public PlayerResources owner;
    public float pirateRetaliationDamage = 2f;
    public float traderRetaliationDamage = 1f;

    private bool npcShot;
    private float npcShipDamage;
    private float npcPlayerDamage;

    public void Initialize(PlayerResources laserOwner)
    {
        owner = laserOwner;
        npcShot = false;
    }

    public void InitializeNpcShot(float shipDamage, float playerDamage)
    {
        owner = null;
        npcShot = true;
        npcShipDamage = Mathf.Max(0f, shipDamage);
        npcPlayerDamage = Mathf.Max(0f, playerDamage);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessHit(other.gameObject);
        Debug.Log("LaserCollided");

        if (other.CompareTag("Player") && !npcShot)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcessHit(collision.gameObject);
        Debug.Log("LaserCollided");

        if (collision.gameObject.CompareTag("Player") && !npcShot)
        {
            return;
        }

        Destroy(gameObject);
    }

    void ProcessHit(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (!npcShot)
            {
                return;
            }

            PlayerResources player = other.GetComponent<PlayerResources>();
            if (player != null)
            {
                player.ReceiveDamage(npcPlayerDamage);
            }

            return;
        }

        var asteroid = other.GetComponent<AsteroidScript>();
        if (asteroid != null)
        {
            asteroid.TakeDamage(npcShot ? Mathf.Max(1, Mathf.RoundToInt(npcShipDamage)) : damage);
            return;
        }

        float hitDamage = npcShot ? npcShipDamage : damage;

        NpcPirateShip pirate = other.GetComponentInParent<NpcPirateShip>();
        if (pirate != null)
        {
            pirate.ReceiveDamage(hitDamage);
            if (!npcShot && owner != null)
            {
                owner.ReceiveDamage(pirateRetaliationDamage);
            }
            return;
        }

        NpcTraderShip trader = other.GetComponentInParent<NpcTraderShip>();
        if (trader != null)
        {
            trader.ReceiveDamage(hitDamage);
            if (!npcShot && owner != null)
            {
                owner.ReceiveDamage(traderRetaliationDamage);
            }
            return;
        }
    }
}
