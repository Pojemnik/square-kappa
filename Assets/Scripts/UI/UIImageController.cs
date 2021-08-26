using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIImageController : MonoBehaviour
{
    private UnityEngine.UI.Image image;
    private EventManager eventManager;

    [SerializeField]
    private float displayTime;


    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        image.enabled = false;
        eventManager = FindObjectOfType<EventManager>();
    }

    public void ShowScreen()
    {
        image.enabled = true;
        StartCoroutine(WaitAndHideImage(displayTime));
    }

    public void HideScreen()
    {
        image.enabled = false;
    }

    private IEnumerator WaitAndHideImage(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        HideScreen();
    }
}
