using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [System.Serializable]
    public struct Level
    {
        public string name;
        public string[] scenes;
    }

    [Header("Levels info")]
    [SerializeField]
    private string levelsBaseScene;
    [SerializeField]
    private Level[] levels;

    [Header("Reloading")]
    [SerializeField]
    private float reloadDelay;

    [Header("Menu")]
    [SerializeField]
    private string menuScene;
    [SerializeField]
    private bool loadMenuOnStart;

    [Header("Base")]
    [SerializeField]
    private string baseScene;

    [Header("Loading")]
    [SerializeField]
    private string loadingScene;

    private int scenesLeftToLoadOrUnload;
    private List<GameObject> removeOnReload;
    private bool loadingInProgress;
    
    private enum CurrentLevel : int
    {
        Level1 = 0,
        Level2,
        Level3,
        Menu
    }

    private CurrentLevel currentLevel;

    public void AddObjectToRemoveOnReload(GameObject obj)
    {
        removeOnReload.Add(obj);
    }

    private IEnumerator LoadScene(string sceneName, bool activate)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (activate)
            {
                loading.allowSceneActivation = true;
            }
            while (!loading.isDone)
            {
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            Debug.LogFormat("Loaded scene {0}", sceneName);
        }
        else
        {
            Debug.LogFormat("Scene {0} was already loaded", sceneName);
        }
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        AsyncOperation loading;
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(sceneName);
            while (!loading.isDone)
            {
                yield return null;
            }
            Debug.LogFormat("Unloaded scene {0}", sceneName);
        }
        else
        {
            Debug.LogFormat("Scene {0} was already unloaded", sceneName);
        }
    }

    private IEnumerator LoadMultipleScenes(string[] scenesToLoad)
    {
        List<AsyncOperation> operations = new List<AsyncOperation>();
        foreach (string lvl in scenesToLoad)
        {
            operations.Add(SceneManager.LoadSceneAsync(lvl, LoadSceneMode.Additive));
        }
        int counter = 0;
        while (counter != operations.Count)
        {
            yield return null;
            counter = 0;
            foreach (AsyncOperation operation in operations)
            {
                if (operation.isDone)
                {
                    counter++;
                }
            }
        }
    }

    private IEnumerator UnloadMultipleScenes(string[] scenesToLoad)
    {
        List<AsyncOperation> operations = new List<AsyncOperation>();
        foreach (string lvl in scenesToLoad)
        {
            operations.Add(SceneManager.UnloadSceneAsync(lvl));
        }
        int counter = 0;
        while (counter != operations.Count)
        {
            yield return null;
            counter = 0;
            foreach (AsyncOperation operation in operations)
            {
                if (operation.isDone)
                {
                    counter++;
                }
            }
        }
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        yield return StartCoroutine(LoadScene(loadingScene, true));
        yield return StartCoroutine(UnloadScene(menuScene));
        yield return StartCoroutine(UnloadScene(levelsBaseScene));
        yield return StartCoroutine(LoadScene(levelsBaseScene, false));
        yield return StartCoroutine(LoadMultipleScenes(levels[levelIndex].scenes));
        yield return StartCoroutine(UnloadScene(loadingScene));
        DeactivateBaseScene();
        EventManager.Instance.TriggerEvent("GameStart");
        currentLevel = (CurrentLevel)levelIndex;
    }

    private IEnumerator LoadMenu()
    {
        if(currentLevel == CurrentLevel.Menu)
        {
            yield break;
        }
        yield return StartCoroutine(LoadScene(loadingScene, true));
        yield return StartCoroutine(UnloadMultipleScenes(levels[(int)currentLevel].scenes));
        yield return StartCoroutine(UnloadScene(levelsBaseScene));
        yield return StartCoroutine(LoadScene(menuScene, true));
        yield return StartCoroutine(UnloadScene(loadingScene));
    }

    public void StartLevel(int level)
    {
        StartCoroutine(LoadLevel(level));
    }

    public void GoFromLevelToMenu()
    {
        EventManager.Instance.TriggerEvent("GameQuit");
        StartCoroutine(LoadMenu());
    }

    private IEnumerator LoadMenuOnly()
    {
        AsyncOperation loading;
        if (!SceneManager.GetSceneByName(menuScene).isLoaded)
        {
            loading = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
            loading.allowSceneActivation = true;
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Menu scene was already loaded");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(menuScene));
    }

    private void Awake()
    {
        removeOnReload = new List<GameObject>();
        RegisterInstance(this);
        if (loadMenuOnStart)
        {
            StartCoroutine(LoadMenuOnly());
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", ReloadGame);
        EventManager.Instance.AddListener("Victory", ReloadGame);
    }

    private void OnLoadingEnd(AsyncOperation _, string eventToSend)
    {
        scenesLeftToLoadOrUnload--;
        if (scenesLeftToLoadOrUnload == 0 && !loadingInProgress)
        {
            EventManager.Instance.TriggerEvent(eventToSend);
            DeactivateBaseScene();
        }
    }

    private void DeactivateBaseScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != levelsBaseScene)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
            }
        }
    }

    private void ReloadGame()
    {
        StartCoroutine(ReloadGameCoroutine());
    }

    private IEnumerator ReloadGameCoroutine()
    {
        yield return new WaitForSecondsRealtime(reloadDelay);
        scenesLeftToLoadOrUnload = SceneManager.sceneCount - 1;
        List<int> scenesToReload = new List<int>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != SceneManager.GetSceneByName(levelsBaseScene))
            {
                scenesToReload.Add(scene.buildIndex);
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        foreach (GameObject go in removeOnReload)
        {
            Destroy(go);
        }
        foreach (int sceneIndex in scenesToReload)
        {
            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive).completed += (a) => OnLoadingEnd(a, "GameReloaded");
        }
    }
}
