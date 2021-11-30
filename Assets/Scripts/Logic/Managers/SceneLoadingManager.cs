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

    [Header("Base")]
    [SerializeField]
    private string baseScene;

    [Header("Loading")]
    [SerializeField]
    private string loadingScene;

    [Header("Startup")]
    [SerializeField]
    private LevelIndexEnum loadOnStartup;
    [SerializeField]
    private string otherScene;

    private List<GameObject> removeOnReload;

    public enum LevelIndexEnum : int
    {
        Level1 = 0,
        Level2,
        Level3,
        Menu,
        Other
    }

    public LevelIndexEnum CurrentLevel { get => currentLevel; }

    private LevelIndexEnum currentLevel;

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
        int currentLevelIndex = (int)currentLevel;
        yield return StartCoroutine(LoadScene(loadingScene, true));
        yield return StartCoroutine(UnloadScene(menuScene));
        yield return StartCoroutine(UnloadScene(levelsBaseScene));
        yield return StartCoroutine(LoadScene(levelsBaseScene, false));
        if (currentLevel != LevelIndexEnum.Menu && currentLevel != LevelIndexEnum.Other)
        {
            yield return StartCoroutine(UnloadMultipleScenes(levels[currentLevelIndex].scenes));
        }
        yield return StartCoroutine(LoadMultipleScenes(levels[levelIndex].scenes));
        yield return StartCoroutine(UnloadScene(loadingScene));
        DeactivateBaseScene();
        currentLevel = (LevelIndexEnum)levelIndex;
        EventManager.Instance.TriggerEvent("GameStart");
    }

    private IEnumerator LoadMenu()
    {
        if (currentLevel == LevelIndexEnum.Menu)
        {
            yield break;
        }
        yield return StartCoroutine(LoadScene(loadingScene, true));
        if (currentLevel != LevelIndexEnum.Menu && currentLevel != LevelIndexEnum.Other)
        {
            yield return StartCoroutine(UnloadMultipleScenes(levels[(int)currentLevel].scenes));
        }
        yield return StartCoroutine(UnloadScene(levelsBaseScene));
        yield return StartCoroutine(LoadScene(menuScene, true));
        yield return StartCoroutine(UnloadScene(loadingScene));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator LoadOneScene(string sceneName)
    {
        yield return LoadScene(levelsBaseScene, false);
        yield return UnloadScene(menuScene);
        yield return LoadScene(sceneName, true);
        EventManager.Instance.TriggerEvent("GameStart");
        currentLevel = LevelIndexEnum.Other;
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

    private void Awake()
    {
        currentLevel = LevelIndexEnum.Other;
        removeOnReload = new List<GameObject>();
        RegisterInstance(this);
        switch (loadOnStartup)
        {
            case LevelIndexEnum.Menu:
                StartCoroutine(LoadMenu());
                break;
            case LevelIndexEnum.Other:
                StartCoroutine(LoadOneScene(otherScene));
                break;
            default:
                int level = (int)loadOnStartup;
                StartLevel(level);
                break;
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", ReloadGame);
        EventManager.Instance.AddListener("Victory", ReloadGame);
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
        if (currentLevel == LevelIndexEnum.Other || currentLevel == LevelIndexEnum.Menu)
        {
            Debug.LogError("Player died in incorrect scene state. This should never happen");
        }
        else
        {
            yield return new WaitForSecondsRealtime(reloadDelay);
            yield return StartCoroutine(UnloadMultipleScenes(levels[(int)currentLevel].scenes));
            yield return StartCoroutine(LoadLevel((int)currentLevel));
            DeactivateBaseScene();
        }
    }
}
