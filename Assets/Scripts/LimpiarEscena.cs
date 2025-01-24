using UnityEngine;

public class ObjectCleaner : MonoBehaviour
{
    public void CleanUpScene()
    {
        // Encuentra todos los objetos con la etiqueta "Destructible"
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Destructible");

        // Destruye cada objeto encontrado
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }
}
