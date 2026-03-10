using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OreScript : MonoBehaviour
{
    public int oreAmount = 1;
    public string playerTag = "Player";

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var playerResources = other.GetComponent<PlayerResources>();
        if (playerResources == null)
            playerResources = other.GetComponentInParent<PlayerResources>();
        else
        {
            playerResources.AddCargo(oreAmount);

            Destroy(gameObject);
        }
    }
}
