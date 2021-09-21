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
    [SerializeField]
    private GameObject pauseUIObject;
    [Header("Settings")]
    [SerializeField]
    private bool pauseWhenNotFocused;

    private bool isPaused = false;

    private void Start()
    {
        Unpause();
    }

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

    public void Pause()
    {
        isPaused = true;
        EventManager.Instance.TriggerEvent("Pause");
        EventManager.Instance.TriggerEvent("UnlockCursor");
        pauseBlurObject.SetActive(true);
        pauseUIObject.SetActive(true);
        gameUIObject.SetActive(false);
    }

    public void Unpause()
    {
        isPaused = false;
        EventManager.Instance.TriggerEvent("LockCursor");
        EventManager.Instance.TriggerEvent("Unpause");
        pauseBlurObject.SetActive(false);
        pauseUIObject.SetActive(false);
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
