using Oculus.Interaction;
using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    public bool isGrabbed = false;
    public IInteractable[] interactable;
    void Start()
    {
        interactable = GetComponentsInChildren<IInteractable>();    
    }
    void Update()
    {
        for(int i = 0; i < interactable.Length; i++)
        {
            if(interactable[i].State == InteractableState.Select)
            {
                isGrabbed = true;
                break;
            }
            if(i == interactable.Length - 1)
            {
                isGrabbed = false; 
            }
        }
    }
}
