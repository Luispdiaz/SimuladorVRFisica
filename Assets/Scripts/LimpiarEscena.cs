using UnityEngine;

public class ObjectCleaner : MonoBehaviour
{
    public CargaPuntualManager cargaPuntualManager; // Referencia al script CargaPuntualManager

    public void CleanUpScene()
    {
        // Encuentra todos los objetos con la etiqueta "Destructible"
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Destructible");

        // Destruye cada objeto encontrado
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

        // Encuentra todos los objetos con la etiqueta "MiniSphere"
        GameObject[] miniSpheresToDestroy = GameObject.FindGameObjectsWithTag("MiniSphere");

        // Destruye cada mini esfera encontrada
        foreach (GameObject miniSphere in miniSpheresToDestroy)
        {
            Destroy(miniSphere);
        }

        // Actualizar las referencias en CargaPuntualManager después de limpiar
        if (cargaPuntualManager != null)
        {
            cargaPuntualManager.ActualizarReferencias();
        }
    }
}
