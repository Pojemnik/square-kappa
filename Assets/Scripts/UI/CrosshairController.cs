using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Hit marker")]
    [SerializeField]
    private float hitMarkerDisplayTime;
    [SerializeField]
    private float hitMarkerFadeTime;
    [SerializeField]
    private GameObject hitMarker;

    [Header("Damage marker")]
    [SerializeField]
    private float damageMarkerDisplayTime;
    [SerializeField]
    private float damageMarkerFadeTime;
    [SerializeField]
    private float damageMarkerDistanceFromCenter;
    [SerializeField]
    private bool damageMarkerShowWhenForward;
    [SerializeField]
    private float damageMarkerMaxForwardAngle;
    [SerializeField]
    private GameObject damageMarker;

    [Header("Player")]
    [SerializeField]
    private GameObject playerCenter;

    [Header("Crosshair lines")]
    [SerializeField]
    private GameObject upLine;
    [SerializeField]
    private GameObject downLine;
    [SerializeField]
    private GameObject leftLine;
    [SerializeField]
    private GameObject rightLine;

    [Header("Crosshair settings")]
    [SerializeField]
    private float defaultRadius;
    [SerializeField]
    private float radiusToSpreadRatio;
    [SerializeField]
    private float minimalRadius;

    private UnityEngine.UI.Image damageMarkerImage;
    private UnityEngine.UI.Image hitMarkerImage;
    private WeaponController weapon;
    private Coroutine damageMarkerFadeOut;
    private float hitMarkerHideTimestamp;

    void Start()
    {
        hitMarkerImage = hitMarker.GetComponent<UnityEngine.UI.Image>();
        hitMarkerImage.color = Color.clear;
        damageMarkerImage = damageMarker.GetComponent<UnityEngine.UI.Image>();
        damageMarkerImage.CrossFadeAlpha(0, 0, true);
        damageMarker.SetActive(false);
        SetCrosshairRadius(defaultRadius);
        EventManager.Instance.AddListener("GameReloaded", OnGameReload);
    }

    private void OnGameReload()
    {
        damageMarkerImage.CrossFadeAlpha(0, damageMarkerFadeTime, true); 
        damageMarker.SetActive(false); 
        hitMarkerImage.color = Color.clear;
    }

    private void Update()
    {
        if (weapon)
        {
            SetCrosshairRadius(weapon.Spread * radiusToSpreadRatio + minimalRadius);
        }
        if (Time.time >= hitMarkerHideTimestamp)
        {
            if (hitMarkerImage.color.a > 0)
            {
                Color temp = hitMarkerImage.color;
                temp.a = Mathf.Clamp(1 - ((Time.time - hitMarkerHideTimestamp) / hitMarkerFadeTime), 0, 1);
                hitMarkerImage.color = temp;
            }
        }
    }

    void SetCrosshairRadius(float radius)
    {
        if (radius < minimalRadius)
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
        hitMarkerHideTimestamp = Time.time + hitMarkerDisplayTime;
    }

    public void OnEnemyDeath()
    {
        hitMarkerImage.color = Color.red;
        hitMarkerImage.CrossFadeAlpha(1, 0, true);
        hitMarkerHideTimestamp = Time.time + hitMarkerDisplayTime;
    }

    public void OnPlayerHit(DamageInfo info)
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 towardsHitPoint = -info.direction;
        float angle = Vector3.Angle(cameraTransform.forward, towardsHitPoint);
        if (!damageMarkerShowWhenForward && angle < damageMarkerMaxForwardAngle)
        {
            return;
        }
        if (damageMarkerFadeOut != null)
        {
            StopCoroutine(damageMarkerFadeOut);
        }
        Vector2 screenPosition = new Vector2(Vector3.Dot(towardsHitPoint, cameraTransform.right), Vector3.Dot(towardsHitPoint, cameraTransform.up));
        damageMarker.SetActive(true);
        UnityEngine.UI.Image damageMarkerImage = damageMarker.GetComponent<UnityEngine.UI.Image>();
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenPosition.y, screenPosition.x) * Mathf.Rad2Deg + 90);
        damageMarkerImage.rectTransform.rotation = rotation;
        damageMarkerImage.rectTransform.anchoredPosition = rotation * Vector3.down * damageMarkerDistanceFromCenter;
        damageMarkerImage.CrossFadeAlpha(1, 0, true);
        damageMarkerFadeOut = StartCoroutine(HideDamageMarker(damageMarkerDisplayTime, damageMarkerImage));
    }

    public void OnWeaponChange(WeaponController newWeapon)
    {
        weapon = newWeapon;
    }

    private IEnumerator HideDamageMarker(float time, UnityEngine.UI.Image marker)
    {
        yield return new WaitForSecondsRealtime(time);
        marker.CrossFadeAlpha(0, damageMarkerFadeTime, true);
        damageMarker.SetActive(false);
    }
}
