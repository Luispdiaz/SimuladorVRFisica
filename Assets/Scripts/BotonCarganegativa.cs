using UnityEngine;

public class BotonCargaNegativa : MonoBehaviour
{
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa
    public Transform spawnPoint; // Punto de aparición

    public void InstanciarCargaNegativa()
    {
        if (cargaNegativaPrefab != null && spawnPoint != null)
        {
            GameObject carga = Instantiate(cargaNegativaPrefab, spawnPoint.position, Quaternion.identity);
            carga.name = "Carga Negativa";
            Debug.Log("Carga negativa creada: " + carga.name);
        }
        else
        {
            Debug.LogError("Prefab de carga negativa o spawnPoint no asignados correctamente.");
        }
    }
}
