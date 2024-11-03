using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static MeshGenerator Instance { get; private set; }

    public ComputeShader marchingCubesShader;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

    private int numPoints;
    private int numVoxelsPerAxis;
    private int numVoxels;
    private int maxTriangleCount;
    private int numThreadsPerAxis;
    private int boundsSize;
    private float pointSpacing;
    private Vector3 worldBounds;

    const int threadGroupSize = 8;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        World.Instance.onWorldDataLoaded += OnWorldDataLoaded;
    }

    void OnDisable()
    {
        World.Instance.onWorldDataLoaded -= OnWorldDataLoaded;
    }

    void OnDestroy()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            triangleBuffer = null;
        }
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
            pointsBuffer = null;
        }
        if (triCountBuffer != null)
        {
            triCountBuffer.Release();
            triCountBuffer = null;
        }
    }

    public Mesh GenerateMesh(ComputeBuffer pointsBuffer, float isoLevel)
    {
        triangleBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(0, "points", pointsBuffer);
        marchingCubesShader.SetBuffer(0, "triangles", triangleBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", World.NumPointsPerAxis);
        marchingCubesShader.SetFloat("isoLevel", isoLevel);

        marchingCubesShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        Mesh mesh = new Mesh();

        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        return mesh;
    }

    private void CalculateConstants()
    {
        int numPointsPerAxis = World.NumPointsPerAxis;

        boundsSize = World.ChunkSize;

        numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        numVoxelsPerAxis = numPointsPerAxis - 1;
        numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        maxTriangleCount = numVoxels * 5;
        numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float) threadGroupSize);

        pointSpacing = boundsSize / (numPointsPerAxis - 1);

        worldBounds = new Vector3(World.RenderDistanceHorizontal, World.RenderDistanceVertical, World.RenderDistanceHorizontal) * boundsSize;
    }

    private void CreateBuffers()
    {
        if (!Application.isPlaying || pointsBuffer == null || numPoints != pointsBuffer.count) {
            if (Application.isPlaying) {
                ReleaseBuffers ();
            }
            triangleBuffer = new ComputeBuffer (maxTriangleCount, sizeof (float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer (numPoints, sizeof (float) * 4);
            triCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);
        }
    }

    private void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            pointsBuffer.Release();
            triCountBuffer.Release();
        }
    }

    private void OnWorldDataLoaded(WorldData worldData)
    {
        CalculateConstants();
        CreateBuffers();
    }

    struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning

        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this [int i] {
            get {
                switch (i) {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
}
