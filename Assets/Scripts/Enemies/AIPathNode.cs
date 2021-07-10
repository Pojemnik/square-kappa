using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereGizmo))]
public class AIPathNode : MonoBehaviour
{
    public AIPathNode next;

    [SerializeField]
    private SphereGizmo sphere;
    [SerializeField]
    private Color pathColor;

    private void DrawGizmo(bool selected)
    {
        sphere.DrawSphere(selected);
        Gizmos.color = pathColor;
        if (next != null)
        {
            Gizmos.DrawLine(transform.position, next.transform.position);
            //Maybe draw some arrow
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }

    private void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
#endif
}