using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [SerializeField]
    private string baseScene;

    private int scenesToReloadCount;
    private int reloadedScenes;

    private void Awake()
    {
        RegisterInstance(this);
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", ReloadGame);
        EventManager.Instance.AddListener("Victory", ReloadGame);
        reloadedScenes = 0;
        //foreach(string sceneName in startupScenes)
        //{
        //    SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += OnStartupSceneLoaded;
        //}
    }

    private void OnReloadEnd(AsyncOperation operation)
    {
        reloadedScenes++;
        if (reloadedScenes == scenesToReloadCount)
        {
            EventManager.Instance.TriggerEvent("GameReloaded");
            reloadedScenes = 0;
        }
    }

    private void ReloadGame()
    {
        scenesToReloadCount = SceneManager.sceneCount - 1;
        List<int> scenesToReload = new List<int>(scenesToReloadCount);
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
            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive).completed += OnReloadEnd;
        }
    }
}
