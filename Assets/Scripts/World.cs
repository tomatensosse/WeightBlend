using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    public static int RenderDistanceHorizontal { get { return Instance.renderDistanceHorizontal; } }
    public static int RenderDistanceVertical { get { return Instance.renderDistanceVertical; } }
    public static int ChunkSize { get { return Instance.chunkSize; } }
    public static int NumPointsPerAxis { get { return Instance.numPointsPerAxis; } }
    public static List<Player> Players { get { return Instance.players; } }

    public int renderDistanceHorizontal = 3;
    public int renderDistanceVertical = 2;
    public int chunkSize = 64;
    public int numPointsPerAxis = 32;
    public List<Player> players;

    public Dictionary<Player, Vector3Int> playerPositions = new Dictionary<Player, Vector3Int>();

    public Material material;

    public delegate void OnPlayerLocationChanged(Player player);
    public OnPlayerLocationChanged onPlayerLocationChanged;

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
        Player[] find = GameObject.FindObjectsOfType<Player>();

        players = find.ToList();

        foreach (Player player in players)
        {
            playerPositions.Add(player, WorldToChunkPosition(player.transform.position));
        }
    }

    void Update()
    {
        foreach (Player player in players)
        {
            Vector3Int playerChunkPosition = WorldToChunkPosition(player.transform.position);

            if (playerPositions[player] != playerChunkPosition)
            {
                playerPositions[player] = playerChunkPosition;

                onPlayerLocationChanged.Invoke(player);
            }
        }
    }

    public Vector3Int WorldToChunkPosition(Vector3 position)
    {
        int offset = World.ChunkSize / 2;

        return new Vector3Int(
            Mathf.FloorToInt((position.x + offset) / ChunkSize),
            Mathf.FloorToInt((position.y + offset) / ChunkSize),
            Mathf.FloorToInt((position.z + offset) / ChunkSize)
        );
    }

    public bool ChunkInRange(Vector3Int chunkPosition, Vector3Int playerChunkPosition)
    {
        return
            chunkPosition.x >= playerChunkPosition.x - RenderDistanceHorizontal &&
            chunkPosition.x <= playerChunkPosition.x + RenderDistanceHorizontal &&
            chunkPosition.y >= playerChunkPosition.y - RenderDistanceVertical &&
            chunkPosition.y <= playerChunkPosition.y + RenderDistanceVertical &&
            chunkPosition.z >= playerChunkPosition.z - RenderDistanceHorizontal &&
            chunkPosition.z <= playerChunkPosition.z + RenderDistanceHorizontal;
    }
}
