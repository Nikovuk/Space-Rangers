using UnityEngine;

public class NpcShipRoute : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
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

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !IsValid)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointA.position, 1.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointB.position, 1.2f);
    }
}
