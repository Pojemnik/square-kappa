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

    private IEnumerator LoadLevel(int levelIndex)
    {
        AsyncOperation loading;
        if (!SceneManager.GetSceneByName(loadingScene).isLoaded)
        {
            loading = SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            loading.allowSceneActivation = true;
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Loading scene was already loaded");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingScene));

        if (SceneManager.GetSceneByName(menuScene).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(menuScene);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Menu scene was already unloaded");
        }

        if (SceneManager.GetSceneByName(levelsBaseScene).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(levelsBaseScene);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Level base was already unloaded");
        }

        loading = SceneManager.LoadSceneAsync(levelsBaseScene, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
        Debug.Log("Loaded level base");

        List<AsyncOperation> operations = new List<AsyncOperation>();
        foreach (string lvl in levels[levelIndex].scenes)
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

        if (SceneManager.GetSceneByName(loadingScene).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(loadingScene);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Loadig scene was already unloaded");
        }

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
        AsyncOperation loading;
        if (!SceneManager.GetSceneByName(loadingScene).isLoaded)
        {
            loading = SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            loading.allowSceneActivation = true;
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Loading scene was already loaded");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingScene));

        List<AsyncOperation> operations = new List<AsyncOperation>();
        foreach (string lvl in levels[(int)currentLevel].scenes)
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

        if (SceneManager.GetSceneByName(levelsBaseScene).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(levelsBaseScene);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Level base was already unloaded");
        }

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

        if (SceneManager.GetSceneByName(loadingScene).isLoaded)
        {
            loading = SceneManager.UnloadSceneAsync(loadingScene);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Loading scene was already unloaded");
        }
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
