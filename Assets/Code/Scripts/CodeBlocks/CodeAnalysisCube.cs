using UnityEngine;

public class CodeAnalysisCube : MonoBehaviour
{
    public CodeExecutor codeExecutor;

    private void OnTriggerEnter(Collider other)
    {
        MethodBlock newBlock = other.GetComponent<MethodBlock>();
        if (newBlock != null && codeExecutor != null)
        {
            codeExecutor.SetCodeBlock(newBlock.GetAbsoluteTopmostBlock());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MethodBlock>() != null)
        {
            ResetAllMethodBlockColors();
            Debug.Log("Code block group exited. All block colors reset.");
        }
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
