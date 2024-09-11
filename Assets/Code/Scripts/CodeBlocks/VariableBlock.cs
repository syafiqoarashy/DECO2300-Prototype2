using UnityEngine;

public class VariableBlock : CodeBlock
{
    public enum VariableType { Integer, Boolean }
    public VariableType Type;

    public int IntegerValue;
    public bool BooleanValue;

    public object GetValue()
    {
        return Type switch
        {
            VariableType.Integer => IntegerValue,
            VariableType.Boolean => BooleanValue,
            _ => null,
        };
    }

    public void SetValue(object value)
    {
        switch (Type)
        {
            case VariableType.Integer:
                if (value is int intValue)
                {
                    IntegerValue = intValue;
                }
                else
                {
                    Debug.LogError("Attempted to set non-integer value to Integer VariableBlock");
                }
                break;
            case VariableType.Boolean:
                if (value is bool boolValue)
                {
                    BooleanValue = boolValue;
                }
                else
                {
                    Debug.LogError("Attempted to set non-boolean value to Boolean VariableBlock");
                }
                break;
        }
    }

    public override void Execute()
    {
        Debug.Log($"Variable of type {Type} with value: {GetValue()}");
    }
}