using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public List<MegachunkData> megachunkDatas;

    // Variables for the world
    public int seed;
    public int chunkSize = -1;
    public int numPointsPerAxis = -1;

    public WorldData(WorldConfig worldConfig)
    {
        chunkSize = WorldConfig.ChunkSize(worldConfig.worldSize);
        numPointsPerAxis = WorldConfig.NumPointsPerAxis(worldConfig.worldSize);
    }
}
