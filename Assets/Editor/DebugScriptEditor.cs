using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugScript))]
public class DebugScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DebugScript debugScript = (DebugScript)target;

        if (GUILayout.Button("Add Player"))
        {
            debugScript.AddPlayer();
        }
    }
}
