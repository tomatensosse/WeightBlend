using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int position;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(World.ChunkSize, World.ChunkSize, World.ChunkSize));
    }

    public void Initialize()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        meshRenderer.material = World.Instance.material;
    }

    public void SetDensity(ComputeBuffer densityBuffer)
    {
        int isoLevel = 1;

        Mesh mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, isoLevel);

        this.meshFilter.mesh = mesh;
        this.meshFilter.sharedMesh = mesh;

        this.meshCollider.sharedMesh = mesh;
    }
}
