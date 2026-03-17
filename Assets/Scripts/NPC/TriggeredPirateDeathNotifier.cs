using UnityEngine;

[RequireComponent(typeof(NpcPirateShip))]
public class TriggeredPirateDeathNotifier : MonoBehaviour
{
    [SerializeField] private PirateStockDropPopup popup;

    private NpcPirateShip pirate;
    private bool notified;

    public void SetPopup(PirateStockDropPopup popupInstance)
    {
        popup = popupInstance;
    }

    private void Awake()
    {
        pirate = GetComponent<NpcPirateShip>();
    }

    private void Update()
    {
        if (notified || pirate == null || !pirate.IsDestroyed)
        {
            return;
        }

        notified = true;
        if (popup != null)
        {
            popup.Show();
        }
    }
}