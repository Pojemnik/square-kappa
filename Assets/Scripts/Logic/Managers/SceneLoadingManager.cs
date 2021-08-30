using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [SerializeField]
    private float uiReloadDelay;
    [SerializeField]
    private string baseScene;

    private void Start()
    {
        EventManager.Instance.AddListener("ReloadScene", ReloadGame);
        EventManager.Instance.AddListener("PlayerDeath", delegate { ReloadAfterDelay(uiReloadDelay); });
        EventManager.Instance.AddListener("Victory", delegate { ReloadAfterDelay(uiReloadDelay); });
    }

    private void ReloadAfterDelay(float time)
    {
        StartCoroutine(WaitForReload(time));
    }

    private IEnumerator WaitForReload(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ReloadGame();
    }

    private void ReloadGame()
    {
        List<int> scenesToReload = new List<int>(SceneManager.sceneCount - 1);
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != SceneManager.GetSceneByName(baseScene))
            {
                scenesToReload.Add(scene.buildIndex);
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        foreach (int sceneIndex in scenesToReload)
        {
            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        }
    }
}
