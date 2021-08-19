using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AsteroidGenerator : MonoBehaviour
{
    public float maxCraterSize;
    public float minCraterSize;
    public float bottomHeight;
    public float hillHeight;
    public float hillRatio;
    public int meshSize;
    public float smoothK;
    public int craters;
    public int seed;

    public enum CraterOverlapMode
    {
        Multiply,
        Smooth,
        None
    }

    public CraterOverlapMode craterOverlap;

    private MeshFilter filter;
    private Mesh mesh;

    public void Generate()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.sharedMesh = new Mesh();
        Icosphere.Create(ref mesh, meshSize);
        ApplyChanges();
        RecalculateMesh();
    }

    private void ApplyChanges()
    {
        Random.InitState(seed);
        Vector3[] cratersPositions = new Vector3[craters];
        float[] cratersSizes = new float[craters];
        for (int i = 0; i < craters; i++)
        {
            cratersPositions[i] = Random.onUnitSphere;
            cratersSizes[i] = Random.Range(minCraterSize, maxCraterSize);
        }
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            float finalHeight = 1;
            for (int j = 0; j < craters; j++)
            {
                float height = 1;
                float dist = (vertices[i] - cratersPositions[j]).magnitude;
                float r = cratersSizes[j];
                if (r < dist)
                {
                    continue;
                }
                float edge = (1 / (r * r)) * dist * dist;
                float bottom = bottomHeight;
                float hillSize = r * hillRatio;
                float b = (bottomHeight - hillHeight + hillSize * hillSize) / hillSize;
                float hill = -dist * dist + b * dist + hillHeight;
                height = SmoothMin(edge, height, smoothK);
                float bottomHill = SmoothMin(bottom, hill, -smoothK);
                height = SmoothMin(bottomHill, height, -smoothK);
                switch (craterOverlap)
                {
                    case CraterOverlapMode.Smooth:
                        finalHeight = SmoothMin(finalHeight, height, smoothK);
                        break;
                    case CraterOverlapMode.Multiply:
                        finalHeight *= height;
                        break;
                    case CraterOverlapMode.None:
                        finalHeight = height;
                        break;
                    default:
                        finalHeight = height;
                        break;
                }

            }
            vertices[i] *= finalHeight;
        }
        mesh.vertices = vertices;
    }

    private float SmoothMin(float a, float b, float k)
    {
        float h = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }

    private Vector2 MapVertex(Vector3 vertex)
    {
        Vector2 result;
        result.y = Mathf.Asin(vertex.y) / Mathf.PI + 0.5F;
        result.x = Mathf.Atan2(vertex.x, vertex.z) / (Mathf.PI * -2);
        if (result.x < 0)
        {
            result.x += 1;
        }
        return result;
    }

    private void RecalculateMesh()
    {
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }
}
