using UnityEngine;

public class BotonSensor : MonoBehaviour
{
    public GameObject indicadorFuerzaPrefab; // Prefab del indicador de fuerza
    public Transform spawnPoint; // Punto fijo donde aparece el sensor
    private int contadorSensores = 0; // Para nombrar sensores únicos

    public void InstanciarSensor()
    {
        if (indicadorFuerzaPrefab != null && spawnPoint != null)
        {
            // Siempre instancia el sensor en el spawnPoint
            GameObject sensor = Instantiate(indicadorFuerzaPrefab, spawnPoint.position, Quaternion.identity);
            sensor.name = "Sensor " + (++contadorSensores);
            Debug.Log("Sensor creado en posición fija: " + sensor.name);
        }
        else
        {
            Debug.LogError("Prefab del indicador de fuerza o spawnPoint no asignados correctamente.");
        }
    }
}
