using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<SceneLoadingManager.LevelIndexEnum, Sprite> sprites;
    public UnityEngine.UI.Button okButton;

    private UnityEngine.UI.Image image;

    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        image.sprite = null;
    }

    public void InitLoading(SceneLoadingManager.LevelIndexEnum levelIndex)
    {
        if(!sprites.ContainsKey(levelIndex))
        {
            Debug.LogErrorFormat("No loading screen for level {0}", levelIndex);
        }
        image.sprite = sprites[levelIndex];
    }

}
