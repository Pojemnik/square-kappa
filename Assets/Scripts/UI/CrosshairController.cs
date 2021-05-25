using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public float hitMarkerDisplayTime;
    public float hitMarkerFadeTime;
    public GameObject hitMarker;

    private UnityEngine.UI.Image hitMarkerImage;

    void Start()
    {
        hitMarkerImage = hitMarker.GetComponent<UnityEngine.UI.Image>();
        hitMarkerImage.color = Color.clear;
    }

    public void OnEnemyHit()
    {
        hitMarkerImage.color = Color.white;
        hitMarkerImage.CrossFadeAlpha(1, 0, true);
        StartCoroutine(HideHitMarker(hitMarkerDisplayTime));
    }

    public void OnEnemyDeath()
    {
        hitMarkerImage.color = Color.red;
        hitMarkerImage.CrossFadeAlpha(1, 0, true);
        StartCoroutine(HideHitMarker(hitMarkerDisplayTime));
    }

    private IEnumerator HideHitMarker(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        hitMarkerImage.CrossFadeAlpha(0, hitMarkerFadeTime, true);
    }
}
