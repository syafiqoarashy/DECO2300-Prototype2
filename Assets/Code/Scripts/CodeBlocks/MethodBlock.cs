using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MethodBlock : ContainerBlock
{
    public enum MethodType { MoveRight, MoveUp, MoveDown, MoveLeft }
    public MethodType Type;

    public XRSocketInteractor upperSocket;
    public XRSocketInteractor lowerSocket;
    public XRSocketInteractor variableSocket;

    private MethodBlock connectedUpperBlock;
    private MethodBlock connectedLowerBlock;
    private VariableBlock connectedVariableBlock;

    private void Start()
    {
        if (upperSocket != null)
            upperSocket.selectEntered.AddListener(OnUpperSocketAttached);
        if (lowerSocket != null)
            lowerSocket.selectEntered.AddListener(OnLowerSocketAttached);
        if (variableSocket != null)
            variableSocket.selectEntered.AddListener(OnVariableAttached);
    }

    private void OnUpperSocketAttached(SelectEnterEventArgs args)
    {
        MethodBlock attachedBlock = args.interactableObject.transform.GetComponent<MethodBlock>();
        if (attachedBlock != null)
        {
            connectedUpperBlock = attachedBlock;
            Debug.Log($"Method block upper socket is stuck to {attachedBlock.Type} block");
        }
    }

    private void OnLowerSocketAttached(SelectEnterEventArgs args)
    {
        MethodBlock attachedBlock = args.interactableObject.transform.GetComponent<MethodBlock>();
        if (attachedBlock != null)
        {
            connectedLowerBlock = attachedBlock;
            Debug.Log($"Method block lower socket is stuck to {attachedBlock.Type} block");
        }
    }

    private void OnVariableAttached(SelectEnterEventArgs args)
    {
        VariableBlock variableBlock = args.interactableObject.transform.GetComponent<VariableBlock>();
        if (variableBlock != null)
        {
            connectedVariableBlock = variableBlock;
            AcceptBlock(variableBlock);
            Debug.Log($"Variable block with value {variableBlock.GetValue()} has been stuck to Method Block {Type}");
        }
    }

    public override bool CanAcceptBlock(CodeBlock block)
    {
        if (block is VariableBlock)
            return connectedVariableBlock == null;
        if (block is MethodBlock)
            return true;
        return false;
    }

    public override void AcceptBlock(CodeBlock block)
    {
        if (CanAcceptBlock(block))
        {
            base.AcceptBlock(block);
        }
    }

    public override void Execute()
    {
        // Execute the upper connected block first, if it exists
        connectedUpperBlock?.Execute();

        // Execute this block
        if (connectedVariableBlock != null)
        {
            Debug.Log($"Executing {Type} with parameter: {connectedVariableBlock.GetValue()}");
        }
        else
        {
            Debug.Log($"Executing {Type} without a parameter");
        }

        // Execute the lower connected block last, if it exists
        connectedLowerBlock?.Execute();
    }

    public void RemoveBlock(CodeBlock block)
    {
        if (block == connectedVariableBlock)
        {
            connectedVariableBlock = null;
            Debug.Log($"Removed variable block from {Type}");
        }
        else if (block == connectedUpperBlock)
        {
            connectedUpperBlock = null;
            Debug.Log($"Removed upper method block from {Type}");
        }
        else if (block == connectedLowerBlock)
        {
            connectedLowerBlock = null;
            Debug.Log($"Removed lower method block from {Type}");
        }
    }
}