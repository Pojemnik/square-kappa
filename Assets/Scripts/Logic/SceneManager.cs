using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private float uiReloadDelay;

    private void Start()
    {
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("ReloadScene", ReloadScene);
        eventManager.AddListener("PlayerDeath", delegate { ReloadAfterDelay(uiReloadDelay); });
        eventManager.AddListener("Victory", delegate { ReloadAfterDelay(uiReloadDelay); });
    }

    private void ReloadAfterDelay(float time)
    {
        StartCoroutine(WaitForReload(time));
    }

    private IEnumerator WaitForReload(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ReloadScene();
    }

    private void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
