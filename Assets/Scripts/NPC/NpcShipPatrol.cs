using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NpcShipPatrol : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private NpcShipRoute route;
    [SerializeField] private bool startFromPointA = true;
    [SerializeField] private float arriveDistance = 4f;
    [SerializeField] private float waitAtPointSeconds = 1.5f;

    [Header("Path Shape")]
    [SerializeField] private bool useRouteAreaDetours = true;

    [Header("Space Flight")]
    [SerializeField] private float cruiseSpeed = 30f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float brakingDistance = 40f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private float maxSpeed = 40f;

    [Header("Trading")]
    [SerializeField] private StockMarketManager stockMarketManager;

    [Header("Combat")]
    [SerializeField] private float maxHull = 20f;
    [SerializeField] private float escapeSpeedMultiplier = 1.25f;


    private bool hasCargo = true;

    private float hull;
    private bool isDestroyed;
    private float escapeTimer;
    private int convoyId = -1;

    public bool IsDestroyed => isDestroyed;
    public NpcShipRoute CurrentRoute => route;
    public int ConvoyId => convoyId;
    private Rigidbody rb;
    private bool targetIsPointA;
    private bool isWaiting;
    private float waitTimer;

    private bool usingDetourTarget;
    private Vector3 detourTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        if (maxSpeed < cruiseSpeed)
        {
            maxSpeed = cruiseSpeed;
        }

        hull = Mathf.Max(1f, maxHull);
        targetIsPointA = !startFromPointA;
    }

    private void Start()
    {
        PrepareLegDetour();
    }

    private void FixedUpdate()
    {
        if (route == null || !route.IsValid)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 0.75f);
            return;
        }

        if (escapeTimer > 0f)
        {
            escapeTimer -= Time.fixedDeltaTime;
        }

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                targetIsPointA = !targetIsPointA;
                PrepareLegDetour();
            }

            return;
        }

        Vector3 finalTargetPosition = route.GetPointPosition(targetIsPointA);
        Vector3 activeTarget = usingDetourTarget ? detourTarget : finalTargetPosition;
        Vector3 toTarget = activeTarget - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= arriveDistance)
        {
            if (usingDetourTarget)
            {
                usingDetourTarget = false;
                return;
            }

            isWaiting = true;
            waitTimer = waitAtPointSeconds;
            return;
        }

        Vector3 direction = toTarget / Mathf.Max(distance, 0.001f);

        float speedFactor = 1f;
        if (distance < brakingDistance)
        {
            speedFactor = Mathf.Clamp01(distance / Mathf.Max(brakingDistance, 0.001f));
            speedFactor = Mathf.SmoothStep(0.1f, 1f, speedFactor);
        }

        float activeSpeedMultiplier = escapeTimer > 0f ? Mathf.Max(1f, escapeSpeedMultiplier) : 1f;
        float desiredSpeed = cruiseSpeed * speedFactor * activeSpeedMultiplier;
        Vector3 desiredVelocity = direction * desiredSpeed;

        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );

        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        Vector3 lookDirection = rb.linearVelocity.sqrMagnitude > 0.2f ? rb.linearVelocity.normalized : direction;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    public void SetConvoyId(int id)
    {
        convoyId = id;
    }

    public void TriggerEscape(float duration)
    {
        escapeTimer = Mathf.Max(escapeTimer, duration);
    }

    public void ReceiveDamage(float damage)
    {
        if (isDestroyed)
        {
            return;
        }

        hull -= Mathf.Max(0f, damage);
        if (hull > 0f)
        {
            return;
        }

        isDestroyed = true;

        NpcPirateTraderSimulation simulation = FindObjectOfType<NpcPirateTraderSimulation>();
        if (simulation != null)
        {
            if (GetComponent<NpcPirateShip>() != null)
            {
                simulation.NotifyPirateDestroyed();
            }

            if (GetComponent<NpcTraderShip>() != null)
            {
                simulation.NotifyTraderDestroyed(route);
            }
        }

        Destroy(gameObject);
    }

    public void SetRoute(NpcShipRoute newRoute, bool beginFromPointA)
    {
        route = newRoute;
        startFromPointA = beginFromPointA;
        targetIsPointA = !startFromPointA;
        isWaiting = false;
        waitTimer = 0f;
        usingDetourTarget = false;
        PrepareLegDetour();
        hasCargo = true;
    }
    private void TryProcessTradeAtDestination()
    {
        if (stockMarketManager == null || route == null)
        {
            return;
        }

        bool destinationIsPointA = targetIsPointA;
        bool destinationIsPointB = !destinationIsPointA;

        if (destinationIsPointB && hasCargo)
        {
            if (!stockMarketManager.ApplyTraderTransfer(route.PointAStockSymbol, route.PointBStockSymbol, true))
            {
                return;
            }

            hasCargo = false;
        }

        if (destinationIsPointB && !hasCargo)
        {
            float buyPrice = stockMarketManager.GetTraderPurchasePrice(route.PointBStockSymbol);
            if (buyPrice > 0f)
            {
                hasCargo = true;
            }
        }
    }
    private void PrepareLegDetour()
    {
        if (!useRouteAreaDetours || route == null)
        {
            usingDetourTarget = false;
            return;
        }

        if (route.TryGetRandomAreaPosition(out Vector3 areaTarget))
        {
            detourTarget = areaTarget;
            usingDetourTarget = true;
            return;
        }

        usingDetourTarget = false;
    }
}
