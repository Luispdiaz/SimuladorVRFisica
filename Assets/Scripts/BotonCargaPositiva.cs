using UnityEngine;

public class BotonCargaPositiva : MonoBehaviour
{
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva
    public Transform spawnPoint; // Punto de aparición

    public void InstanciarCargaPositiva()
    {
        if (cargaPositivaPrefab != null && spawnPoint != null)
        {
            GameObject carga = Instantiate(cargaPositivaPrefab, spawnPoint.position, Quaternion.identity);
            carga.name = "Carga Positiva";
            Debug.Log("Carga positiva creada: " + carga.name);
        }
        else
        {
            Debug.LogError("Prefab de carga positiva o spawnPoint no asignados correctamente.");
        }
    }
}
