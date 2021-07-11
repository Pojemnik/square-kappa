using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConeGizmo : MonoBehaviour
{
    public float height;
    public float angle;
    public GameObject eyes;
    [Space]

    [SerializeField]
    [ReadOnly]
    private float baseRadius;

    [Header("Drawing")]
    [SerializeField]
    private int basePoints;
    [SerializeField]
    private int coneLines;
    [SerializeField]
    private bool drawWhenSelectedOnly;

    [Header("Colors")]
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notSelectedColor;

    private List<Vector3> circle;
    private float lastHeight = 0;
    private float lastAngle = 0;

    public void DrawGizmo(bool selected)
    {
        if (!selected && drawWhenSelectedOnly)
        {
            return;
        }
        if (circle == null || lastHeight != height || lastAngle != angle)
        {
            baseRadius = 2 * height * Mathf.Tan(angle * Mathf.Deg2Rad / 2);
            circle = GenerateCircle(baseRadius, basePoints);
            lastHeight = height;
            lastAngle = angle;
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
        DrawBase(circle);
        DrawLines();
        Gizmos.color = lastGizmoColor;
    }

    private List<Vector3> GenerateCircle(float radius, int points)
    {
        List<Vector2> circle = new List<Vector2>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            circle.Add(new Vector2(radius * Mathf.Sin(i), radius * Mathf.Cos(i)));
        }
        return circle.Select(e => (Vector3)e).ToList();
    }

    private void DrawBase(List<Vector3> circle)
    {
        int i = 0;
        for (int j = 1; j < circle.Count; j++, i++)
        {
            Vector3 PointI = circle[i] + eyes.transform.position + transform.forward * height;
            Vector3 PointJ = circle[j] + eyes.transform.position + transform.forward * height;
            Gizmos.DrawLine(PointI, PointJ);
        }
        Vector3 FistPoint = circle[0] + eyes.transform.position + transform.forward * height;
        Vector3 LastPoint = circle[circle.Count - 1] + eyes.transform.position + transform.forward * height;
        Gizmos.DrawLine(FistPoint, LastPoint);
    }

    private void DrawLines()
    {
        if(coneLines > circle.Count)
        {
            throw new System.Exception(string.Format("Number of cone lines to draw bigger" +
                " than the number of base's points. Gizmo {0}", name));
        }
        int verticesToSkip = circle.Count / coneLines;
        for (int i = verticesToSkip / 2; i < circle.Count; i += verticesToSkip)
        {
            Gizmos.DrawLine(eyes.transform.position, circle[i] + eyes.transform.position + transform.forward * height);
        }
    }
}
