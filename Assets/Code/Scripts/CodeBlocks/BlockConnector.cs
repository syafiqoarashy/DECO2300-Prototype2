using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleBlockConnector : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;

    void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnBlockAttached);
            socketInteractor.selectExited.AddListener(OnBlockDetached);
        }
    }

    void OnBlockAttached(SelectEnterEventArgs args)
    {
        Rigidbody attachedRigidbody = args.interactableObject.transform.GetComponent<Rigidbody>();
        if (attachedRigidbody != null)
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = attachedRigidbody;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            Debug.Log($"Block attached to {gameObject.name}");
        }
    }

    void OnBlockDetached(SelectExitEventArgs args)
    {
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
        {
            Destroy(joint);
            Debug.Log($"Block detached from {gameObject.name}");
        }
    }
}