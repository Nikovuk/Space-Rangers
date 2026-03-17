using UnityEngine;

[RequireComponent(typeof(NpcPirateShip))]
public class TriggeredPirateAttacker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 18f;
    [SerializeField] private float attackDistance = 6f;
    [SerializeField] private float dps = 8f;

    private PlayerResources targetResources;

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
            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 6f * Time.deltaTime);
        }

        if (distance <= attackDistance && targetResources != null)
        {
            targetResources.ReceiveDamage(dps * Time.deltaTime);
        }
    }
}