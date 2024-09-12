using UnityEngine;

public class CodeAnalysisCube : MonoBehaviour
{
    public CodeExecutor codeExecutor;

    private void OnTriggerEnter(Collider other)
    {
        CodeBlock newBlock = other.GetComponent<CodeBlock>();
        if (newBlock != null && codeExecutor != null)
        {
            codeExecutor.SetCodeBlock(GetTopmostBlock(newBlock));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CodeBlock exitingBlock = other.GetComponent<CodeBlock>();
        if (exitingBlock != null)
        {
            ResetAllMethodBlockColors();
            Debug.Log("Code block group exited. All block colors reset.");
        }
    }

    private CodeBlock GetTopmostBlock(CodeBlock block)
    {
        CodeBlock topBlock = block;

        while (true)
        {
            CodeBlock nextBlock = null;

            if (topBlock is MethodBlock methodBlock)
            {
                nextBlock = methodBlock.attachedUpperBlock;
                if (methodBlock.attachedUpperForBlock != null)
                {
                    nextBlock = methodBlock.attachedUpperForBlock;
                }
            }
            else if (topBlock is ForLoopBlock forLoopBlock)
            {
                nextBlock = forLoopBlock.attachedUpperForBlock;
                if (forLoopBlock.attachedUpperBlock != null)
                {
                    nextBlock = forLoopBlock.attachedUpperBlock;
                }
            }

            if (nextBlock == null)
            {
                break;
            }

            topBlock = nextBlock;
        }

        return topBlock;
    }

    private void ResetAllMethodBlockColors()
    {
        MethodBlock[] allMethodBlocks = FindObjectsOfType<MethodBlock>();
        foreach (MethodBlock block in allMethodBlocks)
        {
            block.ResetBlockColor();
            block.ClearFailureState();
        }
    }
}
