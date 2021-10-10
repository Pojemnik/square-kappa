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
    private List<GameObject> removeOnReload;

    public void AddObjectToRemoveOnReload(GameObject obj)
    {
        removeOnReload.Add(obj);
    }

    private void Awake()
    {
        removeOnReload = new List<GameObject>();
        RegisterInstance(this);
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", ReloadGame);
        EventManager.Instance.AddListener("Victory", ReloadGame);
        reloadedScenes = 0;
    }

    private void OnReloadEnd(AsyncOperation operation)
    {
        reloadedScenes++;
        if (reloadedScenes == scenesToReloadCount)
        {
            EventManager.Instance.TriggerEvent("GameReloaded");
            if(SceneManager.sceneCount > 1)
            {
                if(SceneManager.GetSceneAt(0).name == baseScene)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                }
                else
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
                }
            }
            reloadedScenes = 0;
        }
    }

    private void ReloadGame()
    {
        scenesToReloadCount = SceneManager.sceneCount - 1;
        List<int> scenesToReload = new List<int>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != SceneManager.GetSceneByName(baseScene))
            {
                scenesToReload.Add(scene.buildIndex);
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        foreach(GameObject go in removeOnReload)
        {
            Destroy(go);
        }
        foreach (int sceneIndex in scenesToReload)
        {
            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive).completed += OnReloadEnd;
        }
    }
}
