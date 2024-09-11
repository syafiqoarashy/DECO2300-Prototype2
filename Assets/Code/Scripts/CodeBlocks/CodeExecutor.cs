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

    private MethodBlock currentBlock;
    private int currentStep = 0;
    private List<MethodBlock> executionOrder = new List<MethodBlock>();
    private Vector3 originalPlayerPosition;

    private void Start()
    {
        originalPlayerPosition = player.position;
        errorCanvas.SetActive(false);
    }

    public void SetCodeBlock(MethodBlock block)
    {
        ResetPlayer();
        currentBlock = block?.GetAbsoluteTopmostBlock();
        currentStep = 0;
        executionOrder.Clear();
        if (currentBlock != null)
        {
            MethodBlock temp = currentBlock;
            while (temp != null)
            {
                executionOrder.Add(temp);
                temp = temp.attachedLowerBlock;
            }
            ColorCodeBlocks();
        }
    }

    public void ExecuteNextStep()
    {
        if (currentStep >= executionOrder.Count)
        {
            Debug.Log("Code execution complete.");
            return;
        }

        MethodBlock blockToExecute = executionOrder[currentStep];
        bool executed = ExecuteBlock(blockToExecute);

        if (!executed)
        {
            blockToExecute.MarkAsFailed();
            Debug.Log("Execution failed.");
            ResetPlayer();
            return;
        }

        blockToExecute.SetBlockColor(Color.green);
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
        MethodBlock blockToExecute = executionOrder[currentStep];

        UndoBlockExecution(blockToExecute);
        blockToExecute.ResetBlockColor();

        ColorCodeBlocks();
    }

    private bool ExecuteBlock(MethodBlock block)
    {
        if (block.attachedVariableBlock == null)
        {
            DisplayError($"Execution failed: {block.Type} is missing a variable block.");
            return false;
        }

        Vector3 movement = Vector3.zero;
        switch (block.Type)
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

        int steps = block.attachedVariableBlock.IntegerValue;
        Vector3 newPosition = player.position + movement * moveDistance * steps;

        if (IsOnGridPath(newPosition))
        {
            player.position = newPosition;
            Debug.Log($"Executed: {block.Type} {steps} step(s)");
            CheckGoal();
            return true;
        }
        else
        {
            DisplayError($"Execution failed: Moving {block.Type} {steps} step(s) would place the player outside the GridPath.");
            return false;
        }
    }

    private void UndoBlockExecution(MethodBlock block)
    {
        if (block.attachedVariableBlock == null)
        {
            Debug.Log($"Cannot undo: {block.Type} is missing a variable block.");
            return;
        }

        Vector3 movement = Vector3.zero;
        switch (block.Type)
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

        int steps = block.attachedVariableBlock.IntegerValue;
        player.position += movement * moveDistance * steps;
        Debug.Log($"Undid: {block.Type} {steps} step(s)");
    }

    private bool IsOnGridPath(Vector3 position)
    {
        float rayLength = 1.0f;
        Vector3 rayOrigin = position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength, gridPathLayer))
        {
            Debug.Log($"Ray hit: {hit.collider.gameObject.name} at {hit.point}");
            return true;
        }
        Debug.Log("Raycast did not hit a valid GridPath.");
        return false;
    }

    private void CheckGoal()
    {
        if (Vector3.Distance(player.position, gridGoal.position) < 0.1f)
        {
            Debug.Log("Goal Reached!");
        }
    }

    private void ColorCodeBlocks()
    {
        for (int i = 0; i < executionOrder.Count; i++)
        {
            if (i < currentStep)
            {
                executionOrder[i].SetBlockColor(Color.green);
            }
            else if (i == currentStep)
            {
                executionOrder[i].SetBlockColor(Color.blue);
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
