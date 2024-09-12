using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class MethodBlock : ContainerBlock
{
    public enum MethodType { MoveRight, MoveUp, MoveDown, MoveLeft }
    public MethodType Type;
    public XRSocketInteractor upperSocket;
    public XRSocketInteractor lowerSocket;
    public XRSocketInteractor variableSocket;
    public MethodBlock attachedUpperBlock;
    public MethodBlock attachedLowerBlock;
    public ForLoopBlock attachedUpperForBlock;
    public ForLoopBlock attachedLowerForBlock;
    public VariableBlock attachedVariableBlock;

    public TMP_Text methodTypeText;

    private Renderer blockRenderer;
    private Color originalColor;
    private Material blockMaterial;
    private bool isFailed = false;

    private void Start()
    {
        if (upperSocket != null)
        {
            upperSocket.selectEntered.AddListener(OnUpperSocketAttached);
            upperSocket.selectExited.AddListener(OnUpperSocketDetached);
        }
        if (lowerSocket != null)
        {
            lowerSocket.selectEntered.AddListener(OnLowerSocketAttached);
            lowerSocket.selectExited.AddListener(OnLowerSocketDetached);
        }
        if (variableSocket != null)
        {
            variableSocket.selectEntered.AddListener(OnVariableAttached);
            variableSocket.selectExited.AddListener(OnVariableDetached);
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            blockMaterial = renderer.material;
            originalColor = blockMaterial.color;
        }

        UpdateMethodTypeText();
    }

    private void UpdateMethodTypeText()
    {
        if (methodTypeText != null)
        {
            methodTypeText.text = Type.ToString();
        }
    }

    public MethodBlock GetAbsoluteTopmostBlock()
    {
        MethodBlock currentBlock = this;

        while (currentBlock.attachedLowerBlock != null)
        {
            currentBlock = currentBlock.attachedLowerBlock;
        }

        while (currentBlock.attachedUpperBlock != null)
        {
            currentBlock = currentBlock.attachedUpperBlock;
        }

        return currentBlock;
    }

    private void OnUpperSocketAttached(SelectEnterEventArgs args)
    {
        CodeBlock attachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (attachedBlock != null)
        {
            if (attachedBlock is MethodBlock methodBlock)
            {
                attachedUpperBlock = methodBlock;
                methodBlock.attachedLowerBlock = this;
            }
            else if (attachedBlock is ForLoopBlock forLoopBlock)
            {
                attachedUpperForBlock = forLoopBlock;
                forLoopBlock.attachedLowerBlock = this;
            }
        }
    }

    private void OnLowerSocketAttached(SelectEnterEventArgs args)
    {
        CodeBlock attachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (attachedBlock != null)
        {
            if (attachedBlock is MethodBlock methodBlock)
            {
                attachedLowerBlock = methodBlock;
                methodBlock.attachedUpperBlock = this;
            }
            else if (attachedBlock is ForLoopBlock forLoopBlock)
            {
                attachedLowerForBlock = forLoopBlock;
                forLoopBlock.attachedUpperBlock = this;
            }
        }
    }

    private void OnVariableAttached(SelectEnterEventArgs args)
    {
        VariableBlock variableBlock = args.interactableObject.transform.GetComponent<VariableBlock>();
        if (variableBlock != null)
        {
            attachedVariableBlock = variableBlock;
        }
    }

    public void OnUpperSocketDetached(SelectExitEventArgs args)
    {
        CodeBlock detachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (detachedBlock != null)
        {
            if (detachedBlock is MethodBlock methodBlock && methodBlock == attachedUpperBlock)
            {
                attachedUpperBlock.attachedLowerBlock = null;
                attachedUpperBlock = null;
            }
            else if (detachedBlock is ForLoopBlock forLoopBlock && forLoopBlock == attachedUpperForBlock)
            {
                attachedUpperForBlock.attachedLowerForBlock = null;
                attachedUpperForBlock = null;
            }
        }
    }

    public void OnLowerSocketDetached(SelectExitEventArgs args)
    {
        CodeBlock detachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (detachedBlock != null)
        {
            if (detachedBlock is MethodBlock methodBlock && methodBlock == attachedLowerBlock)
            {
                attachedLowerBlock.attachedUpperBlock = null;
                attachedLowerBlock = null;
            }
            else if (detachedBlock is ForLoopBlock forLoopBlock && forLoopBlock == attachedLowerForBlock)
            {
                attachedLowerForBlock.attachedUpperForBlock = null;
                attachedLowerForBlock = null;
            }
        }
    }

    public void OnVariableDetached(SelectExitEventArgs args)
    {
        VariableBlock detachedBlock = args.interactableObject.transform.GetComponent<VariableBlock>();
        if (detachedBlock != null && detachedBlock == attachedVariableBlock)
        {
            attachedVariableBlock = null;
        }
    }

    public override void Execute()
    {
    }

    public void SetBlockColor(Color color)
    {
        if (blockMaterial != null && !isFailed)
        {
            blockMaterial.color = color;
        }
    }

    public void MarkAsFailed()
    {
        if (blockMaterial != null)
        {
            blockMaterial.color = Color.red;
            isFailed = true;
        }
    }

    public void ResetBlockColor()
    {
        if (blockMaterial != null && !isFailed)
        {
            blockMaterial.color = originalColor;
        }
    }

    public void ClearFailureState()
    {
        isFailed = false;
    }

    public void SetMethodType(MethodType newType)
    {
        Type = newType;
        UpdateMethodTypeText();
    }
}
