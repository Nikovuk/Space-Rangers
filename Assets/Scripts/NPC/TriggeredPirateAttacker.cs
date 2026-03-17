using UnityEngine;

[RequireComponent(typeof(NpcPirateShip))]
public class TriggeredPirateAttacker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 18f;
    [SerializeField] private float laserRange = 24f;
    [SerializeField] private float keepDistance = 14f;
    [SerializeField] private float laserCooldown = 0.25f;
    [SerializeField] private float dps = 8f;

    private PlayerResources targetResources;
    private float laserTimer;

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
        targetResources = target == null ? null : target.GetComponent<PlayerResources>();
    }

    private void Awake()
    {
        if (target != null)
        {
            targetResources = target.GetComponent<PlayerResources>();
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        if (distance > 0.01f)
        {
            Vector3 direction = toTarget / distance;

            float moveDirection = distance < keepDistance ? -1f : 1f;
            transform.position += direction * moveDirection * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 6f * Time.deltaTime);
        }

        if (laserTimer > 0f)
        {
            laserTimer -= Time.deltaTime;
        }

        if (distance <= laserRange && distance >= keepDistance * 0.75f && targetResources != null && laserTimer <= 0f)
        {
            targetResources.ReceiveDamage(dps * Mathf.Max(0.05f, laserCooldown));
            laserTimer = Mathf.Max(0.05f, laserCooldown);
        }
    }
}
