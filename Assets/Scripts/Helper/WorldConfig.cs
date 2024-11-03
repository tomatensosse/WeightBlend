using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldConfig
{
    public enum WorldSize { Small, Medium, Large }

    public WorldSize worldSize;

    public static int ChunkSize(WorldSize worldSize)
    {
        switch (worldSize)
        {
            case WorldSize.Small:
                return 32;
            case WorldSize.Medium:
                return 64;
            case WorldSize.Large:
                return 128;
            default:
                return 64;
        }
    }

    public static int NumPointsPerAxis(WorldSize worldSize)
    {
        switch (worldSize)
        {
            case WorldSize.Small:
                return 16;
            case WorldSize.Medium:
                return 32;
            case WorldSize.Large:
                return 64;
            default:
                return 32;
        }
    }
}
