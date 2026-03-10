using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public int maxHP = 3;
    int currentHP;
    public GameObject orePrefab;    
    public int oreDropCount = 1;   
    public float spawnRadius = 0.5f;
    public float minSpawnDistance = 1f;
    public float maxSpawnDistance = 2f;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            DropOre();
            currentHP = maxHP;
        }
        Debug.Log("Damage to asteroid");
    } 
     
    void DropOre() 
    { 
        Vector3 dir = Random.onUnitSphere; 
        float dist = Random.Range(minSpawnDistance, maxSpawnDistance); 
        Vector3 spawnPos = transform.position + dir * dist; 
        Instantiate(orePrefab, spawnPos, Quaternion.identity); 
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    var laser = other.GetComponent<LaserScript>();
    //    if (laser != null)
    //    {
    //        TakeDamage(laser.damage);
    //        Destroy(other.gameObject);
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var laser = collision.gameObject.GetComponent<LaserScript>();
    //    if (laser != null)
    //    {
    //        TakeDamage(laser.damage);
    //        Destroy(collision.gameObject);
    //    }
    //}
}
