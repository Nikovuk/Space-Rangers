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
    [SerializeField] private float spawnOffsetRadius = 6f;

    private readonly List<NpcShipPatrol> spawnedShips = new List<NpcShipPatrol>();
    private readonly List<NpcShipRoute> runtimeRoutes = new List<NpcShipRoute>();
    private readonly List<RouteSpawnState> routeSpawnStates = new List<RouteSpawnState>();

    private class RouteSpawnState
    {
        public NpcShipRoute Route;
        public float Timer;
        public bool NextStartFromA = true;
    }

    private void Start()
    {
        InitializeRouteStates();
    }

    private void Update()
    {
        if (npcShipPrefab == null)
        {
            return;
        }

        for (int i = 0; i < routeSpawnStates.Count; i++)
        {
            RouteSpawnState state = routeSpawnStates[i];
            if (state.Route == null || !state.Route.IsValid)
            {
                continue;
            }

            state.Timer -= Time.deltaTime;
            if (state.Timer > 0f)
            {
                continue;
            }

            int shipsToSpawn = state.Route.GetShipsToSpawn();
            SpawnShipsForRoute(state, shipsToSpawn);
            state.Timer = state.Route.GetNextSpawnDelay();
        }
    }

    [ContextMenu("Respawn Traffic")]
    public void RespawnTraffic()
    {
        ClearTraffic();
        InitializeRouteStates();
    }

    [ContextMenu("Collect Routes From Root")]
    public void CollectRoutesFromRoot()
    {
        if (routesRoot == null)
        {
            return;
        }

        NpcShipRoute[] foundRoutes = routesRoot.GetComponentsInChildren<NpcShipRoute>(includeInactiveRoutes);
        if (foundRoutes == null || foundRoutes.Length == 0)
        {
            return;
        }

        routes.Clear();
        for (int i = 0; i < foundRoutes.Length; i++)
        {
            if (foundRoutes[i] != null)
            {
                routes.Add(foundRoutes[i]);
            }
        }
    }

    private void InitializeRouteStates()
    {
        BuildRuntimeRoutes();
        routeSpawnStates.Clear();

        for (int i = 0; i < runtimeRoutes.Count; i++)
        {
            NpcShipRoute route = runtimeRoutes[i];
            RouteSpawnState state = new RouteSpawnState
            {
                Route = route,
                Timer = route.GetNextSpawnDelay(),
                NextStartFromA = i % 2 == 0
            };

            routeSpawnStates.Add(state);
        }
    }

    private void SpawnShipsForRoute(RouteSpawnState state, int shipsCount)
    {
        for (int shipIndex = 0; shipIndex < shipsCount; shipIndex++)
        {
            bool startFromA = state.NextStartFromA;
            Vector3 spawnPoint = state.Route.GetPointPosition(startFromA);
            Vector3 randomOffset = Random.insideUnitSphere * spawnOffsetRadius;

            NpcShipPatrol ship = Instantiate(npcShipPrefab, spawnPoint + randomOffset, Quaternion.identity, transform);
            ship.SetRoute(state.Route, startFromA);
            spawnedShips.Add(ship);

            state.NextStartFromA = !state.NextStartFromA;
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
