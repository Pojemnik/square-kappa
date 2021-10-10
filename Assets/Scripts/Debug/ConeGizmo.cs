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
            gizmo = new ConeGizmoCore(ConeGizmoCore.LookingDirection.Forward);
        }
        UpdateCoreValues();
    }


    void OnDrawGizmosSelected()
    {
        if (gizmo == null)
        {
            gizmo = new ConeGizmoCore(ConeGizmoCore.LookingDirection.Forward);
            UpdateCoreValues();
        }
        gizmo.Draw(true, eyes.transform);
    }

    private void OnDrawGizmos()
    {
        if (gizmo == null)
        {
            gizmo = new ConeGizmoCore(ConeGizmoCore.LookingDirection.Forward);
            UpdateCoreValues();
        }
        gizmo.Draw(false, eyes.transform);
    }
#endif
}

public class ConeGizmoCore
{
    public float baseRadius;
    public int coneLines;
    public bool drawWhenSelectedOnly;
    public Color selectedColor;
    public Color notSelectedColor;

    private List<Vector3> circle;
    private LookingDirection lookingDirection;

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

    public ConeGizmoCore(LookingDirection direction)
    {
        lookingDirection = direction;
        RecalculateRadiusAndGenerateCircle();
    }

    public void Draw(bool selected, Transform transform)
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
        DrawBase(circle, transform);
        DrawLines(transform);
        Gizmos.color = lastGizmoColor;
    }

    public enum LookingDirection
    {
        Up,
        Forward
    }

    private void RecalculateRadiusAndGenerateCircle()
    {
        baseRadius = 2 * Height * Mathf.Tan(Angle * Mathf.Deg2Rad / 2);
        circle = GenerateCircle(baseRadius, BasePoints);
    }

    private List<Vector3> GenerateCircle(float radius, int points)
    {
        List<Vector3> circle = new List<Vector3>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            if (lookingDirection == LookingDirection.Up)
            {
                circle.Add(new Vector3(radius * Mathf.Sin(i), 0, radius * Mathf.Cos(i)));
            }
            else
            {
                circle.Add(new Vector3(radius * Mathf.Sin(i), radius * Mathf.Cos(i), 0));
            }
        }
        return circle;
    }

    private Vector3 TransformCirclePointIgnoringScale(Vector3 point, Transform transform)
    {
        //Vector3 inverseScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
        //return Vector3.Scale(transform.TransformPoint(point), inverseScale);
        return transform.position + transform.up * point.x + transform.right * point.y;
    }

    private void DrawBase(List<Vector3> circle, Transform transform)
    {
        int i = 0;
        Vector3 direction = transform.forward;
        if (lookingDirection == LookingDirection.Up)
        {
            direction = transform.up;
        }
        for (int j = 1; j < circle.Count; j++, i++)
        {
            Vector3 PointI = TransformCirclePointIgnoringScale(circle[i], transform)+ direction * Height;
            Vector3 PointJ = TransformCirclePointIgnoringScale(circle[j], transform) + direction * Height;
            Gizmos.DrawLine(PointI, PointJ);
        }
        Vector3 FistPoint = TransformCirclePointIgnoringScale(circle[0], transform) + direction * Height;
        Vector3 LastPoint = TransformCirclePointIgnoringScale(circle[circle.Count - 1], transform) + direction * Height;
        Gizmos.DrawLine(FistPoint, LastPoint);
    }

    private void DrawLines(Transform transform)
    {
        if (coneLines > circle.Count)
        {
            throw new System.Exception("Number of cone lines to draw bigger than the number of base's points");
        }
        int verticesToSkip = circle.Count / coneLines;
        Vector3 direction = transform.forward;
        if (lookingDirection == LookingDirection.Up)
        {
            direction = transform.up;
        }
        for (int i = verticesToSkip / 2; i < circle.Count; i += verticesToSkip)
        {
            Gizmos.DrawLine(transform.position, TransformCirclePointIgnoringScale(circle[i], transform) + direction * Height);
        }
    }
}