using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForLoopBlock : ContainerBlock
{
    public XRSocketInteractor upperSocket;
    public XRSocketInteractor lowerSocket;
    public XRSocketInteractor startVariableSocket;
    public XRSocketInteractor endVariableSocket;

    public VariableBlock startVariable;
    public VariableBlock endVariable;

    public MethodBlock attachedUpperBlock;
    public MethodBlock attachedLowerBlock;
    public ForLoopBlock attachedUpperForBlock;
    public ForLoopBlock attachedLowerForBlock;

    private Renderer blockRenderer;
    private Material blockMaterial;
    private Color originalColor;
    private bool isFailed = false;

    public bool IsEndLoopBlock = false;

    private void Start()
    {
        if (upperSocket != null)
            upperSocket.selectEntered.AddListener(OnUpperSocketAttached);
        if (lowerSocket != null)
            lowerSocket.selectEntered.AddListener(OnLowerSocketAttached);
        if (startVariableSocket != null)
            startVariableSocket.selectEntered.AddListener(OnStartVariableAttached);
        if (endVariableSocket != null)
            endVariableSocket.selectEntered.AddListener(OnEndVariableAttached);

        blockRenderer = GetComponent<Renderer>();
        if (blockRenderer != null)
        {
            blockMaterial = blockRenderer.material;
            originalColor = blockMaterial.color;
        }
    }

    private void OnUpperSocketAttached(SelectEnterEventArgs args)
    {
        CodeBlock attachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (attachedBlock != null)
        {
            if (attachedBlock is MethodBlock methodBlock)
            {
                attachedUpperBlock = methodBlock;
                methodBlock.attachedLowerForBlock = this;
            }
            else if (attachedBlock is ForLoopBlock forLoopBlock)
            {
                attachedUpperForBlock = forLoopBlock;
                forLoopBlock.attachedLowerForBlock = this;
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
                methodBlock.attachedUpperForBlock = this;
            }
            else if (attachedBlock is ForLoopBlock forLoopBlock)
            {
                attachedLowerForBlock = forLoopBlock;
                forLoopBlock.attachedUpperForBlock = this;
            }
        }
    }

    private void OnStartVariableAttached(SelectEnterEventArgs args)
    {
        VariableBlock variableBlock = args.interactableObject.transform.GetComponent<VariableBlock>();
        if (variableBlock != null)
        {
            startVariable = variableBlock;
        }
    }

    private void OnEndVariableAttached(SelectEnterEventArgs args)
    {
        VariableBlock variableBlock = args.interactableObject.transform.GetComponent<VariableBlock>();
        if (variableBlock != null)
        {
            endVariable = variableBlock;
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

    public int GetStartValue()
    {
        return startVariable != null ? startVariable.IntegerValue : 0;
    }

    public int GetEndValue()
    {
        return endVariable != null ? endVariable.IntegerValue : 0;
    }
}
