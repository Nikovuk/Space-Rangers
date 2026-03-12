using System.Collections.Generic;
using UnityEngine;

public class NpcShipTrafficSystem : MonoBehaviour
{
    [Header("Traffic Setup")]
    [SerializeField] private NpcShipPatrol npcShipPrefab;
    [SerializeField] private List<NpcShipRoute> routes = new List<NpcShipRoute>();
    [SerializeField] private Transform routesRoot;
    [SerializeField] private bool includeInactiveRoutes = true;

    [Header("Spawn Settings")]
    [SerializeField] private int shipsPerRoute = 2;
    [SerializeField] private float spawnOffsetRadius = 6f;

    private readonly List<NpcShipPatrol> spawnedShips = new List<NpcShipPatrol>();
    private readonly List<NpcShipRoute> runtimeRoutes = new List<NpcShipRoute>();

    private void Start()
    {
        SpawnTraffic();
    }

    private void SpawnTraffic()
    {
        if (npcShipPrefab == null || shipsPerRoute <= 0)
        {
            return;
        }

        BuildRuntimeRoutes();
        if (runtimeRoutes.Count == 0)
        {
            return;
        }

        for (int routeIndex = 0; routeIndex < runtimeRoutes.Count; routeIndex++)
        {
            NpcShipRoute route = runtimeRoutes[routeIndex];
            for (int shipIndex = 0; shipIndex < shipsPerRoute; shipIndex++)
            {
                bool startFromA = shipIndex % 2 == 0;
                Vector3 spawnPoint = route.GetPointPosition(startFromA);
                Vector3 randomOffset = Random.insideUnitSphere * spawnOffsetRadius;

                NpcShipPatrol ship = Instantiate(npcShipPrefab, spawnPoint + randomOffset, Quaternion.identity, transform);
                ship.SetRoute(route, startFromA);
                spawnedShips.Add(ship);
            }
        }
    }

    private void BuildRuntimeRoutes()
    {
        runtimeRoutes.Clear();

        for (int i = 0; i < routes.Count; i++)
        {
            TryAddRoute(runtimeRoutes, routes[i]);
        }

        if (routesRoot != null)
        {
            NpcShipRoute[] foundRoutes = routesRoot.GetComponentsInChildren<NpcShipRoute>(includeInactiveRoutes);
            for (int i = 0; i < foundRoutes.Length; i++)
            {
                TryAddRoute(runtimeRoutes, foundRoutes[i]);
            }
        }
    }

    private static void TryAddRoute(List<NpcShipRoute> target, NpcShipRoute route)
    {
        if (route == null || !route.IsValid || target.Contains(route))
        {
            return;
        }

        target.Add(route);
    }

    private void ClearTraffic()
    {
        for (int i = spawnedShips.Count - 1; i >= 0; i--)
        {
            if (spawnedShips[i] == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(spawnedShips[i].gameObject);
            }
            else
            {
                DestroyImmediate(spawnedShips[i].gameObject);
            }
        }

        spawnedShips.Clear();
    }
}
