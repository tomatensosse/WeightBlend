using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DensityNode))]
public class DensityNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DensityNode densityNode = (DensityNode)target;

        // Draw the shader field
        densityNode.shader = (ComputeShader)EditorGUILayout.ObjectField("Shader", densityNode.shader, typeof(ComputeShader), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

        // Iterate over each parameter and display only name, type, and a remove button
        for (int i = 0; i < densityNode.parameters.Count; i++)
        {
            var param = densityNode.parameters[i];

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Name", GUILayout.Width(50));
            param.name = EditorGUILayout.TextField(param.name);

            EditorGUILayout.LabelField("Type", GUILayout.Width(50));
            param.type = (ParameterType)EditorGUILayout.EnumPopup(param.type);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                densityNode.parameters.RemoveAt(i);
                i--; // Adjust the index to avoid skipping an element
            }

            EditorGUILayout.EndHorizontal();
        }

        // Button to add a new parameter
        if (GUILayout.Button("Add Parameter"))
        {
            densityNode.parameters.Add(new DensityNodeParameter { name = "NewParam", type = ParameterType.Float });
        }

        // Mark the object as dirty so changes are saved
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}