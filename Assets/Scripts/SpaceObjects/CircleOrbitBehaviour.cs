using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CircleOrbitBehaviour : MonoBehaviour
{
    private struct LocalDirections
    {
        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;
    }

    [Header("Orbit parameters")]
    [Tooltip("Radius of orbit in units")]
    [SerializeField]
    private float radius;
    [Tooltip("Time to complete one orbit")]
    [SerializeField]
    private float orbitalPeriod;

    [Header("Behaviour on orbit")]
    [SerializeField]
    private Vector3 startRotation;
    [SerializeField]
    private Vector3 startAngularSpeed;

    [Header("Gizmo parameters")]
    [Tooltip("Number of points connected by line which create orbit gizmo - it doesn't influence orbit itself")]
    [SerializeField]
    private int points;
    [Tooltip("When on, orbit gizmo is drawn only when object is selected in insepector")]
    [SerializeField]
    private bool drawWhenSelectedOnly;
    [SerializeField]
    private Color selectedColor = Color.yellow;
    [SerializeField]
    private Color notSelectedColor = Color.green;

    private List<Vector2> orbitPath = null;
    private new Rigidbody rigidbody;
    private Vector3 startPosition;
    private Vector3 center;
    private bool running = false;
    private float linearVelocityValue;
    private const float directionGizmoLength = 5;
    private LocalDirections localDirections;

    private List<Vector2> GenerateCircle(float radius, int points)
    {
        List<Vector2> circle = new List<Vector2>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            circle.Add(new Vector2(radius * Mathf.Sin(i), radius * Mathf.Cos(i)));
        }
        return circle;
    }

    private void DrawCircularOrbitGizmo(Vector3 orbitCenter, LocalDirections direction, Color color)
    {
        Gizmos.color = color;
        int i = 0;
        for (int j = 1; j < orbitPath.Count; j++, i++)
        {
            Vector3 globalPointi = orbitPath[i].x * direction.forward + orbitPath[i].y * direction.right + orbitCenter;
            Vector3 globalPointj = orbitPath[j].x * direction.forward + orbitPath[j].y * direction.right + orbitCenter;
            Gizmos.DrawLine(globalPointi, globalPointj);
        }
        Vector3 globalPointFirst = orbitPath[0].x * direction.forward + orbitPath[0].y * direction.right + orbitCenter;
        Vector3 globalPointLast = orbitPath[orbitPath.Count - 1].x * direction.forward + orbitPath[orbitPath.Count - 1].y * direction.right + orbitCenter;
        Gizmos.DrawLine(globalPointFirst, globalPointLast);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (drawWhenSelectedOnly)
        {
            DrawGizmo(selectedColor);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawWhenSelectedOnly)
        {
            DrawGizmo(notSelectedColor);
        }
    }
#endif

    private void DrawGizmo(Color pathColor)
    {
        if (running)
        {
            DrawCircularOrbitGizmo(center, localDirections, pathColor);
        }
        else
        {
            LocalDirections direction;
            direction.forward = transform.forward;
            direction.right = transform.right;
            direction.up = transform.up;
            if (orbitPath == null || orbitPath.Count == 0 || orbitPath.Count != points)
            {
                orbitPath = GenerateCircle(radius, points);
            }
            DrawCircularOrbitGizmo(transform.position, direction, pathColor);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + direction.forward * radius);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + direction.forward * radius, transform.position + direction.forward * radius + direction.right * directionGizmoLength);
        }
    }


    private void Awake()
    {
        orbitPath = GenerateCircle(radius, points);
        center = transform.position;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        startPosition = transform.position + transform.forward * radius;
    }

    private void Start()
    {
        localDirections.forward = transform.forward;
        localDirections.right = transform.right;
        localDirections.up = transform.up;
        rigidbody.MovePosition(startPosition);
        running = true;
    }

    private void FixedUpdate()
    {
        if (rigidbody.isKinematic)
        {
            if (Mathf.Abs((transform.position - startPosition).sqrMagnitude) < 0.1) //that's pretty big epsilon
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = false;
                rigidbody.AddForce(transform.right * 2 * Mathf.PI * radius / orbitalPeriod, ForceMode.VelocityChange);
                rigidbody.rotation = Quaternion.Euler(startRotation);
                rigidbody.AddRelativeTorque(startAngularSpeed, ForceMode.VelocityChange);
                linearVelocityValue = 2 * Mathf.PI * radius / orbitalPeriod;
            }
        }
        else
        {
            Vector3 towardsCenter = (center - transform.position).normalized;
            rigidbody.AddForce(towardsCenter * rigidbody.mass * linearVelocityValue * linearVelocityValue / radius);
        }
    }
}
