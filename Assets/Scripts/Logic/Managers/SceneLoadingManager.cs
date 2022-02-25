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
    private ScreenCoverController currentScreenCover;
    private bool loadingConfirmed;

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

    private IEnumerator LoadSceneIfNotLoaded(string sceneName, bool activate)
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

    private IEnumerator UnloadSceneIfLoaded(string sceneName)
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
            if (!SceneManager.GetSceneByName(lvl).isLoaded)
            {
                operations.Add(SceneManager.LoadSceneAsync(lvl, LoadSceneMode.Additive));
            }
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
            if (SceneManager.GetSceneByName(lvl).isLoaded)
            {
                operations.Add(SceneManager.UnloadSceneAsync(lvl));
            }
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

    private void LoadingConfirmationListener()
    {
        loadingConfirmed = true;
    }

    private IEnumerator LoadLevel(int levelIndex, bool waitForPlayerConfirmation)
    {
        int currentLevelIndex = (int)currentLevel;
        yield return StartCoroutine(LoadSceneIfNotLoaded(loadingScene, true));
        LoadingScreenController loadingScreen = FindObjectOfType<LoadingScreenController>();
        loadingScreen.InitLoading(currentLevel);
        if(waitForPlayerConfirmation)
        {
            loadingConfirmed = false;
            loadingScreen.okButton.onClick.AddListener(LoadingConfirmationListener);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        yield return StartCoroutine(UnloadSceneIfLoaded(levelsBaseScene));
        yield return StartCoroutine(UnloadSceneIfLoaded(menuScene));
        if (currentLevel != LevelIndexEnum.Menu && currentLevel != LevelIndexEnum.Other)
        {
            yield return StartCoroutine(UnloadMultipleScenes(levels[currentLevelIndex].scenes));
        }
        yield return StartCoroutine(UnloadMultipleScenes(levels[levelIndex].scenes));
        yield return StartCoroutine(LoadSceneIfNotLoaded(levelsBaseScene, false));
        yield return StartCoroutine(LoadMultipleScenes(levels[levelIndex].scenes));
        if (waitForPlayerConfirmation)
        {
            loadingScreen.ShowOKButton();
            while (!loadingConfirmed)
            {
                yield return null;
            }
        }
        yield return StartCoroutine(UnloadSceneIfLoaded(loadingScene));
        DeactivateBaseScene();
        currentLevel = (LevelIndexEnum)levelIndex;
        currentScreenCover = FindObjectOfType<ScreenCoverController>();
        EventManager.Instance.TriggerEvent("GameStart");
        if (currentScreenCover == null)
        {
            Debug.LogWarning("Screen cover not found");
        }
        else
        {
            yield return StartCoroutine(currentScreenCover.HideCoverCoroutine());
        }
    }

    private IEnumerator LoadMenu()
    {
        if (currentLevel == LevelIndexEnum.Menu)
        {
            yield break;
        }
        if (currentScreenCover == null)
        {
            Debug.LogWarning("Screen cover not cached. This may be caused by a scene loading error");
        }
        else
        {
            yield return StartCoroutine(currentScreenCover.ShowCoverCoroutine());
        }
        yield return StartCoroutine(LoadSceneIfNotLoaded(loadingScene, true));
        LoadingScreenController loadingScreen = FindObjectOfType<LoadingScreenController>();
        loadingScreen.InitLoading(LevelIndexEnum.Menu);
        if (currentLevel != LevelIndexEnum.Menu && currentLevel != LevelIndexEnum.Other)
        {
            yield return StartCoroutine(UnloadMultipleScenes(levels[(int)currentLevel].scenes));
        }
        yield return StartCoroutine(UnloadSceneIfLoaded(levelsBaseScene));
        yield return StartCoroutine(LoadSceneIfNotLoaded(menuScene, true));
        yield return StartCoroutine(UnloadSceneIfLoaded(loadingScene));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator LoadOneScene(string sceneName)
    {
        yield return UnloadSceneIfLoaded(menuScene);
        foreach (Level lvl in levels)
        {
            yield return UnloadMultipleScenes(lvl.scenes);
        }
        yield return UnloadSceneIfLoaded(levelsBaseScene);
        yield return LoadSceneIfNotLoaded(levelsBaseScene, false);
        yield return LoadSceneIfNotLoaded(sceneName, true);
        EventManager.Instance.TriggerEvent("GameStart");
        currentLevel = LevelIndexEnum.Other;
    }

    public void StartLevel(int level, bool waitForConfirmation)
    {
        StartCoroutine(LoadLevel(level, waitForConfirmation));
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
                StartLevel(level, false);
                break;
        }
    }

    private void DeactivateBaseScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != levelsBaseScene && SceneManager.GetSceneAt(i).name != baseScene)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
            }
        }
    }

    public void ReloadGame()
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
            yield return StartCoroutine(LoadLevel((int)currentLevel, false));
            DeactivateBaseScene();
        }
    }
}
