using UnityEngine;

public class NpcPirateWaveTriggerSpawner : MonoBehaviour
{
    [SerializeField] private NpcShipPatrol pirateShipPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int piratesPerWave = 3;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private PirateStockDropPopup stockDropPopup;

    private bool spawned;

    private void OnTriggerEnter(Collider other)
    {
        if (spawned || pirateShipPrefab == null || !other.CompareTag("Player"))
        {
            return;
        }

        spawned = true;
        Transform target = other.transform;
        if (stockDropPopup == null)
        {
            stockDropPopup = FindObjectOfType<PirateStockDropPopup>();
        }

        Vector3 center = spawnPoint == null ? transform.position : spawnPoint.position;

        for (int i = 0; i < Mathf.Max(0, piratesPerWave); i++)
        {
            Vector3 position = center + Random.insideUnitSphere * spawnRadius;
            NpcShipPatrol patrol = Instantiate(pirateShipPrefab, position, Quaternion.identity);

            patrol.enabled = false;

            NpcPirateShip pirate = patrol.GetComponent<NpcPirateShip>();
            if (pirate == null)
            {
                pirate = patrol.gameObject.AddComponent<NpcPirateShip>();
            }

            TriggeredPirateAttacker attacker = patrol.GetComponent<TriggeredPirateAttacker>();
            if (attacker == null)
            {
                attacker = patrol.gameObject.AddComponent<TriggeredPirateAttacker>();
            }

            attacker.SetTarget(target);

            TriggeredPirateDeathNotifier notifier = patrol.GetComponent<TriggeredPirateDeathNotifier>();
            if (notifier == null)
            {
                notifier = patrol.gameObject.AddComponent<TriggeredPirateDeathNotifier>();
            }

            notifier.SetPopup(stockDropPopup);
        }
    }
}
