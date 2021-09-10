using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIImageController : MonoBehaviour
{
    [SerializeField]
    private GameObject image;
    [SerializeField]
    private float displayTime;
    [SerializeField]
    private bool localizedImage;

    private void Awake()
    {
        image.SetActive(false);
    }

    public void ShowScreenForTime()
    {
        image.SetActive(true);
        StartCoroutine(WaitAndHideImage(displayTime));
    }

    public void ShowScreen()
    {
        image.SetActive(true);
        if(localizedImage)
        {
            var localizer = image.GetComponent<UnityEngine.Localization.PropertyVariants.GameObjectLocalizer>();
            localizer.ApplyLocaleVariant(UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale);
        }
    }

    public void HideScreen()
    {
        image.SetActive(false);
    }

    private IEnumerator WaitAndHideImage(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        HideScreen();
    }
}
