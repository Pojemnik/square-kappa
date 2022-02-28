using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<SceneLoadingManager.LevelIndexEnum, GameObject> screens;

    public UnityEngine.UI.Button okButton;

    private void Awake()
    {
        foreach(GameObject screen in screens.Values)
        {
            screen.SetActive(false);
        }
    }

    public void InitLoading(SceneLoadingManager.LevelIndexEnum levelIndex)
    {
        Debug.Log(levelIndex);
        if (!screens.ContainsKey(levelIndex))
        {
            Debug.LogErrorFormat("No loading screen for level {0}", levelIndex);
        }
        else
        {
            screens[levelIndex].SetActive(true);
        }
        okButton.gameObject.SetActive(false);
    }

    public void ShowOKButton()
    {
        okButton.gameObject.SetActive(true);
    }
}
