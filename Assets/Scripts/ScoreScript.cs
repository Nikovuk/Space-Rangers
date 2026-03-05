using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreScript : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text scoreText2;
    //[SerializeField] TMP_Text scoreText3;

    [Header("Events")]
    public UnityEvent OnCollectAll;

    int collCount = 0;
    int collected = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RaceRingScript[] collectables = FindObjectsByType<RaceRingScript>(FindObjectsSortMode.None);

        collCount = collectables.Length;

        foreach (RaceRingScript collectable in collectables)
            collectable.OnPlayerEnter.AddListener(OnCollect);

        updateScoreText(collected, collCount);
    }

    public void OnCollect()
    {
        if (collected >= collCount) return;

        collected++;

        updateScoreText(collected, collCount);

        if (collected >= collCount)
            OnCollectAll?.Invoke();
    }

    void updateScoreText(int collected, int collCount)
    {
        scoreText.text = "Score: " + collected.ToString() + " / " + collCount.ToString();
        scoreText2.text = "Score: " + collected.ToString() + " / " + collCount.ToString();
        //scoreText3.text = "Score: " + collected.ToString() + " / " + collCount.ToString();
    }
}
