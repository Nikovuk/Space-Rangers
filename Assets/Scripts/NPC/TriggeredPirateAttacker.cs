using UnityEngine;

[RequireComponent(typeof(NpcPirateShip))]
public class TriggeredPirateAttacker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 18f;
    [SerializeField] private float laserRange = 24f;
    [SerializeField] private float keepDistance = 14f;
    [SerializeField] private float distanceTolerance = 2f;
    [SerializeField] private float laserCooldown = 0.25f;
    [SerializeField] private float laserDamage = 2f;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private float laserTimer;

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
    }

    public void SetLaserPrefab(GameObject prefab)
    {
        laserPrefab = prefab;
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
            float minDistance = Mathf.Max(0.1f, keepDistance - Mathf.Abs(distanceTolerance));
            float maxDistance = keepDistance + Mathf.Abs(distanceTolerance);
            float moveDirection = 0f;

            if (distance < minDistance)
            {
                moveDirection = -1f;
            }
            else if (distance > maxDistance)
            {
                moveDirection = 1f;
            }

            if (moveDirection != 0f)
            {
                transform.position += direction * moveDirection * speed * Time.deltaTime;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 6f * Time.deltaTime);
        }

        if (laserTimer > 0f)
        {
            laserTimer -= Time.deltaTime;
        }

        if (distance <= laserRange && distance >= keepDistance * 0.75f && laserTimer <= 0f)
        {
            ShootAtTarget();
            laserTimer = Mathf.Max(0.05f, laserCooldown);
        }
    }

    private void ShootAtTarget()
    {
        if (laserPrefab == null || target == null)
        {
            return;
        }

        Vector3 spawnPosition = firePoint == null ? transform.position + transform.forward * 2f : firePoint.position;
        Vector3 direction = (target.position - spawnPosition).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = transform.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject laser = Instantiate(laserPrefab, spawnPosition, rotation);
        LaserScript laserScript = laser.GetComponent<LaserScript>();
        if (laserScript != null)
        {
            laserScript.InitializeNpcShot(0f, laserDamage);
        }
    }
}
