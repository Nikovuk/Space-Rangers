using UnityEngine;

[RequireComponent(typeof(NpcShipPatrol), typeof(Rigidbody))]
public class NpcPirateRoamArea : MonoBehaviour
{
    [SerializeField] private Transform pirateBase;
    [SerializeField] private float routePointOffsetRadius = 40f;
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
                targetPoint = routePoint + Random.insideUnitSphere * routePointOffsetRadius;
                return;
            }
        }

        Vector3 center = pirateBase == null ? transform.position : pirateBase.position;
        targetPoint = center + Random.insideUnitSphere * routePointOffsetRadius;
    }
}
