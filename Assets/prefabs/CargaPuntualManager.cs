using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Necesario para usar List

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // Panel del submenú
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Button botonSensor; // Botón para crear sensores (indicadores de fuerza)
    public Transform spawnPoint; // Punto de aparición de las cargas
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa
    public GameObject indicadorFuerzaPrefab; // Prefab del indicador de fuerza

    private List<GameObject> cargas = new List<GameObject>(); // Lista para almacenar las cargas
    private List<GameObject> sensores = new List<GameObject>(); // Lista para almacenar los sensores creados

    private void Start()
    {
        // Ocultar el submenú al inicio
        subMenuCarga.SetActive(false);

        // Asignar funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
        botonSensor.onClick.AddListener(CrearSensor); // Nuevo botón para sensores
    }

    private void Update()
    {
        // Actualizar la dirección y rotación de todos los sensores
        foreach (var sensor in sensores)
        {
            ActualizarSensor(sensor);
        }
    }

    public void MostrarSubMenuCargas()
    {
        subMenuCarga.SetActive(true);
    }

    public void IngresarCargaPositiva()
    {
        CrearCarga(cargaPositivaPrefab, true);
    }

    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab, false);
    }

    private void CrearCarga(GameObject cargaPrefab, bool esPositiva)
    {
        if (cargaPrefab != null && spawnPoint != null)
        {
            GameObject carga = Instantiate(cargaPrefab, spawnPoint.position, Quaternion.identity);
            carga.name = $"Carga {(esPositiva ? "Positiva" : "Negativa")}";

            Carga cargaScript = carga.GetComponent<Carga>();
            if (cargaScript != null)
            {
                cargaScript.esPositiva = esPositiva;
            }

            cargas.Add(carga);
            Debug.Log($"Carga creada: {carga.name}");
        }
        else
        {
            Debug.LogError("Prefab de carga o SpawnPoint no asignados correctamente.");
        }
    }

    private void CrearSensor()
    {
        if (indicadorFuerzaPrefab != null)
        {
            // Crear un nuevo sensor en una posición predeterminada cerca del spawnPoint
            Vector3 posicionSensor = spawnPoint.position + new Vector3(0, 0, sensores.Count * 1.5f); // Espaciar sensores
            GameObject sensor = Instantiate(indicadorFuerzaPrefab, posicionSensor, Quaternion.identity);
            sensor.name = $"Sensor {sensores.Count + 1}";

            sensores.Add(sensor);
            Debug.Log($"Sensor creado: {sensor.name}");
        }
        else
        {
            Debug.LogError("Prefab del indicador de fuerza no asignado.");
        }
    }

    private void ActualizarSensor(GameObject sensor)
    {
        Vector3 fuerzaTotal = CalcularFuerzaTotal(sensor);

        // Acceder al script de IndicadorFuerza para actualizar su dirección
        IndicadorFuerza indicador = sensor.GetComponent<IndicadorFuerza>();
        if (indicador != null)
        {
            indicador.ActualizarDireccion(fuerzaTotal);
        }
    }

    private Vector3 CalcularFuerzaTotal(GameObject sensor)
    {
        Vector3 fuerzaTotal = Vector3.zero;

        foreach (GameObject carga in cargas)
        {
            Vector3 direccion = sensor.transform.position - carga.transform.position;
            float distancia = direccion.magnitude;

            if (distancia > 0.01f) // Evitar divisiones por 0
            {
                direccion.Normalize();
                float magnitudFuerza = 1f / (distancia * distancia);

                Carga cargaScript = carga.GetComponent<Carga>();
                if (cargaScript != null)
                {
                    if (cargaScript.esPositiva)
                    {
                        fuerzaTotal += direccion * magnitudFuerza;
                    }
                    else
                    {
                        fuerzaTotal -= direccion * magnitudFuerza;
                    }
                }
            }
        }

        return fuerzaTotal;
    }
}
