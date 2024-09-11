using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class MethodBlock : MonoBehaviour
{
    public enum MethodType { MoveRight, MoveUp, MoveDown, MoveLeft }
    public MethodType Type;
    public XRSocketInteractor upperSocket;
    public XRSocketInteractor lowerSocket;
    public XRSocketInteractor variableSocket;
    public MethodBlock attachedUpperBlock;
    public MethodBlock attachedLowerBlock;
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
        MethodBlock attachedBlock = args.interactableObject.transform.GetComponent<MethodBlock>();
        if (attachedBlock != null)
        {
            attachedUpperBlock = attachedBlock;
            attachedBlock.attachedLowerBlock = this;
        }
    }

    private void OnLowerSocketAttached(SelectEnterEventArgs args)
    {
        MethodBlock attachedBlock = args.interactableObject.transform.GetComponent<MethodBlock>();
        if (attachedBlock != null)
        {
            attachedLowerBlock = attachedBlock;
            attachedBlock.attachedUpperBlock = this;
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
        if (args.interactableObject.transform.GetComponent<MethodBlock>() == attachedUpperBlock)
        {
            attachedUpperBlock.attachedLowerBlock = null;
            attachedUpperBlock = null;
        }
    }

    public void OnLowerSocketDetached(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.GetComponent<MethodBlock>() == attachedLowerBlock)
        {
            attachedLowerBlock.attachedUpperBlock = null;
            attachedLowerBlock = null;
        }
    }

    public void OnVariableDetached(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.GetComponent<VariableBlock>() == attachedVariableBlock)
        {
            attachedVariableBlock = null;
        }
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
        string code = $"{Type}";
        if (attachedVariableBlock != null)
        {
            code += $"({attachedVariableBlock.GetValue()})";
        }
        else
        {
            code += "()";
        }
        if (attachedLowerBlock != null)
        {
            code += "\n" + attachedLowerBlock.AnalyzeCode();
        }
        return code;
    }

    public void SetMethodType(MethodType newType)
    {
        Type = newType;
        UpdateMethodTypeText();
    }
}
