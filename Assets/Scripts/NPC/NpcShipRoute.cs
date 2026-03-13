using System.Collections.Generic;
using UnityEngine;

public class NpcShipRoute : MonoBehaviour
{
    [Header("Route Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private List<Transform> areaPoints = new List<Transform>();

    [Header("Per Route Spawn Settings")]
    [Range(10f, 40f)]
    [SerializeField] private float minSpawnDelay = 10f;
    [Range(10f, 40f)]
    [SerializeField] private float maxSpawnDelay = 20f;
    [Range(1, 3)]
    [SerializeField] private int minShipsPerSpawn = 1;
    [Range(1, 3)]
    [SerializeField] private int maxShipsPerSpawn = 2;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public Transform PointA => pointA;
    public Transform PointB => pointB;

    public bool IsValid => pointA != null && pointB != null;

    public Vector3 GetPointPosition(bool usePointA)
    {
        if (!IsValid)
        {
            return transform.position;
        }

        return usePointA ? pointA.position : pointB.position;
    }

    public float GetNextSpawnDelay()
    {
        float min = Mathf.Min(minSpawnDelay, maxSpawnDelay);
        float max = Mathf.Max(minSpawnDelay, maxSpawnDelay);
        return Random.Range(min, max);
    }

    public int GetShipsToSpawn()
    {
        int min = Mathf.Min(minShipsPerSpawn, maxShipsPerSpawn);
        int max = Mathf.Max(minShipsPerSpawn, maxShipsPerSpawn);
        return Random.Range(min, max + 1);
    }

    public bool TryGetRandomAreaPosition(out Vector3 areaPosition)
    {
        areaPosition = Vector3.zero;

        if (!IsValid)
        {
            return false;
        }

        List<Vector3> points = GetAreaVolumePoints();
        if (points.Count == 0)
        {
            return false;
        }

        Vector3 min = points[0];
        Vector3 max = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        areaPosition = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );

        return true;
    }

    private List<Vector3> GetAreaVolumePoints()
    {
        List<Vector3> points = new List<Vector3>();

        if (pointA != null)
        {
            points.Add(pointA.position);
        }

        if (pointB != null)
        {
            points.Add(pointB.position);
        }

        for (int i = 0; i < areaPoints.Count; i++)
        {
            if (areaPoints[i] != null)
            {
                points.Add(areaPoints[i].position);
            }
        }

        return points;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !IsValid)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointA.position, 1.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointB.position, 1.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);

        for (int i = 0; i < areaPoints.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(areaPoints[i].position, 0.8f);
            Gizmos.DrawLine(pointA.position, areaPoints[i].position);
            Gizmos.DrawLine(pointB.position, areaPoints[i].position);
        }

        List<Vector3> points = GetAreaVolumePoints();
        if (points.Count < 2)
        {
            return;
        }

        Vector3 min = points[0];
        Vector3 max = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireCube((min + max) * 0.5f, max - min);
    }
}
