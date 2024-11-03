using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeDensityNode
{
    public DensityNode densityNode; // Reference to the shared DensityNode
    public List<DensityNodeParameter> overriddenParameters = new List<DensityNodeParameter>();

    // Constructor to copy default parameters from the DensityNode
    public BiomeDensityNode(DensityNode densityNode)
    {
        this.densityNode = densityNode;

        // Initialize overridden parameters with copies of the default parameters
        foreach (var param in densityNode.parameters)
        {
            var copy = new DensityNodeParameter
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
            overriddenParameters.Add(copy);
        }
    }

    // Method to get the value of a parameter by name
    public DensityNodeParameter GetParameter(string name)
    {
        return overriddenParameters.Find(p => p.name == name);
    }
}