using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Density Node", menuName = "World/Density Node")]
public class DensityNode : ScriptableObject
{
    public ComputeShader shader;
    public List<DensityNodeParameter> parameters = new List<DensityNodeParameter>();

    public void SetParameter(string name, object value)
    {
        var param = parameters.Find(p => p.name == name);
        if (param == null)
        {
            Debug.LogError($"Parameter {name} not found in Density Node {name}");
            return;
        }

        switch (param.type)
        {
            case ParameterType.Float:
                param.floatValue = (float)value;
                break;
            case ParameterType.Int:
                param.intValue = (int)value;
                break;
            case ParameterType.Bool:
                param.boolValue = (bool)value;
                break;
            case ParameterType.Vector2:
                param.vector2Value = (Vector2)value;
                break;
            case ParameterType.Vector3:
                param.vector3Value = (Vector3)value;
                break;
            case ParameterType.Vector4:
                param.vector4Value = (Vector4)value;
                break;
            default:
                Debug.LogError($"Parameter {name} has an invalid type");
                break;
        }
    }
}
