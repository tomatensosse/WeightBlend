using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public static DebugScript Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPlayer()
    {
        World.Instance.AddPlayer(RandomName());
    }

    private string RandomName()
    {
        string[] names = new string[] { "John", "Jane", "Bob", "Alice", "Charlie", "Eve", "Mallory", "Trudy" };

        return names[Random.Range(0, names.Length)] + "_" + Random.Range(0, 1000);
    }
}
