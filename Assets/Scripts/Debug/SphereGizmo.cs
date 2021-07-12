using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SphereGizmo : MonoBehaviour
{
    [Header("Gizmo properities")]
    public float radius;
    [SerializeField]
    [Min(1)]
    private int points;

    [SerializeField]
    private bool drawWhenSelectedOnly;

    [Header("Colors")]
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notSelectedColor;

    private SphereGizmoCore gizmo;

    private void UpdateCoreValues()
    {
        gizmo.Radius = radius;
        gizmo.Points = points;
        gizmo.drawWhenSelectedOnly = drawWhenSelectedOnly;
        gizmo.selectedColor = selectedColor;
        gizmo.notSelectedColor = notSelectedColor;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gizmo == null)
        {
            gizmo = new SphereGizmoCore();
        }
        UpdateCoreValues();
    }

    void OnDrawGizmosSelected()
    {
        if (gizmo == null)
        {
            gizmo = new SphereGizmoCore();
            UpdateCoreValues();
        }
        gizmo.Draw(true, transform.position);
    }

    private void OnDrawGizmos()
    {
        if (gizmo == null)
        {
            gizmo = new SphereGizmoCore();
            UpdateCoreValues();
        }
        gizmo.Draw(false, transform.position);
    }
#endif
}

public class SphereGizmoCore
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
            GenerateCircles();
        }
    }
    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            GenerateCircles();
        }
    }

    private float radius;
    private int points;
    private List<Vector3>[] circles;

    public void Draw(bool selected, Vector3 position)
    {
        if (!selected && drawWhenSelectedOnly)
        {
            return;
        }
        if (circles == null)
        {
            GenerateCircles();
        }
        Color lastGizmoColor = Gizmos.color;
        if (selected)
        {
            Gizmos.color = selectedColor;
        }
        else
        {
            Gizmos.color = notSelectedColor;
        }
        foreach (List<Vector3> circle in circles)
        {
            DrawCircle(circle, position);
        }
        Gizmos.color = lastGizmoColor;
    }

    private List<Vector2> GenerateCircle(float radius, int points)
    {
        List<Vector2> circle = new List<Vector2>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            circle.Add(new Vector2(radius * Mathf.Sin(i), radius * Mathf.Cos(i)));
        }
        return circle;
    }

    private void DrawCircle(List<Vector3> circle, Vector3 position)
    {
        int i = 0;
        for (int j = 1; j < circle.Count; j++, i++)
        {
            Vector3 PointI = circle[i] + position;
            Vector3 PointJ = circle[j] + position;
            Gizmos.DrawLine(PointI, PointJ);
        }
        Vector3 FistPoint = circle[0] + position;
        Vector3 LastPoint = circle[circle.Count - 1] + position;
        Gizmos.DrawLine(FistPoint, LastPoint);
    }

    private enum CircleAxis
    {
        xy, xz, yz
    }

    private List<Vector3> RotateCircle(List<Vector2> circle, CircleAxis axis)
    {
        switch (axis)
        {
            case CircleAxis.xy:
                return circle.Select(e => (Vector3)e).ToList();
            case CircleAxis.xz:
                return circle.Select(e => new Vector3(e.x, 0, e.y)).ToList();
            case CircleAxis.yz:
                return circle.Select(e => new Vector3(0, e.x, e.y)).ToList();
        }
        //This should never happen
        throw new System.Exception(string.Format("Incorrect CircleAxis enum value: {0}", axis));
    }

    private void GenerateCircles()
    {
        circles = new List<Vector3>[3];
        List<Vector2> circle = GenerateCircle(Radius, Points);
        circles[0] = RotateCircle(circle, CircleAxis.xy);
        circles[1] = RotateCircle(circle, CircleAxis.xz);
        circles[2] = RotateCircle(circle, CircleAxis.yz);
    }
}