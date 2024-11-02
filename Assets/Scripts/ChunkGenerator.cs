using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }

    public delegate void OnNewChunksGenerated(List<Chunk> newChunks);
    public OnNewChunksGenerated onNewChunksGenerated;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        foreach (Player player in World.Instance.players)
        {
            GenerateChunks(player);
        }
    }

    void OnEnable()
    {
        World.Instance.onPlayerLocationChanged += OnPlayerLocationChanged;
    }

    void OnDisable()
    {
        World.Instance.onPlayerLocationChanged -= OnPlayerLocationChanged;
    }

    private void OnPlayerLocationChanged(Player player)
    {
        GenerateChunks(player);

        FindAndDeleteDirtyChunks(player);
    }

    private void GenerateChunks(Player player)
    {
        List<Chunk> newChunks = new List<Chunk>();

        Vector3Int playerChunkPosition = player.ChunkPosition();

        for (int x = playerChunkPosition.x - World.RenderDistanceHorizontal; x <= playerChunkPosition.x + World.RenderDistanceHorizontal; x++)
        {
            for (int y = playerChunkPosition.y - World.RenderDistanceVertical; y <= playerChunkPosition.y + World.RenderDistanceVertical; y++)
            {
                for (int z = playerChunkPosition.z - World.RenderDistanceHorizontal; z <= playerChunkPosition.z + World.RenderDistanceHorizontal; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x, y, z);

                    if (!ChunkHolder.Instance.IsChunkLoaded(chunkPosition))
                    {
                        Chunk chunk = GenerateChunk(new Vector3Int(x, y, z));

                        newChunks.Add(chunk);
                    }
                }
            }
        }

        if (newChunks.Count > 0)
        {
            onNewChunksGenerated.Invoke(newChunks);
        }
    }

    private Chunk GenerateChunk(Vector3Int position)
    {
        GameObject chunkGameObject = new GameObject("Chunk " + position);
        chunkGameObject.transform.SetParent(ChunkHolder.Instance.transform);

        chunkGameObject.transform.position = new Vector3(position.x * World.ChunkSize, position.y * World.ChunkSize, position.z * World.ChunkSize);

        Chunk chunk = chunkGameObject.AddComponent<Chunk>();

        chunk.position = position;
        chunk.Initialize();

        ChunkHolder.Instance.chunks.Add(chunk);

        return chunk;
    }

    private void FindAndDeleteDirtyChunks(Player player)
    {
        List<Chunk> dirtyChunks = new List<Chunk>();

        foreach (Chunk chunk in ChunkHolder.Instance.chunks)
        {
            if (!World.Instance.ChunkInRange(chunk.position, player.ChunkPosition()))
            {
                dirtyChunks.Add(chunk);
            }
        }

        for (int i = 0; i < dirtyChunks.Count; i++)
        {
            Chunk chunk = dirtyChunks[i];

            ChunkHolder.Instance.chunks.Remove(chunk);
            Destroy(chunk.gameObject);
        }
    }
}
