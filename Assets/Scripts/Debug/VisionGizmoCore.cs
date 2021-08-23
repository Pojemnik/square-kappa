using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionGizmoCore
{
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

    public int ConeLines
    {
        get => coneLines;
        set
        {
            coneLines = value;
            cone.coneLines = coneLines;
        }
    }

    public bool DrawWhenSelectedOnly
    {
        get => drawWhenSelectedOnly; 
        set
        {
            drawWhenSelectedOnly = value;
            cone.drawWhenSelectedOnly = drawWhenSelectedOnly;
            sphere.drawWhenSelectedOnly = drawWhenSelectedOnly;
        }
    }
    private bool drawWhenSelectedOnly;
    public Color SelectedColor
    {
        get => selectedColor; 
        set
        {
            selectedColor = value;
            cone.selectedColor = selectedColor;
            sphere.selectedColor = selectedColor;
        }
    }
    private Color selectedColor;

    public Color NotSelectedColor
    {
        get => notSelectedColor; 
        set
        {
            notSelectedColor = value;
            cone.notSelectedColor = notSelectedColor;
            sphere.notSelectedColor = notSelectedColor;
        }
    }
    private Color notSelectedColor;

    public float ConeBaseRadius
    {
        get => cone.baseRadius;
    }


    private int coneLines;

    private SphereGizmoCore sphere;
    private ConeGizmoCore cone;

    public VisionGizmoCore()
    {
        sphere = new SphereGizmoCore();
        cone = new ConeGizmoCore(ConeGizmoCore.LookingDirection.Forward);
    }

    public void Draw(bool selected, Transform transform)
    {
        if (!selected && DrawWhenSelectedOnly)
        {
            return;
        }
        if (sphere == null)
        {
            sphere = new SphereGizmoCore();
        }
        if (cone == null)
        {
            cone = new ConeGizmoCore(ConeGizmoCore.LookingDirection.Up);
        }
        cone.Draw(selected, transform);
        sphere.Draw(selected, transform.position);
    }
}
