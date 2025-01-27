using UnityEngine;
using System.Collections.Generic;

public class FlechasManager : MonoBehaviour
{
    public GameObject flechaPrefab; // Prefab de la flecha

    private List<GameObject> flechas = new List<GameObject>(); // Lista para almacenar las flechas

    public void CrearFlecha(Vector3 startPoint, Vector3 endPoint, bool esPositiva)
    {
        // Crear una sola flecha en el punto medio entre startPoint y endPoint
        Vector3 pointPosition = (startPoint + endPoint) / 2;
        GameObject flecha = Instantiate(flechaPrefab, pointPosition, Quaternion.identity);
        Flecha flechaScript = flecha.AddComponent<Flecha>();
        flechaScript.ConfigurarFlecha(startPoint, endPoint, esPositiva);
        flechas.Add(flecha);
    }

    public void EliminarFlechas()
    {
        foreach (var flecha in flechas)
        {
            Destroy(flecha);
        }
        flechas.Clear();
    }
}
