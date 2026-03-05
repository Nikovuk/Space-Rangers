using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    //[SerializeField] int nextSceneIndex = 0;
    [SerializeField] string NextSceneName;
    public void freezeScene()
    {
        Time.timeScale = 0.0f;
    }
    public void unfreezeScene()
    {
        Time.timeScale = 1.0f;
    }
    public void loadNextScene()
    {
        unfreezeScene();
        SceneManager.LoadScene(NextSceneName);
    }
    public void reloadScene()
    {
        unfreezeScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
