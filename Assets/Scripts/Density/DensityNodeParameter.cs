using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DensityNodeParameter
{
    public string name;
    public ParameterType type;

    public float floatValue;
    public int intValue;
    public bool boolValue;
    public Vector2 vector2Value;
    public Vector3 vector3Value;
    public Vector4 vector4Value;
}

public enum ParameterType
{
    Float,
    Int,
    Bool,
    Vector2,
    Vector3,
    Vector4
}