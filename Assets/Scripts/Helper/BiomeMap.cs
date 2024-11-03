using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BiomeMap : ISerializationCallbackReceiver
{
    [SerializeField] private int[] flattenedBiomeMap;
    [SerializeField] private int scale;
    [DoNotSerialize] public int[,,] map;

    public void OnBeforeSerialize()
    {
        int xSize = map.GetLength(0);
        int ySize = map.GetLength(1);
        int zSize = map.GetLength(2);

        flattenedBiomeMap = new int[xSize * ySize * zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    flattenedBiomeMap[x + xSize * (y + ySize * z)] = map[x, y, z];
                }
            }
        }

        if (xSize != ySize || xSize != zSize || ySize != zSize)
        {
            Debug.LogError("BiomeMap is not a cube!");
        }

        scale = xSize;
    }

    public void OnAfterDeserialize()
    {
        map = new int[scale, scale, scale];
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    map[x, y, z] = flattenedBiomeMap[x + scale * (y + scale * z)];
                }
            }
        }
    }
}