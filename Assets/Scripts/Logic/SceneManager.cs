using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Vectory screen")]
    [SerializeField]
    private bool enableVictory;
    [SerializeField]
    private GameObject victoryScreen;
    [SerializeField]
    private float victoryScreenDisplayTime;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnVictory()
    {
        if (enableVictory)
        {
            StartCoroutine(DisplayVictoryScreen(victoryScreenDisplayTime));
        }
    }

    private IEnumerator DisplayVictoryScreen(float time)
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        victoryScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
