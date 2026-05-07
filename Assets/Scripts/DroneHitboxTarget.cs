using UnityEngine;

public class DroneHitboxTarget : MonoBehaviour
{
    public GameObject droneRoot;
    public Material hitMaterial;
    public int health = 3;

    public void Hit()
    {
        health--;

        Debug.Log("Попадание по дрону. Осталось здоровья: " + health);

        if (droneRoot != null && hitMaterial != null)
        {
            Renderer[] renderers = droneRoot.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material = hitMaterial;
            }
        }

        if (health <= 0)
        {
            Debug.Log("Дрон уничтожен");
        }
    }
}