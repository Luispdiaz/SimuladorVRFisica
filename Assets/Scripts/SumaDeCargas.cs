using UnityEngine;

public class SumaDeCargas : MonoBehaviour
{
    public GameObject miniSpherePrefab; // Prefab de la mini esfera
    public int numberOfPoints = 30; // Número de puntos a crear en la línea

    public void CrearSumaDeCargas(Vector3 startPoint, Vector3 endPoint, bool esPositiva)
    {
        // Eliminar mini esferas existentes
        GameObject[] existingSpheres = GameObject.FindGameObjectsWithTag("MiniSphere");
        foreach (GameObject sphere in existingSpheres)
        {
            Destroy(sphere);
        }

        Vector3 direccion = (endPoint - startPoint).normalized;
        float distancia = Vector3.Distance(startPoint, endPoint);

        // Calcular el punto final adicional con doble distancia
        Vector3 nuevoEndPoint = endPoint - direccion * distancia * 2;

        for (int i = 0; i <= numberOfPoints; i++)
        {
            float t = (float)i / numberOfPoints;
            Vector3 pointPosition = Vector3.Lerp(endPoint, nuevoEndPoint, t);
            GameObject miniSphere = Instantiate(miniSpherePrefab, pointPosition, Quaternion.identity);
            Renderer renderer = miniSphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = esPositiva ? Color.red : Color.blue; // Rojo para positiva, azul para negativa
            }
        }
    }
}
