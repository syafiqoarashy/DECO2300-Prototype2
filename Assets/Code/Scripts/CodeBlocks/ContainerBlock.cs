using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContainerBlock : CodeBlock
{
    protected List<CodeBlock> childBlocks = new List<CodeBlock>();

    public override bool CanAcceptBlock(CodeBlock block)
    {
        return true;
    }

    public override void AcceptBlock(CodeBlock block)
    {
        if (CanAcceptBlock(block))
        {
            childBlocks.Add(block);
        }
    }
}
