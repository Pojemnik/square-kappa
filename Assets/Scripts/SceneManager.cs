using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private string startSceneName;

    [Header("Game over screen")]
    [SerializeField]
    private bool enableGameOver;
    [SerializeField]
    private GameObject gameOverScreen;
    [SerializeField]
    private float gameOverScreenDisplayTime;

    public void OnPlayerDeath()
    {
        if (enableGameOver)
        {
            StartCoroutine(DisplayGameOverScreen(gameOverScreenDisplayTime));
        }
    }

    private IEnumerator DisplayGameOverScreen(float time)
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(startSceneName);
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
