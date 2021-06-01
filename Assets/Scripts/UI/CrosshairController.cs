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

    [Header("Player")]
    public GameObject playerCenter;

    private UnityEngine.UI.Image damageMarkerImage;
    private UnityEngine.UI.Image hitMarkerImage;

    void Start()
    {
        hitMarkerImage = hitMarker.GetComponent<UnityEngine.UI.Image>();
        hitMarkerImage.color = Color.clear;
        damageMarkerImage = damageMarker.GetComponent<UnityEngine.UI.Image>();
        //damageMarkerimage.color = Color.clear;
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

    public void OnPlayerHit(Vector3 projectileDirection)
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 towardsHitPoint = -projectileDirection;
        Vector2 screenPosition = new Vector2(Vector3.Dot(towardsHitPoint, cameraTransform.right), Vector3.Dot(towardsHitPoint, cameraTransform.up));
        damageMarkerImage.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenPosition.y, screenPosition.x) * Mathf.Rad2Deg + 90);
    }

    private IEnumerator HideHitMarker(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        hitMarkerImage.CrossFadeAlpha(0, hitMarkerFadeTime, true);
    }
}
