using UnityEngine;

[RequireComponent(typeof(NpcShipPatrol))]
public class NpcTraderShip : MonoBehaviour
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

    public void TriggerEscape(float duration)
    {
        if (patrol != null)
        {
            patrol.TriggerEscape(duration);
        }
    }

    public void ReceiveDamage(float damage)
    {
        if (patrol != null)
        {
            patrol.ReceiveDamage(damage);
        }
    }
}