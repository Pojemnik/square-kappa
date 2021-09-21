using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject pauseBlurObject;
    [SerializeField]
    private GameObject gameUIObject;
    [Header("Settings")]
    [SerializeField]
    private bool pauseWhenNotFocused;

    private bool isPaused = false;

    public void ChangePauseState()
    {
        isPaused = !isPaused;
        if(isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        EventManager.Instance.TriggerEvent("Pause");
        pauseBlurObject.SetActive(true);
        gameUIObject.SetActive(false);
    }

    private void Unpause()
    {

        isPaused = false;
        EventManager.Instance.TriggerEvent("Unpause");
        pauseBlurObject.SetActive(false);
        gameUIObject.SetActive(true);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!pauseWhenNotFocused)
        {
            return;
        }
        if (hasFocus)
        {
            if(isPaused)
            {
                Unpause();
            }
        }
        else
        {
            if(!isPaused)
            {
                Pause();
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseWhenNotFocused)
        {
            return;
        }
        if (pauseStatus)
        {
            if (!isPaused)
            {
                Pause();
            }
        }
        else
        {
            if (isPaused)
            {
                Unpause();
            }
        }
    }
}
