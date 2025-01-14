using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject cuboPrefab; // Prefab del cubo que se va a instanciar.
    public Transform spawnPoint; // Punto donde aparecerán los objetos.

    // Lista para almacenar los objetos instanciados desde el menú.
    private List<GameObject> objetosInstanciados = new List<GameObject>();

    // Método para spawnear un cubo.
    public void SpawnCubo()
    {
        if (cuboPrefab != null && spawnPoint != null)
        {
            // Crear el cubo en el punto de spawn.
            GameObject nuevoCubo = Instantiate(cuboPrefab, spawnPoint.position, Quaternion.identity);

            // Añadir el cubo a la lista de objetos instanciados.
            objetosInstanciados.Add(nuevoCubo);
        }
        else
        {
            Debug.LogWarning("Cubo Prefab o Spawn Point no está configurado.");
        }
    }

    // Método para limpiar todos los objetos instanciados desde el menú.
    public void LimpiarObjetos()
    {
        // Recorrer la lista de objetos instanciados y destruir cada uno.
        foreach (GameObject objeto in objetosInstanciados)
        {
            if (objeto != null)
            {
                Destroy(objeto);
            }
        }

        // Limpiar la lista.
        objetosInstanciados.Clear();
    }
}