using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Biome))]
public class BiomeEditor : Editor
{
    private List<DensityNode> availableDensityNodes = new List<DensityNode>();
    private int selectedNodeIndex = 0;

    private SerializedObject serializedBiome;

    private void OnEnable()
    {
        // Load all DensityNode assets in the project
        string[] guids = AssetDatabase.FindAssets("t:DensityNode");
        availableDensityNodes.Clear();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DensityNode node = AssetDatabase.LoadAssetAtPath<DensityNode>(path);
            if (node != null)
            {
                availableDensityNodes.Add(node);
            }
        }

        // Initialize SerializedObject
        serializedBiome = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        serializedBiome.Update();
        Biome biome = (Biome)target;

        // Display basic fields for the Biome
        EditorGUILayout.LabelField("Biome Properties", EditorStyles.boldLabel);
        biome.biomeUID = EditorGUILayout.TextField("Biome UID", biome.biomeUID);
        biome.biomeName = EditorGUILayout.TextField("Biome Name", biome.biomeName);
        biome.biomeColor = EditorGUILayout.ColorField("Biome Color", biome.biomeColor);

        EditorGUILayout.Space();

        // Dropdown to select a DensityNode
        EditorGUILayout.LabelField("Add Density Node", EditorStyles.boldLabel);

        if (availableDensityNodes.Count > 0)
        {
            // Create a list of names for the dropdown
            string[] options = new string[availableDensityNodes.Count];
            for (int i = 0; i < availableDensityNodes.Count; i++)
            {
                options[i] = availableDensityNodes[i].name;
            }

            selectedNodeIndex = EditorGUILayout.Popup("Select Density Node", selectedNodeIndex, options);

            if (GUILayout.Button("Add Selected Density Node"))
            {
                DensityNode selectedNode = availableDensityNodes[selectedNodeIndex];
                biome.AddDensityNode(selectedNode);
                EditorUtility.SetDirty(biome);
                serializedBiome.Update(); // Refresh serialized object
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Density Nodes available in project.", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Density Nodes in Biome", EditorStyles.boldLabel);

        // Ensure densityNodes is not null
        SerializedProperty densityNodesProp = serializedBiome.FindProperty("densityNodes");
        if (densityNodesProp != null)
        {
            for (int i = 0; i < densityNodesProp.arraySize; i++)
            {
                SerializedProperty biomeDensityNodeProp = densityNodesProp.GetArrayElementAtIndex(i);
                SerializedProperty densityNodeProp = biomeDensityNodeProp.FindPropertyRelative("densityNode");

                if (densityNodeProp.objectReferenceValue != null)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(densityNodeProp.objectReferenceValue.name, EditorStyles.boldLabel);

                    // Synchronize parameters
                    SyncParameters(biome.densityNodes[i]);

                    // Display each overridden parameter in a horizontal layout
                    SerializedProperty overriddenParametersProp = biomeDensityNodeProp.FindPropertyRelative("overriddenParameters");
                    for (int j = 0; j < overriddenParametersProp.arraySize; j++)
                    {
                        SerializedProperty parameterProp = overriddenParametersProp.GetArrayElementAtIndex(j);
                        SerializedProperty paramNameProp = parameterProp.FindPropertyRelative("name");
                        SerializedProperty paramTypeProp = parameterProp.FindPropertyRelative("type");

                        EditorGUILayout.BeginHorizontal();

                        // Display parameter name
                        EditorGUILayout.LabelField(paramNameProp.stringValue, GUILayout.Width(100));

                        // Display appropriate field based on parameter type next to the name
                        switch ((ParameterType)paramTypeProp.enumValueIndex)
                        {
                            case ParameterType.Float:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("floatValue"), GUIContent.none);
                                break;
                            case ParameterType.Int:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("intValue"), GUIContent.none);
                                break;
                            case ParameterType.Bool:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("boolValue"), GUIContent.none);
                                break;
                            case ParameterType.Vector2:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("vector2Value"), GUIContent.none);
                                break;
                            case ParameterType.Vector3:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("vector3Value"), GUIContent.none);
                                break;
                            case ParameterType.Vector4:
                                EditorGUILayout.PropertyField(parameterProp.FindPropertyRelative("vector4Value"), GUIContent.none);
                                break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    // Remove button
                    if (GUILayout.Button("Remove"))
                    {
                        densityNodesProp.DeleteArrayElementAtIndex(i);
                        serializedBiome.ApplyModifiedProperties(); // Apply immediately
                        break;
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }
        }

        // Apply changes to serialized object
        serializedBiome.ApplyModifiedProperties();
    }

    private void SyncParameters(BiomeDensityNode biomeDensityNode)
    {
        var densityNode = biomeDensityNode.densityNode;
        if (densityNode == null) return;

        foreach (var param in densityNode.parameters)
        {
            if (!biomeDensityNode.overriddenParameters.Exists(p => p.name == param.name))
            {
                // Create a new parameter instance if it doesn't exist
                var newParam = new DensityNodeParameter
                {
                    name = param.name,
                    type = param.type,
                    floatValue = param.floatValue,
                    intValue = param.intValue,
                    boolValue = param.boolValue,
                    vector2Value = param.vector2Value,
                    vector3Value = param.vector3Value,
                    vector4Value = param.vector4Value
                };
                biomeDensityNode.overriddenParameters.Add(newParam);
            }
        }
    }
}
