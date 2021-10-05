using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AsteroidGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled;
        public float noiseMultipler;
        public float baseHeight;
        public Vector3 offset;
        public float positionScale;
        public enum NoiseType
        {
            Perlin,
            Simplex,
            Cellular3,
            Cellular2
        }

        public NoiseType noiseType;

        private Dictionary<NoiseType, System.Func<Vector3, float>> noiseFunctions;

        private void GetNoiseFunctions()
        {
            noiseFunctions = new Dictionary<NoiseType, System.Func<Vector3, float>>
            {
                [NoiseType.Perlin] = (pos) => Unity.Mathematics.noise.cnoise(pos),
                [NoiseType.Simplex] = (pos) => Unity.Mathematics.noise.snoise(pos),
                [NoiseType.Cellular3] = (pos) => Unity.Mathematics.noise.cellular(pos).y,
                [NoiseType.Cellular2] = (pos) => Unity.Mathematics.noise.cellular2x2x2(pos).x
            };
        }

        public float GetVertexHeight(Vector3 vertex)
        {
            if(!enabled)
            {
                return 1;
            }
            if(noiseFunctions == null)
            {
                GetNoiseFunctions();
            }
            Vector3 pos = vertex * positionScale + offset;
            return Mathf.Clamp01(noiseFunctions[noiseType](pos) / 2 + 0.5f) * noiseMultipler + baseHeight;
        }
    }
    public int meshSize;

    public List<NoiseLayer> noiseLayers;

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
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            float finalHeight = 1;
            foreach (NoiseLayer layer in noiseLayers)
            {
                finalHeight *= layer.GetVertexHeight(vertices[i]);
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
