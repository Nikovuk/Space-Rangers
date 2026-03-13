using System.Collections.Generic;
using UnityEngine;

public class NpcPirateBaseSpawner : MonoBehaviour
{
    [SerializeField] private NpcShipPatrol pirateShipPrefab;
    [SerializeField] private Transform pirateBase;
    [SerializeField] private int maxPirates = 4;
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private float spawnOffsetRadius = 8f;

    private readonly List<NpcShipPatrol> spawnedPirates = new List<NpcShipPatrol>();
    private float spawnTimer;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
        {
            return;
        }

        spawnTimer = Mathf.Max(1f, spawnInterval);
        CleanupDestroyedPirates();

        if (pirateShipPrefab == null || pirateBase == null || spawnedPirates.Count >= Mathf.Max(0, maxPirates))
        {
            return;
        }

        Vector3 spawnPoint = pirateBase.position + Random.insideUnitSphere * spawnOffsetRadius;
        NpcShipPatrol patrol = Instantiate(pirateShipPrefab, spawnPoint, Quaternion.identity, transform);

        if (patrol.GetComponent<NpcTraderShip>() != null)
        {
            Destroy(patrol.GetComponent<NpcTraderShip>());
        }

        if (patrol.GetComponent<NpcPirateShip>() == null)
        {
            patrol.gameObject.AddComponent<NpcPirateShip>();
        }

        NpcPirateRoamArea roam = patrol.GetComponent<NpcPirateRoamArea>();
        if (roam == null)
        {
            roam = patrol.gameObject.AddComponent<NpcPirateRoamArea>();
        }

        roam.SetPirateBase(pirateBase);
        spawnedPirates.Add(patrol);
    }

    private void CleanupDestroyedPirates()
    {
        for (int i = spawnedPirates.Count - 1; i >= 0; i--)
        {
            if (spawnedPirates[i] == null || spawnedPirates[i].IsDestroyed)
            {
                spawnedPirates.RemoveAt(i);
            }
        }
    }
}
