using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "World/Biome")]
public class Biome : ScriptableObject
{
    public string biomeUID;
    public string biomeName;
    public Color biomeColor = Color.white;

    public List<BiomeDensityNode> densityNodes = new List<BiomeDensityNode>();

    public void AddDensityNode(DensityNode densityNode)
    {
        if (densityNode != null)
        {
            densityNodes.Add(new BiomeDensityNode(densityNode));
        }
    }
}
