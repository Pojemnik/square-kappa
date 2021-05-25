using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public float hitMarkerDisplayTime;
    public GameObject hitMarker;

    private int hits;

    void Start()
    {
        hits = 0;
    }

    public void OnEnemyHit()
    {
        hitMarker.SetActive(true);
        hits++;
        StartCoroutine(HideHitMarker(hitMarkerDisplayTime));
    }

    private IEnumerator HideHitMarker(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        hits--;
        if (hits == 0)
        {
            hitMarker.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
