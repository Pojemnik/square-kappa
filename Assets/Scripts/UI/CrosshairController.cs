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
    public float damageMarkerDisplayTime;
    public float damageMarkerFadeTime;
    public GameObject damageMarkerPrefab;

    [Header("Player")]
    public GameObject playerCenter;

    [Header("Crosshair lines")]
    public GameObject upLine;
    public GameObject downLine;
    public GameObject leftLine;
    public GameObject rightLine;

    [Header("Crosshair settings")]
    public float defaultRadius;
    public float radiusToSpreadRatio;
    public float minimalRadius;

    private UnityEngine.UI.Image damageMarkerImage;
    private UnityEngine.UI.Image hitMarkerImage;
    private UnityEngine.UI.Image upImage;
    private UnityEngine.UI.Image downImage;
    private UnityEngine.UI.Image leftImage;
    private UnityEngine.UI.Image rightImage;

    private WeaponController weapon;

    void Start()
    {
        hitMarkerImage = hitMarker.GetComponent<UnityEngine.UI.Image>();
        hitMarkerImage.color = Color.clear;
        damageMarkerImage = damageMarkerPrefab.GetComponent<UnityEngine.UI.Image>();
        damageMarkerImage.CrossFadeAlpha(0, 0, true);
        upImage = upLine.GetComponent<UnityEngine.UI.Image>();
        downImage = downLine.GetComponent<UnityEngine.UI.Image>();
        leftImage = leftLine.GetComponent<UnityEngine.UI.Image>();
        rightImage = rightLine.GetComponent<UnityEngine.UI.Image>();
        SetCrosshairRadius(defaultRadius);
    }

    private void Update()
    {
        if(weapon)
        {
            SetCrosshairRadius(weapon.spread * radiusToSpreadRatio + minimalRadius);
        }
    }

    void SetCrosshairRadius(float radius)
    {
        if(radius < minimalRadius)
        {
            radius = minimalRadius;
        }
        upLine.transform.localPosition = new Vector3(0, radius, 0);
        downLine.transform.localPosition = new Vector3(0, -radius, 0);
        leftLine.transform.localPosition = new Vector3(-radius, 0, 0);
        rightLine.transform.localPosition = new Vector3(radius, 0, 0);
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
        GameObject damageMarker = Instantiate(damageMarkerPrefab, transform);
        UnityEngine.UI.Image damageMarkerImage = damageMarker.GetComponent<UnityEngine.UI.Image>();
        damageMarkerImage.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenPosition.y, screenPosition.x) * Mathf.Rad2Deg + 90);
        damageMarkerImage.CrossFadeAlpha(1, 0, true);
        StartCoroutine(HideDamageMarker(damageMarkerDisplayTime, damageMarkerImage));
    }

    public void OnWeaponChange(WeaponController newWeapon)
    {
        weapon = newWeapon;
    }

    private IEnumerator HideHitMarker(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        hitMarkerImage.CrossFadeAlpha(0, hitMarkerFadeTime, true);
    }

    private IEnumerator HideDamageMarker(float time, UnityEngine.UI.Image marker)
    {
        yield return new WaitForSecondsRealtime(time);
        marker.CrossFadeAlpha(0, damageMarkerFadeTime, true);
        Destroy(marker.gameObject, damageMarkerFadeTime);
    }
}
