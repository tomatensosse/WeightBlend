using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityGenerator : MonoBehaviour
{
    public static DensityGenerator Instance { get; private set; }

    public ComputeShader densityShader;

    List<ComputeBuffer> buffersToRelease;

    public int seed;
    public int numPointsPerAxis = 32;
    public int numOctaves = 4;
    public float lacunarity = 2;
    public float persistence = .5f;
    public float noiseScale = .2f;
    public float noiseWeight = 16;
    public bool closeEdges;
    public float floorOffset = 8;
    public float weightMultiplier = 4;

    public float hardFloorHeight;
    public float hardFloorWeight;

    public Vector4 shaderParams = new Vector4(1, 0, 0, 0);

    const int threadGroupSize = 8;

    private Vector3 centre;
    private Vector3 offset;
    private float boundsSize;
    private float spacing;
    private Vector3 worldBounds;

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
        ChunkGenerator.Instance.onNewChunksGenerated += OnNewChunksGenerated;

        GenerateConstants();
    }

    void OnDisable()
    {
        ChunkGenerator.Instance.onNewChunksGenerated -= OnNewChunksGenerated;
    }

    void GenerateConstants()
    {
        boundsSize = World.ChunkSize;
        offset = new Vector3(0, 0, 0);
        spacing = boundsSize / (numPointsPerAxis - 1);
        worldBounds = new Vector3(World.RenderDistanceHorizontal, World.RenderDistanceVertical, World.RenderDistanceHorizontal) * World.ChunkSize;
    }

    private void OnNewChunksGenerated(List<Chunk> newChunks)
    {
        foreach (Chunk chunk in newChunks)
        {
            GenerateDensity(chunk);
        }
    }

    private void GenerateDensity(Chunk chunk)
    {
        ComputeBuffer pointsBuffer = new ComputeBuffer(numPointsPerAxis * numPointsPerAxis * numPointsPerAxis, sizeof(float) * 4);;

        buffersToRelease = new List<ComputeBuffer>();

        // Noise params
        var prng = new System.Random (seed);
        var offsets = new Vector3[numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < numOctaves; i++) {
            offsets[i] = new Vector3 ((float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1) * offsetRange;
        }

        centre = chunk.transform.position;

        int numThreadsPerAxis = Mathf.CeilToInt (numPointsPerAxis / (float) threadGroupSize);
        // Points buffer is populated inside shader with pos (xyz) + density (w).

        var offsetsBuffer = new ComputeBuffer (offsets.Length, sizeof (float) * 3);
        offsetsBuffer.SetData (offsets);
        buffersToRelease.Add (offsetsBuffer);

        densityShader.SetVector ("centre", new Vector4 (centre.x, centre.y, centre.z));
        densityShader.SetInt ("octaves", Mathf.Max (1, numOctaves));
        densityShader.SetFloat ("lacunarity", lacunarity);
        densityShader.SetFloat ("persistence", persistence);
        densityShader.SetFloat ("noiseScale", noiseScale);
        densityShader.SetFloat ("noiseWeight", noiseWeight);
        densityShader.SetBool ("closeEdges", closeEdges);
        densityShader.SetBuffer (0, "offsets", offsetsBuffer);
        densityShader.SetFloat ("floorOffset", floorOffset);
        densityShader.SetFloat ("weightMultiplier", weightMultiplier);
        densityShader.SetFloat ("hardFloor", hardFloorHeight);
        densityShader.SetFloat ("hardFloorWeight", hardFloorWeight);

        // Set paramaters
        densityShader.SetBuffer (0, "points", pointsBuffer);
        densityShader.SetInt ("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat ("boundsSize", boundsSize);
        densityShader.SetVector ("centre", new Vector4 (centre.x, centre.y, centre.z));
        densityShader.SetVector ("offset", new Vector4 (offset.x, offset.y, offset.z));
        densityShader.SetFloat ("spacing", spacing);
        densityShader.SetVector("worldSize", worldBounds);

        densityShader.SetVector ("params", shaderParams);

        // Dispatch shader
        densityShader.Dispatch (0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        if (buffersToRelease != null) {
            foreach (var b in buffersToRelease) {
                b.Release();
            }
        }

        chunk.SetDensity(pointsBuffer);

        pointsBuffer.Release();
    }
}
