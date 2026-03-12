using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NpcShipPatrol : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private NpcShipRoute route;
    [SerializeField] private bool startFromPointA = true;
    [SerializeField] private float arriveDistance = 4f;
    [SerializeField] private float waitAtPointSeconds = 1.5f;

    [Header("Space Flight")]
    [SerializeField] private float cruiseSpeed = 30f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float brakingDistance = 40f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private float maxSpeed = 40f;

    private Rigidbody rb;
    private bool targetIsPointA;
    private bool isWaiting;
    private float waitTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        if (maxSpeed < cruiseSpeed)
        {
            maxSpeed = cruiseSpeed;
        }

        targetIsPointA = !startFromPointA;
    }

    private void FixedUpdate()
    {
        if (route == null || !route.IsValid)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 0.75f);
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                targetIsPointA = !targetIsPointA;
            }

            return;
        }

        Vector3 targetPosition = route.GetPointPosition(targetIsPointA);
        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= arriveDistance)
        {
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

        float desiredSpeed = cruiseSpeed * speedFactor;
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

    public void SetRoute(NpcShipRoute newRoute, bool beginFromPointA)
    {
        route = newRoute;
        startFromPointA = beginFromPointA;
        targetIsPointA = !startFromPointA;
        isWaiting = false;
        waitTimer = 0f;
    }
}
