using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLoopBlock : ContainerBlock
{
    private VariableBlock startVariable;
    private VariableBlock endVariable;

    public override bool CanAcceptBlock(CodeBlock block)
    {
        if (block is VariableBlock)
        {
            return startVariable == null || endVariable == null;
        }
        return block is MethodBlock;
    }

    public override void AcceptBlock(CodeBlock block)
    {
        if (block is VariableBlock variableBlock)
        {
            if (startVariable == null)
                startVariable = variableBlock;
            else if (endVariable == null)
                endVariable = variableBlock;
        }
        else
        {
            base.AcceptBlock(block);
        }
    }

    public override void Execute()
    {
        if (startVariable == null || endVariable == null)
        {
            Debug.LogError("For loop is missing start or end variable");
            return;
        }

        int start = (int)startVariable.Value;
        int end = (int)endVariable.Value;

        for (int i = start; i <= end; i++)
        {
            foreach (var block in childBlocks)
            {
                block.Execute();
            }
        }
    }
}
