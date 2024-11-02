using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHolder : MonoBehaviour
{
    public static ChunkHolder Instance { get; private set; }
    
    public List<Chunk> chunks = new List<Chunk>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool IsChunkLoaded(Vector3Int at)
    {
        foreach (Chunk chunk in chunks)
        {
            if (chunk.position == at)
            {
                return true;
            }
        }

        return false;
    }
}
