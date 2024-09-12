using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class CodeExecutor : MonoBehaviour
{
    public Transform player;
    public Transform gridGoal;
    public float moveDistance = 0.25f;
    public LayerMask gridPathLayer;
    public TMP_Text errorMessage;
    public GameObject errorCanvas;

    private MethodBlock currentMethodBlock;
    private ForLoopBlock currentForLoopBlock;
    private int currentStep = 0;
    private List<CodeBlock> executionOrder = new List<CodeBlock>();
    private Vector3 originalPlayerPosition;
    private Dictionary<ForLoopBlock, int> loopIterationCounts = new Dictionary<ForLoopBlock, int>();
    private bool isAtEndOfLoop = false;

    private void Start()
    {
        originalPlayerPosition = player.position;
        errorCanvas.SetActive(false);
    }

    public void SetCodeBlock(CodeBlock block)
    {
        ResetPlayer();
        currentStep = 0;
        executionOrder.Clear();
        loopIterationCounts.Clear();
        isAtEndOfLoop = false;

        if (block == null) return;

        FlattenCodeStructure(block);

        LogExecutionOrder();
        ColorCodeBlocks();
    }

    private void FlattenCodeStructure(CodeBlock block)
    {
        Stack<CodeBlock> stack = new Stack<CodeBlock>();
        stack.Push(block);

        while (stack.Count > 0)
        {
            CodeBlock currentBlock = stack.Pop();

            if (currentBlock != null)
            {
                executionOrder.Add(currentBlock);

                if (currentBlock is ForLoopBlock forLoopBlock)
                {
                    if (forLoopBlock.IsEndLoopBlock)
                    {
                        continue;
                    }

                    MethodBlock child = forLoopBlock.attachedLowerBlock;
                    while (child != null)
                    {
                        stack.Push(child);
                        child = child.attachedLowerBlock;
                    }

                    loopIterationCounts[forLoopBlock] = (forLoopBlock.GetEndValue()-1) - forLoopBlock.GetStartValue();
                }
                else if (currentBlock is MethodBlock methodBlock)
                {
                    if (methodBlock.attachedLowerBlock != null)
                    {
                        stack.Push(methodBlock.attachedLowerBlock);
                    }
                    if (methodBlock.attachedLowerForBlock != null)
                    {
                        stack.Push(methodBlock.attachedLowerForBlock);
                    }
                }
            }
        }
    }

    public void ExecuteNextStep()
    {
        if (currentStep >= executionOrder.Count)
        {
            Debug.Log("Code execution complete.");
            return;
        }

        if (isAtEndOfLoop)
        {
            isAtEndOfLoop = false;
            currentStep++;
            ColorCodeBlocks();
            return;
        }

        CodeBlock blockToExecute = executionOrder[currentStep];
        bool executed = ExecuteBlock(blockToExecute);

        if (!executed)
        {
            if (blockToExecute is MethodBlock methodBlock)
            {
                methodBlock.MarkAsFailed();
            }
            Debug.Log("Execution failed.");
            ResetPlayer();
            return;
        }

        if (blockToExecute is MethodBlock methodBlockToColor)
        {
            methodBlockToColor.SetBlockColor(Color.green);
        }
        currentStep++;
        ColorCodeBlocks();
    }

    public void ExecutePreviousStep()
    {
        if (currentStep <= 0)
        {
            Debug.Log("Already at the start of the execution.");
            return;
        }

        currentStep--;
        CodeBlock blockToExecute = executionOrder[currentStep];

        UndoBlockExecution(blockToExecute);
        if (blockToExecute is MethodBlock methodBlockToReset)
        {
            methodBlockToReset.ResetBlockColor();
        }

        ColorCodeBlocks();
    }

    private bool ExecuteBlock(CodeBlock block)
    {
        if (block is MethodBlock methodBlock)
        {
            if (methodBlock.attachedVariableBlock == null)
            {
                DisplayError($"Execution failed: {methodBlock.Type} is missing a variable block.");
                return false;
            }

            Vector3 movement = Vector3.zero;
            switch (methodBlock.Type)
            {
                case MethodBlock.MethodType.MoveRight:
                    movement = Vector3.right;
                    break;
                case MethodBlock.MethodType.MoveLeft:
                    movement = Vector3.left;
                    break;
                case MethodBlock.MethodType.MoveUp:
                    movement = Vector3.forward;
                    break;
                case MethodBlock.MethodType.MoveDown:
                    movement = Vector3.back;
                    break;
            }

            int steps = methodBlock.attachedVariableBlock.IntegerValue;
            Vector3 newPosition = player.position + movement * moveDistance * steps;

            if (IsOnGridPath(newPosition))
            {
                player.position = newPosition;
                Debug.Log($"Executed: {methodBlock.Type} {steps} step(s)");
                return true;
            }
            else
            {
                DisplayError($"Execution failed: Moving {methodBlock.Type} {steps} step(s) would place the player outside the GridPath.");
                return false;
            }
        }
        else if (block is ForLoopBlock forLoopBlock)
        {
            if (forLoopBlock.IsEndLoopBlock)
            {
                HandleEndLoopBlock(forLoopBlock);
                return true;
            }
            else
            {
                ProcessForLoopBlock(forLoopBlock);
                return true;
            }
        }

        return false;
    }

    private void HandleEndLoopBlock(ForLoopBlock endLoopBlock)
    {
        Debug.Log("End of loop reached.");
        isAtEndOfLoop = true;
    }

    private void ProcessForLoopBlock(ForLoopBlock forLoopBlock)
    {
        if (forLoopBlock.startVariable == null || forLoopBlock.endVariable == null)
        {
            DisplayError("ForLoopBlock is missing start or end variable.");
            Debug.LogError("ForLoopBlock is missing start or end variable.");
            return;
        }

        int start = forLoopBlock.GetStartValue();
        int end = forLoopBlock.GetEndValue();

        List<MethodBlock> blocksToExecute = new List<MethodBlock>();
        MethodBlock child = forLoopBlock.attachedLowerBlock;

        while (child != null)
        {
            blocksToExecute.Add(child);
            child = child.attachedLowerBlock;
        }

        for (int i = start; i < end; i++)
        {
            foreach (var block in blocksToExecute)
            {
                if (block is MethodBlock methodBlock)
                {
                    executionOrder.Add(methodBlock);
                }
            }
        }

        LogExecutionOrder();
        ColorCodeBlocks();
    }

    private void UndoBlockExecution(CodeBlock block)
    {
        if (block is MethodBlock methodBlock)
        {
            if (methodBlock.attachedVariableBlock == null)
            {
                Debug.Log($"Cannot undo: {methodBlock.Type} is missing a variable block.");
                return;
            }

            Vector3 movement = Vector3.zero;
            switch (methodBlock.Type)
            {
                case MethodBlock.MethodType.MoveRight:
                    movement = Vector3.left;
                    break;
                case MethodBlock.MethodType.MoveLeft:
                    movement = Vector3.right;
                    break;
                case MethodBlock.MethodType.MoveUp:
                    movement = Vector3.back;
                    break;
                case MethodBlock.MethodType.MoveDown:
                    movement = Vector3.forward;
                    break;
            }

            int steps = methodBlock.attachedVariableBlock.IntegerValue;
            player.position += movement * moveDistance * steps;
            Debug.Log($"Undid: {methodBlock.Type} {steps} step(s)");
        }
    }

    private bool IsOnGridPath(Vector3 position)
    {
        float rayLength = 1.0f;
        Vector3 rayOrigin = position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength, gridPathLayer))
        {
            Debug.Log($"Ray hit: {hit.collider.gameObject.name} at {hit.point}");
            if (hit.collider.gameObject.name == "GridGoal")
            {
                DisplayError("Goal reached, congratulations!");
                Debug.Log("Goal Reached!");
            }
            return true;
        }
        Debug.Log("Raycast did not hit a valid GridPath.");
        return false;
    }

    private void ColorCodeBlocks()
    {
        for (int i = 0; i < executionOrder.Count; i++)
        {
            if (i < currentStep)
            {
                if (executionOrder[i] is MethodBlock methodBlock)
                {
                    methodBlock.SetBlockColor(Color.green);
                }
            }
            else if (i == currentStep)
            {
                if (executionOrder[i] is MethodBlock methodBlock)
                {
                    methodBlock.SetBlockColor(Color.blue);
                }
            }
        }
    }

    private void LogExecutionOrder()
    {
        Debug.Log("Execution Order:");
        foreach (var block in executionOrder)
        {
            if (block is MethodBlock methodBlock)
            {
                Debug.Log($"MethodBlock - Type: {methodBlock.Type}, Variable: {methodBlock.attachedVariableBlock?.IntegerValue}");
            }
            else if (block is ForLoopBlock forLoopBlock)
            {
                Debug.Log($"ForLoopBlock - Start: {forLoopBlock.GetStartValue()}, End: {forLoopBlock.GetEndValue()}");
            }
        }
    }

    public void ResetPlayer()
    {
        player.position = originalPlayerPosition;
        currentStep = 0;
        ColorCodeBlocks();
    }

    private void DisplayError(string message)
    {
        if (errorMessage != null)
        {
            errorMessage.text = message;
        }
        if (errorCanvas != null)
        {
            errorCanvas.SetActive(true);
            StartCoroutine(HideErrorCanvas());
        }
    }

    private IEnumerator HideErrorCanvas()
    {
        yield return new WaitForSeconds(3f);
        if (errorCanvas != null)
        {
            errorCanvas.SetActive(false);
        }
    }
}
