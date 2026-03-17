using UnityEngine;

public class PirateStockDropPopup : MonoBehaviour
{
    [SerializeField] private string message = "Цена акций пиратов упала";
    [SerializeField] private float duration = 2f;

    private float timer;
    private int visibleCount;

    public void Show()
    {
        visibleCount++;
        timer = Mathf.Max(0.1f, duration);
    }

    private void Update()
    {
        if (timer <= 0f)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            visibleCount = 0;
        }
    }

    private void OnGUI()
    {
        if (visibleCount <= 0 || timer <= 0f)
        {
            return;
        }

        Rect rect = new Rect(Screen.width * 0.3f, 20f, Screen.width * 0.4f, 40f);
        GUI.Box(rect, message + (visibleCount > 1 ? " x" + visibleCount : ""));
    }
}
