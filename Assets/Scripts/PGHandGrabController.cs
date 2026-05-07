using UnityEngine;


public class PGHandGrabController : MonoBehaviour
{
    [Header("References")]
    public Transform rightHand;
    public Transform leftHand;

    [Header("Hand movement")]
    public float handMoveSpeed = 0.8f;
    public float grabRadius = 0.45f;

    [Header("Default hand positions")]
    public Vector3 rightHandDefaultPosition = new Vector3(0.35f, -0.35f, 0.75f);
    public Vector3 leftHandDefaultPosition = new Vector3(-0.35f, -0.35f, 0.75f);

    private Transform activeHand;
    private Rigidbody grabbedRigidbody;
    private Transform grabbedObject;
    private bool usingRightHand = true;

    private void Start()
    {
        if (rightHand != null)
        {
            rightHand.localPosition = rightHandDefaultPosition;
        }

        if (leftHand != null)
        {
            leftHand.localPosition = leftHandDefaultPosition;
        }

        activeHand = rightHand;
    }

    private void Update()
    {
        SwitchHand();
        MoveActiveHand();
        GrabOrRelease();
    }
    private void SwitchHand()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            usingRightHand = !usingRightHand;
            activeHand = usingRightHand ? rightHand : leftHand;

            Debug.Log(usingRightHand ? "Активна правая рука" : "Активна левая рука");
        }
    }

    private void MoveActiveHand()
    {
        if (activeHand == null)
        {
            return;
        }

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.J))
        {
            move.x -= 1f;
        }

        if (Input.GetKey(KeyCode.L))
        {
            move.x += 1f;
        }

        if (Input.GetKey(KeyCode.I))
        {
            move.y += 1f;
        }

        if (Input.GetKey(KeyCode.K))
        {
            move.y -= 1f;
        }

        if (Input.GetKey(KeyCode.U))
        {
            move.z -= 1f;
        }

        if (Input.GetKey(KeyCode.O))
        {
            move.z += 1f;
        }

        activeHand.localPosition += move * handMoveSpeed * Time.deltaTime;

        activeHand.localPosition = new Vector3(
            Mathf.Clamp(activeHand.localPosition.x, -1.2f, 1.2f),
            Mathf.Clamp(activeHand.localPosition.y, -0.8f, 0.5f),
            Mathf.Clamp(activeHand.localPosition.z, 0.2f, 1.5f)
        );
    }

    private void GrabOrRelease()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grabbedObject == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }
    }
    private void TryGrab()
    {
        if (activeHand == null)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(activeHand.position, grabRadius);

        Rigidbody nearestRigidbody = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();

            if (rb == null)
            {
                continue;
            }

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = rb.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (grabInteractable == null)
            {
                continue;
            }

            float distance = Vector3.Distance(activeHand.position, rb.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestRigidbody = rb;
            }
        }

        if (nearestRigidbody == null)
        {
            Debug.Log("Рядом нет предмета для захвата");
            return;
        }

        grabbedRigidbody = nearestRigidbody;
        grabbedObject = grabbedRigidbody.transform;

        grabbedRigidbody.useGravity = false;
        grabbedRigidbody.isKinematic = true;

        bool isPistol =
            grabbedObject.name.ToLower().Contains("pistol") ||
            grabbedObject.name.ToLower().Contains("futuristic");

        if (isPistol)
        {
            Transform holdPoint = activeHand.Find("PistolHoldPoint");

            if (holdPoint != null)
            {
                grabbedObject.SetParent(holdPoint, false);
                grabbedObject.localPosition = Vector3.zero;
                grabbedObject.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("PistolHoldPoint не найден внутри активной руки.");
                grabbedObject.SetParent(activeHand, false);
                grabbedObject.localPosition = new Vector3(0f, 0f, 0.25f);
                grabbedObject.localRotation = Quaternion.identity;
            }
        }
        else
        {
            grabbedObject.SetParent(activeHand, false);
            grabbedObject.localPosition = new Vector3(0f, 0f, 0.25f);
            grabbedObject.localRotation = Quaternion.identity;
        }

        grabbedObject.SendMessage("OnPCGrabbed", SendMessageOptions.DontRequireReceiver);

        Debug.Log("Предмет взят: " + grabbedObject.name);
    }
    private void Release()
    {
        if (grabbedObject == null)
        {
            return;
        }

        grabbedObject.SendMessage("OnPCReleased", SendMessageOptions.DontRequireReceiver);

        grabbedObject.SetParent(null);

        if (grabbedRigidbody != null)
        {
            grabbedRigidbody.isKinematic = false;
            grabbedRigidbody.useGravity = true;
        }

        Debug.Log("Предмет отпущен");

        grabbedObject = null;
        grabbedRigidbody = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        if (rightHand != null)
        {
            Gizmos.DrawWireSphere(rightHand.position, grabRadius);
        }

        Gizmos.color = Color.green;

        if (leftHand != null)
        {
            Gizmos.DrawWireSphere(leftHand.position, grabRadius);
        }
    }
}