using UnityEngine;

public abstract class MegachunkIdentity : ScriptableObject
{
    public string megachunkUID;
    public string megachunkName;
    [Range(0, 1)] public float spawnChance = .25f;

    public virtual MegachunkData Generate()
    {
        Debug.LogError("MegachunkIdentity.Generate() must be overridden in a derived class.");

        return null;
    }

    /*
    public virtual List<BiomeGeneration> GetBiomeGenerations()
    {
        Debug.LogError("MegachunkIdentity.GetBiomeGenerations() must be overridden in a derived class.");

        return null;
    }
    */
}