using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabLightReaction : MonoBehaviour
{
    public Light reactionLight;

    public Color normalColor = Color.yellow;
    public Color grabbedColor = Color.cyan;

    public float normalIntensity = 0.3f;
    public float grabbedIntensity = 5f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        ApplyGrabbedState();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        ApplyNormalState();
    }

    public void OnPCGrabbed()
    {
        ApplyGrabbedState();
    }

    public void OnPCReleased()
    {
        ApplyNormalState();
    }

    private void ApplyGrabbedState()
    {
        if (reactionLight != null)
        {
            reactionLight.color = grabbedColor;
            reactionLight.intensity = grabbedIntensity;
        }
    }

    private void ApplyNormalState()
    {
        if (reactionLight != null)
        {
            reactionLight.color = normalColor;
            reactionLight.intensity = normalIntensity;
        }
    }
}