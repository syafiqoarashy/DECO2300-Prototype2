using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodBlock : ContainerBlock
{
    public enum MethodType { MoveRight, MoveUp, MoveDown, MoveLeft }
    public MethodType Type;

    public override bool CanAcceptBlock(CodeBlock block)
    {
        return block is VariableBlock && childBlocks.Count == 0;
    }

    public override void Execute()
    {
        Debug.Log($"Executing {Type} with parameter: {childBlocks[0]}");
    }
}
