using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CircleOrbitBehaviour : MonoBehaviour
{
    [Tooltip("Radius of orbit in units")]
    public float radius;
    [Tooltip("Number of points connected by line which create orbit gizmo - it doesn't influence orbit itself")]
    public int points;
    [Tooltip("Time to complete one orbit")]
    public float orbitalPeriod;
    [Tooltip("Time to complete one orbit")]
    public bool drawWhenSelectedOnly;

    private List<Vector2> orbitPath = null;
    private new Rigidbody rigidbody;
    private Vector3 startPosition;
    private Vector3 center;
    private bool running = false;
    private float linearVelocityValue;
    private const float directionGizmoLength = 5;

    private List<Vector2> GenerateCircle(float radius, int points)
    {
        List<Vector2> circle = new List<Vector2>();
        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 2 / points)
        {
            circle.Add(new Vector2(radius * Mathf.Sin(i), radius * Mathf.Cos(i)));
        }
        return circle;
    }

    private void DrawCircularOrbitGizmo(Vector3 orbitCenter)
    {
        Gizmos.color = Color.green;
        int i = 0;
        for (int j = 1; j < orbitPath.Count; j++, i++)
        {
            Vector3 globalPointi = orbitPath[i].x * transform.forward + orbitPath[i].y * transform.right + orbitCenter;
            Vector3 globalPointj = orbitPath[j].x * transform.forward + orbitPath[j].y * transform.right + orbitCenter;
            Gizmos.DrawLine(globalPointi, globalPointj);
        }
        Vector3 globalPointFirst = orbitPath[0].x * transform.forward + orbitPath[0].y * transform.right + orbitCenter;
        Vector3 globalPointLast = orbitPath[orbitPath.Count - 1].x * transform.forward + orbitPath[orbitPath.Count - 1].y * transform.right + orbitCenter;
        Gizmos.DrawLine(globalPointFirst, globalPointLast);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (drawWhenSelectedOnly)
        {
            DrawGizmo();
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawWhenSelectedOnly)
        {
            DrawGizmo();
        }
    }
#endif

    private void DrawGizmo()
    {
        if (running)
        {
            DrawCircularOrbitGizmo(center);
        }
        else
        {
            if (orbitPath == null || orbitPath.Count == 0 || orbitPath.Count != points)
            {
                orbitPath = GenerateCircle(radius, points);
            }
            DrawCircularOrbitGizmo(transform.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.forward * radius, transform.position + transform.forward * radius + transform.right * directionGizmoLength);
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
