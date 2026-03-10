using UnityEngine;
using UnityEngine.Events;

public class RaceRingScript : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            OnPlayerEnter?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
