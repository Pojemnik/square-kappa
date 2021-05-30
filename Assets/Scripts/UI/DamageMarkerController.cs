using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkerController : MonoBehaviour
{
    public GameObject crosshair;
    public float offsetFromCrosshair;

    private UnityEngine.UI.Image image;

    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
