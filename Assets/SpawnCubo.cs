using UnityEngine;

public class SpawnCubo : MonoBehaviour
{
    public GameObject cuboPrefab; // Prefab del cubo
    public Transform spawnPoint; // Punto donde aparecerá el cubo

    // Método para instanciar el cubo
    public void SpawnCube()
    {
        if (cuboPrefab != null && spawnPoint != null)
        {
            // Instancia el cubo en el punto de aparición
            Instantiate(cuboPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogError("CuboPrefab o SpawnPoint no asignados en el Inspector.");
        }
    }
}