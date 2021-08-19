using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereGizmo))]
public class AIPathNode : MonoBehaviour
{
    public AIPathNode next;

    [SerializeField]
    private Color pathColor;

    private void DrawGizmo()
    {
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
        DrawGizmo();
    }

    private void OnDrawGizmos()
    {
        DrawGizmo();
    }
#endif
}