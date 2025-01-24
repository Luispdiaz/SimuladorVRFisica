using UnityEngine;

public class LineasPunteadas : MonoBehaviour
{
    public GameObject miniSpherePrefab; // Prefab de la mini esfera
    public int numberOfPoints = 10; // Número de puntos a crear en la línea

    public void CrearLineasPunteadas(Vector3 startPoint, Vector3 endPoint)
    {
        for (int i = 0; i <= numberOfPoints; i++)
        {
            float t = (float)i / numberOfPoints;
            Vector3 pointPosition = Vector3.Lerp(startPoint, endPoint, t);
            Instantiate(miniSpherePrefab, pointPosition, Quaternion.identity);
        }
    }
}
