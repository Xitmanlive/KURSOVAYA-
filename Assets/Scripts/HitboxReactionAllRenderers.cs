using UnityEngine;

public class HitboxReactionAllRenderers : MonoBehaviour
{
    public GameObject droneRoot;
    public Material hitMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (droneRoot != null && hitMaterial != null)
        {
            Renderer[] renderers = droneRoot.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material = hitMaterial;
            }
        }

        Debug.Log("Сработал хитбокс дрона. Объект касания: " + other.name);
    }
}