using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameOverScreenController : MonoBehaviour
{
    protected UIImageController imageController;
    private bool optionAlreadySelected = false;

    protected virtual void Start()
    {
        imageController = GetComponent<UIImageController>();
    }

    public void BackToMenu()
    {
        if(optionAlreadySelected)
        {
            return;
        }
        SceneLoadingManager.Instance.GoFromLevelToMenu();
        optionAlreadySelected = true;
    }

    public void RestartLevel()
    {
        if (optionAlreadySelected)
        {
            return;
        }
        SceneLoadingManager.Instance.ReloadGame();
        optionAlreadySelected = true;
    }

    protected void ShowScreenAndUnlockCursor()
    {
        imageController.ShowScreen();
        EventManager.Instance.TriggerEvent("UnlockCursor");
    }
}
