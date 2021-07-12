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
    [Min(1)]
    private int basePoints;
    [SerializeField]
    [Min(1)]
    private int coneLines;
    [SerializeField]
    private bool drawWhenSelectedOnly;

    [Header("Colors")]
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notSelectedColor;

    private ConeGizmoCore gizmo;

    private void UpdateCoreValues()
    {
        gizmo.Height = height;
        gizmo.Angle = angle;
        gizmo.eyes = eyes;
        gizmo.BasePoints = basePoints;
        gizmo.coneLines = coneLines;
        gizmo.drawWhenSelectedOnly = drawWhenSelectedOnly;
        gizmo.selectedColor = selectedColor;
        gizmo.notSelectedColor = notSelectedColor;
        baseRadius = gizmo.baseRadius;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gizmo == null)
        {
            gizmo = new ConeGizmoCore();
            UpdateCoreValues();
        }
    }

    void OnDrawGizmosSelected()
    {
        if(gizmo == null)
        {
            gizmo = new ConeGizmoCore();
            UpdateCoreValues();
        }
        gizmo.Draw(true);
    }

    private void OnDrawGizmos()
    {
        if (gizmo == null)
        {
            gizmo = new ConeGizmoCore();
            UpdateCoreValues();
        }
        gizmo.Draw(false);
    }
#endif
}

public class ConeGizmoCore
{
    public GameObject eyes;
    public float baseRadius;
    public int coneLines;
    public bool drawWhenSelectedOnly;
    public Color selectedColor;
    public Color notSelectedColor;

    private List<Vector3> circle;

    public float Height
    {
        get => height; 
        set
        {
            height = value;
            RecalculateRadiusAndGenerateCircle();
        }
    }
    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
            RecalculateRadiusAndGenerateCircle();
        }
    }

    public int BasePoints
    {
        get => basePoints;
        set
        {
            basePoints = value;
            RecalculateRadiusAndGenerateCircle();
        }
    }

    private float height;
    private float angle;
    private int basePoints;

    public void Draw(bool selected)
    {
        if (!selected && drawWhenSelectedOnly)
        {
            return;
        }
        if (circle == null)
        {
            RecalculateRadiusAndGenerateCircle();
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

    private void RecalculateRadiusAndGenerateCircle()
    {
        baseRadius = 2 * Height * Mathf.Tan(Angle * Mathf.Deg2Rad / 2);
        circle = GenerateCircle(baseRadius, BasePoints);
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
            Vector3 PointI = circle[i] + eyes.transform.position + eyes.transform.forward * Height;
            Vector3 PointJ = circle[j] + eyes.transform.position + eyes.transform.forward * Height;
            Gizmos.DrawLine(PointI, PointJ);
        }
        Vector3 FistPoint = circle[0] + eyes.transform.position + eyes.transform.forward * Height;
        Vector3 LastPoint = circle[circle.Count - 1] + eyes.transform.position + eyes.transform.forward * Height;
        Gizmos.DrawLine(FistPoint, LastPoint);
    }

    private void DrawLines()
    {
        if (coneLines > circle.Count)
        {
            throw new System.Exception("Number of cone lines to draw bigger than the number of base's points");
        }
        int verticesToSkip = circle.Count / coneLines;
        for (int i = verticesToSkip / 2; i < circle.Count; i += verticesToSkip)
        {
            Gizmos.DrawLine(eyes.transform.position, circle[i] + eyes.transform.position + eyes.transform.forward * Height);
        }
    }
}