using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Hit marker")]
    public float hitMarkerDisplayTime;
    public float hitMarkerFadeTime;
    public GameObject hitMarker;

    [Header("Damage marker")]
    public float offsetFromCrosshair;
    public GameObject damageMarker;

    private UnityEngine.UI.Image damageMarkerimage;
    private UnityEngine.UI.Image hitMarkerImage;

    void Start()
    {
        hitMarkerImage = hitMarker.GetComponent<UnityEngine.UI.Image>();
        hitMarkerImage.color = Color.clear;
        damageMarkerimage = damageMarker.GetComponent<UnityEngine.UI.Image>();
        damageMarkerimage.color = Color.clear;
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

    public void OnPlayerHit(Vector3 hitPosition)
    {
        print(hitPosition);
    }

    private IEnumerator HideHitMarker(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        hitMarkerImage.CrossFadeAlpha(0, hitMarkerFadeTime, true);
    }
}
