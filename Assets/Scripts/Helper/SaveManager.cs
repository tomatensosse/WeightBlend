using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Save Generator")]
    public bool loadWorld = false;
    public int seed = -1;
    public WorldConfig worldConfig;

    public const string saveFolder = "Assets/Out/";
    public const string saveName = "world.json";


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
        WorldData worldData = TryLoadWorld();

        World.Instance.SetWorld(worldData);
    }

    void OnApplicationQuit()
    {
        SaveWorld();
    }

    private WorldData TryLoadWorld()
    {
        if (!System.IO.Directory.Exists(saveFolder))
        {
            System.IO.Directory.CreateDirectory(saveFolder);
        }

        if (!loadWorld)
        {
            Debug.Log("Overwrited world");
            return new WorldData(new WorldConfig());
        }

        if (System.IO.File.Exists(saveFolder + saveName))
        {
            Debug.Log("Found world");
            string json = System.IO.File.ReadAllText(saveFolder + saveName);
            return JsonUtility.FromJson<WorldData>(json);
        }

        Debug.Log("Creating new world");
        return new WorldData(new WorldConfig());
    }

    private void SaveWorld()
    {
        string json = JsonUtility.ToJson(World.WorldData);
        System.IO.File.WriteAllText(saveFolder + saveName, json);
        Debug.Log("Saved world");
    }
}
