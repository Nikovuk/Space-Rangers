using UnityEngine;

[RequireComponent(typeof(NpcShipPatrol))]
public class NpcPirateShip : MonoBehaviour
{
    [SerializeField] private NpcShipPatrol patrol;

    public bool IsDestroyed => patrol == null || patrol.IsDestroyed;
    public NpcShipRoute CurrentRoute => patrol == null ? null : patrol.CurrentRoute;

    private void Awake()
    {
        if (patrol == null)
        {
            patrol = GetComponent<NpcShipPatrol>();
        }
    }

    public void ReceiveDamage(float damage)
    {
        if (patrol != null)
        {
            patrol.ReceiveDamage(damage);
        }
    }

    public void TriggerEscape(float duration)
    {
        if (patrol != null)
        {
            patrol.TriggerEscape(duration);
        }
    }

    public void ReturnToBase()
    {
        if (patrol == null)
        {
            return;
        }

        NpcPirateRoamArea roam = patrol.GetComponent<NpcPirateRoamArea>();
        if (roam != null)
        {
            roam.ReturnToBase();
        }
    }
}
