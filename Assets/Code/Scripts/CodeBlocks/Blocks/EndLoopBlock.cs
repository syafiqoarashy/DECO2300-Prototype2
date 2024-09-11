using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndLoopBlock : MonoBehaviour
{
    public XRSocketInteractor upperSocket;
    public XRSocketInteractor lowerSocket;

    public CodeBlock attachedUpperBlock;
    public CodeBlock attachedLowerBlock;

    private Renderer blockRenderer;
    private Material blockMaterial;
    private Color originalColor;
    private bool isFailed = false;

    private void Start()
    {
        if (upperSocket != null)
            upperSocket.selectEntered.AddListener(OnUpperSocketAttached);
        if (lowerSocket != null)
            lowerSocket.selectEntered.AddListener(OnLowerSocketAttached);

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
            attachedUpperBlock = attachedBlock;
        }
    }

    private void OnLowerSocketAttached(SelectEnterEventArgs args)
    {
        CodeBlock attachedBlock = args.interactableObject.transform.GetComponent<CodeBlock>();
        if (attachedBlock != null)
        {
            attachedLowerBlock = attachedBlock;
        }
    }

    public void Execute()
    {
        // Execution logic will be handled by CodeExecutor
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

    public string AnalyzeCode()
    {
        return "End Loop";
    }
}
