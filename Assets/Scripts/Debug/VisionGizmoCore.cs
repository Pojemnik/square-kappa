using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionGizmoCore
{
    public bool drawWhenSelectedOnly;
    public Color selectedColor;
    public Color notSelectedColor;

    public int Points
    {
        get => points;
        set
        {
            points = value;
            sphere.Points = points;
            cone.BasePoints = points;
        }
    }
    private int points;

    public float SphereRadius
    {
        get => sphereRadius;
        set
        {
            sphereRadius = value;
            sphere.Radius = sphereRadius;
        }
    }
    private float sphereRadius;

    public float ConeHeight
    {
        get => coneHeight;
        set
        {
            coneHeight = value;
            cone.Height = coneHeight;
        }
    }
    private float coneHeight;

    public float ConeAngle
    {
        get => coneAngle;
        set
        {
            coneAngle = value;
            cone.Angle = coneAngle;
        }
    }
    private float coneAngle;

    private SphereGizmoCore sphere;
    private ConeGizmoCore cone;

    public VisionGizmoCore()
    {
        sphere = new SphereGizmoCore();
        cone = new ConeGizmoCore();
    }

    public void Draw(bool selected, Vector3 position, Vector3 forward)
    {
        if (!selected && drawWhenSelectedOnly)
        {
            return;
        }
        if (sphere == null)
        {
            sphere = new SphereGizmoCore();
        }
        if (cone == null)
        {
            cone = new ConeGizmoCore();
        }
        cone.Draw(selected, position, forward);
        sphere.Draw(selected, position);
    }
}
