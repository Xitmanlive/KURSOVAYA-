using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PistolShooter : MonoBehaviour
{
    public Transform muzzle;
    public float shootDistance = 50f;
    public float fireCooldown = 0.25f;
    public LineRenderer shotLine;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isHeld;
    private float nextFireTime;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnXRGrabbed);
            grabInteractable.selectExited.AddListener(OnXRReleased);
            grabInteractable.activated.AddListener(OnXRActivated);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnXRGrabbed);
            grabInteractable.selectExited.RemoveListener(OnXRReleased);
            grabInteractable.activated.RemoveListener(OnXRActivated);
        }
    }

    private void Update()
    {
        if (isHeld && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void OnXRGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    private void OnXRReleased(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    private void OnXRActivated(ActivateEventArgs args)
    {
        Shoot();
    }

    public void OnPCGrabbed()
    {
        isHeld = true;
    }

    public void OnPCReleased()
    {
        isHeld = false;
    }

    public void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireCooldown;

        if (muzzle == null)
        {
            Debug.LogWarning("У пистолета не назначен Muzzle");
            return;
        }

        Vector3 endPoint = muzzle.position + muzzle.forward * shootDistance;

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, shootDistance, ~0, QueryTriggerInteraction.Collide))
        {
            endPoint = hit.point;

            DroneHitboxTarget target = hit.collider.GetComponent<DroneHitboxTarget>();

            if (target == null)
                target = hit.collider.GetComponentInParent<DroneHitboxTarget>();

            if (target != null)
                target.Hit();

            Debug.Log("Выстрел попал в: " + hit.collider.name);
        }
        else
        {
            Debug.Log("Выстрел мимо");
        }

        DrawShotLine(endPoint);
    }

    private void DrawShotLine(Vector3 endPoint)
    {
        if (shotLine == null)
            return;

        shotLine.enabled = true;
        shotLine.positionCount = 2;
        shotLine.SetPosition(0, muzzle.position);
        shotLine.SetPosition(1, endPoint);

        CancelInvoke(nameof(HideShotLine));
        Invoke(nameof(HideShotLine), 0.05f);
    }

    private void HideShotLine()
    {
        if (shotLine != null)
        {
            shotLine.enabled = false;
        }
    }
}