using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TBA
    }

    // Update is called once per frame
    void Update()
    {
        // TBA
    }

    public Vector3Int ChunkPosition()
    {
        return World.Instance.WorldToChunkPosition(transform.position);
    }
}
