using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIPathNode : MonoBehaviour
{
    public float epsilonRadius;
    public AIPathNode next;

    [Header("Gizmo properities")]
    [SerializeField]
    private bool drawWhenSelectedOnly;
    [SerializeField]
    private int points;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notSelectedColor;
    [SerializeField]
    private Color pathColor;

    private List<Vector3>[] circles;
    private float lastRadius = 0;

    private List<Vector2> GenerateCircle(float radius, int points)
    {
        List<Vector2> circle = new List<Vector2>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            circle.Add(new Vector2(radius * Mathf.Sin(i), radius * Mathf.Cos(i)));
        }
        return circle;
    }

    private void DrawGizmo(Color color)
    {
        Gizmos.color = color;
        foreach(List<Vector3> circle in circles)
        {
            DrawCircle(circle);
        }
        Gizmos.color = pathColor;
        if (next != null)
        {
            Gizmos.DrawLine(transform.position, next.transform.position);
            Vector3 lineCenter = transform.position + next.transform.position / 2;
            Vector3 towardsNext = next.transform.position - transform.position;
        }
    }

    private void DrawCircle(List<Vector3> circle)
    {
        int i = 0;
        for (int j = 1; j < circle.Count; j++, i++)
        {
            Vector3 PointI = circle[i] + transform.position;
            Vector3 PointJ = circle[j] + transform.position;
            Gizmos.DrawLine(PointI, PointJ);
        }
        Vector3 FistPoint = circle[0] + transform.position;
        Vector3 LastPoint = circle[circle.Count - 1] + transform.position;
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
        return new List<Vector3>();
    }

    private void GenerateCircles()
    {
        circles = new List<Vector3>[3];
        List<Vector2> circle = GenerateCircle(epsilonRadius, points);
        circles[0] = RotateCircle(circle, CircleAxis.xy);
        circles[1] = RotateCircle(circle, CircleAxis.xz);
        circles[2] = RotateCircle(circle, CircleAxis.yz);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if(circles == null || lastRadius != epsilonRadius)
        {
            GenerateCircles();
            lastRadius = epsilonRadius;
        }
        if (drawWhenSelectedOnly)
        {
            DrawGizmo(selectedColor);
        }
    }

    private void OnDrawGizmos()
    {
        if (circles == null || lastRadius != epsilonRadius)
        {
            GenerateCircles();
            lastRadius = epsilonRadius;
        }
        if (!drawWhenSelectedOnly)
        {
            DrawGizmo(notSelectedColor);
        }
    }
#endif
}