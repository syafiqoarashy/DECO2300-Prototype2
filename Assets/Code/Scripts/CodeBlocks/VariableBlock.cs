using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableBlock : CodeBlock
{
    public enum VariableType { Integer, Boolean }
    public VariableType Type;
    public object Value;

    public override void Execute()
    {
        Debug.Log($"Variable of type {Type} with value: {Value}");
    }
}
