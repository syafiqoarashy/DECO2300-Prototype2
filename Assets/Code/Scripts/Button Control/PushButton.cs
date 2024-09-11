using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PushButton : MonoBehaviour
{
    public CodeExecutor codeExecutor;
    public float pushDistance = 0.01f;
    public float pushDuration = 0.1f;
    public bool isForward = true;

    private XRSimpleInteractable interactable;
    private Vector3 originalPosition;
    private bool isPressed = false;

    private void Start()
    {
        originalPosition = transform.localPosition;
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }
        interactable.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!isPressed)
        {
            isPressed = true;
            StartCoroutine(AnimateButton());
            if (codeExecutor != null)
            {
                if (isForward)
                {
                    codeExecutor.ExecuteNextStep();
                }
                else
                {
                    codeExecutor.ExecutePreviousStep();
                }
            }
        }
    }

    private System.Collections.IEnumerator AnimateButton()
    {
        Vector3 pushedPosition = originalPosition - Vector3.forward * pushDistance;
        float elapsedTime = 0f;

        while (elapsedTime < pushDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, pushedPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            transform.localPosition = Vector3.Lerp(pushedPosition, originalPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isPressed = false;
    }
}
