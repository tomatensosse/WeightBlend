using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    public static WorldData WorldData { get { return Instance.worldData; } }
    public static int RenderDistanceHorizontal { get { return Instance.renderDistanceHorizontal; } }
    public static int RenderDistanceVertical { get { return Instance.renderDistanceVertical; } }
    public static int Seed { get { return Instance.worldData.seed; } }
    public static int ChunkSize { get { return Instance.worldData.chunkSize; } }
    public static int NumPointsPerAxis { get { return Instance.worldData.numPointsPerAxis; } }
    public static List<Player> Players { get { return Instance.players; } }
    
    private int renderDistanceHorizontal = 3;
    private int renderDistanceVertical = 2;
    private WorldData worldData;
    private List<Player> players = new List<Player>();

    public Dictionary<Player, Vector3Int> playerPositions = new Dictionary<Player, Vector3Int>();

    public Material material;

    public delegate void OnPlayerLocationChanged(Player player);
    public OnPlayerLocationChanged onPlayerLocationChanged;

    public delegate void OnWorldSaveLoaded(WorldData worldData);
    public OnWorldSaveLoaded onWorldDataLoaded;

    public delegate void OnPlayerLoaded(Player player);
    public OnPlayerLoaded onPlayerLoaded;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        foreach (Player player in players)
        {
            if (player == null)
            {
                Debug.LogError("Player was null. It should have been removed");
            }

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
        int offset = ChunkSize / 2;

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

    public bool ChunkInRangeAnyPlayer(Vector3Int chunkPosition)
    {
        foreach (Player player in players)
        {
            if (ChunkInRange(chunkPosition, playerPositions[player]))
            {
                return true;
            }
        }

        return false;
    }

    public void SetWorld(WorldData worldData)
    {
        this.worldData = worldData;

        onWorldDataLoaded.Invoke(worldData);

        AddPlayer("Player");
    }

    public void AddPlayer(string playerName)
    {
        GameObject playerGameObject = new GameObject(playerName);
        playerGameObject.transform.position = new Vector3(0, 0, 0);

        Player player = playerGameObject.AddComponent<Player>();

        players.Add(player);

        playerPositions.Add(player, WorldToChunkPosition(player.transform.position));

        onPlayerLoaded.Invoke(player);
    }
}
