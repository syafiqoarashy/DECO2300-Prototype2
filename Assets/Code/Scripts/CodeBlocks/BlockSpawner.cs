using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform spawnPoint;

    private XRGrabInteractable grabInteractable;
    private bool hasSpawned = false;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component not found on the object.");
            return;
        }
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!hasSpawned)
        {
            SpawnAndGrabBlock(args);
            hasSpawned = true;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        hasSpawned = false;
    }

    private void SpawnAndGrabBlock(SelectEnterEventArgs args)
    {
        Vector3 spawnPosition = spawnPoint.position;
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, transform.rotation);

        XRGrabInteractable newGrabInteractable = newBlock.GetComponent<XRGrabInteractable>();
        if (newGrabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component not found on the spawned block.");
            return;
        }

        args.interactorObject.transform.GetComponent<XRBaseInteractor>().allowSelect = false;
        args.interactorObject.transform.GetComponent<XRBaseInteractor>().allowSelect = true;

        newGrabInteractable.interactionManager.SelectEnter(args.interactorObject, newGrabInteractable);
    }
}