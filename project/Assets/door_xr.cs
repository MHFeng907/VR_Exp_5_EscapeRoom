using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class XR_OpenCloseDoor : MonoBehaviour
{
    public Animator doorAnimator;
    private bool isOpen = false;

    private void Awake()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OpenOrCloseDoor);
    }

    private void OpenOrCloseDoor(SelectEnterEventArgs args)
    {
        if (isOpen)
        {
            doorAnimator.Play("Closing");
            Debug.Log("you are closing the door");
        }
        else
        {
            doorAnimator.Play("Opening");
            Debug.Log("you are opening the door");
        }

        isOpen = !isOpen;
    }
}
