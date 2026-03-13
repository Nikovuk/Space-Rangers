using UnityEngine;

[RequireComponent(typeof(NpcShipPatrol), typeof(Rigidbody))]
public class NpcPirateRoamArea : MonoBehaviour
{
    [SerializeField] private Transform pirateBase;
    [Header("Roam Area")]
    [SerializeField] private Transform areaPointA;
    [SerializeField] private Transform areaPointB;
    [SerializeField] private float routePointOffsetRadius = 40f;

    [Header("Points Of Interest")]
    [SerializeField] private Transform[] pointsOfInterest = new Transform[3];
    [SerializeField] private float interestPull = 0.35f;
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float turnSpeed = 4f;
    [SerializeField] private float arriveDistance = 6f;
    [SerializeField] private float routeSearchCooldown = 6f;

    private Rigidbody rb;
    private NpcShipPatrol patrol;
    private Vector3 targetPoint;
    private bool returningToBase;
    private NpcShipRoute[] cachedRoutes;
    private float routeSearchTimer;

    public void SetPirateBase(Transform baseTransform)
    {
        pirateBase = baseTransform;
        ReturnToBase();
    }

    public void ReturnToBase()
    {
        returningToBase = true;
        targetPoint = pirateBase == null ? transform.position : pirateBase.position;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        patrol = GetComponent<NpcShipPatrol>();

        if (patrol != null)
        {
            patrol.enabled = false;
        }

        rb.useGravity = false;
        PickNewTarget();
    }

    private void FixedUpdate()
    {
        targetPoint = ApplyInterestPull(targetPoint);
        Vector3 toTarget = targetPoint - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= arriveDistance)
        {
            if (returningToBase)
            {
                returningToBase = false;
            }

            PickNewTarget();
            return;
        }

        Vector3 direction = toTarget / Mathf.Max(distance, 0.001f);
        rb.linearVelocity = direction * moveSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
    }

    private Vector3 ApplyInterestPull(Vector3 currentTarget)
    {
        Transform interest = GetClosestInterestPoint();
        if (interest == null)
        {
            return currentTarget;
        }

        return Vector3.Lerp(currentTarget, interest.position, Mathf.Clamp01(interestPull));
    }

    private Transform GetClosestInterestPoint()
    {
        Transform closest = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < pointsOfInterest.Length; i++)
        {
            Transform point = pointsOfInterest[i];
            if (point == null)
            {
                continue;
            }

            float sqrDistance = (point.position - transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = point;
            }
        }

        return closest;
    }

    private Vector3 GetRandomPointInArea(Vector3 fallbackCenter)
    {
        if (areaPointA == null || areaPointB == null)
        {
            return fallbackCenter + Random.insideUnitSphere * routePointOffsetRadius;
        }

        Vector3 a = areaPointA.position;
        Vector3 b = areaPointB.position;
        return new Vector3(
            Random.Range(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x)),
            Random.Range(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y)),
            Random.Range(Mathf.Min(a.z, b.z), Mathf.Max(a.z, b.z))
        );
    }

    private void PickNewTarget()
    {
        if (returningToBase)
        {
            targetPoint = pirateBase == null ? transform.position : pirateBase.position;
            return;
        }

        routeSearchTimer -= Time.deltaTime;
        if (cachedRoutes == null || cachedRoutes.Length == 0 || routeSearchTimer <= 0f)
        {
            cachedRoutes = FindObjectsOfType<NpcShipRoute>();
            routeSearchTimer = Mathf.Max(1f, routeSearchCooldown);
        }

        if (cachedRoutes != null && cachedRoutes.Length > 0)
        {
            NpcShipRoute route = cachedRoutes[Random.Range(0, cachedRoutes.Length)];
            if (route != null && route.IsValid)
            {
                Vector3 routePoint = route.GetPointPosition(Random.value > 0.5f);
                targetPoint = GetRandomPointInArea(routePoint);
                return;
            }
        }

        Vector3 center = pirateBase == null ? transform.position : pirateBase.position;
        targetPoint = GetRandomPointInArea(center);
    }
}
