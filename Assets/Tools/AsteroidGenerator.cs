using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AsteroidGenerator : MonoBehaviour
{
    public int meshSize;

    public float noiseMultipler;
    public float baseHeight;
    public Vector3 offset;
    public float positionScale;

    public enum CraterOverlapMode
    {
        Multiply,
        Smooth,
        None
    }

    public CraterOverlapMode craterOverlap;

    public enum NoiseType
    {
        Perlin,
        Simplex,
        Cellular3,
        Cellular2
    }

    public NoiseType noiseType;

    private MeshFilter filter;
    private Mesh mesh;

    private Dictionary<NoiseType, System.Func<Vector3, float>> noiseFunctions;

    private void GetNoiseFunctions()
    {
        noiseFunctions = new Dictionary<NoiseType, System.Func<Vector3, float>>
        {
            [NoiseType.Perlin] = (pos) => noise.cnoise(pos),
            [NoiseType.Simplex] = (pos) => noise.snoise(pos),
            [NoiseType.Cellular3] = (pos) => noise.cellular(pos).y,
            [NoiseType.Cellular2] = (pos) => noise.cellular2x2x2(pos).x
        };
    }

    public void Generate()
    {
        if(noiseFunctions == null)
        {
            GetNoiseFunctions();
        }
        filter = GetComponent<MeshFilter>();
        mesh = filter.sharedMesh = new Mesh();
        Icosphere.Create(ref mesh, meshSize);
        ApplyChanges();
        RecalculateMesh();
    }

    private void ApplyChanges()
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            float finalHeight = 1;
            Vector3 pos = vertices[i] * positionScale + offset;
            finalHeight = Mathf.Clamp01(noiseFunctions[noiseType](pos) / 2 + 0.5f) * noiseMultipler + baseHeight;
            vertices[i] *= finalHeight;
        }
        mesh.vertices = vertices;
    }

    private void RecalculateMesh()
    {
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }
}
