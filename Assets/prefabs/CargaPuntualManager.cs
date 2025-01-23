using UnityEngine;
using UnityEngine.UI; // Asegúrate de importar este espacio de nombres
using System.Collections.Generic;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // Panel del submenú
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Button botonSensor; // Botón para crear sensores (indicadores de fuerza)
    public Button closeButton; // Botón para cerrar el submenú
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
        closeButton.onClick.AddListener(CerrarSubMenu); // Botón para cerrar el submenú
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

    public void CerrarSubMenu()
    {
        subMenuCarga.SetActive(false);
    }

    public void IngresarCargaPositiva()
    {
        CrearCarga(cargaPositivaPrefab, 1.0f, true);
    }

    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab, -1.0f, false);
    }

    private void CrearCarga(GameObject cargaPrefab, float fuerza, bool esPositiva)
    {
        GameObject nuevaCarga = Instantiate(cargaPrefab, spawnPoint.position, spawnPoint.rotation);
        Carga cargaScript = nuevaCarga.AddComponent<Carga>();
        cargaScript.fuerza = fuerza;
        cargaScript.esPositiva = esPositiva;
        cargas.Add(nuevaCarga);
    }

    private void CrearSensor()
    {
        GameObject nuevoSensor = Instantiate(indicadorFuerzaPrefab, spawnPoint.position, spawnPoint.rotation);
        IndicadorFuerza indicadorScript = nuevoSensor.AddComponent<IndicadorFuerza>();
        ActualizarSensor(nuevoSensor);
        sensores.Add(nuevoSensor);
    }

    private void ActualizarSensor(GameObject sensor)
    {
        IndicadorFuerza indicadorScript = sensor.GetComponent<IndicadorFuerza>();
        if (indicadorScript != null)
        {
            Vector3 fuerzaTotal = CalcularFuerzaTotal(sensor.transform.position);
            indicadorScript.ActualizarDireccion(fuerzaTotal);
        }
    }

    private Vector3 CalcularFuerzaTotal(Vector3 posicionSensor)
    {
        Vector3 fuerzaTotal = Vector3.zero;
        foreach (GameObject carga in cargas)
        {
            Carga cargaScript = carga.GetComponent<Carga>();
            if (cargaScript != null)
            {
                Vector3 direccion = posicionSensor - carga.transform.position;
                float distancia = direccion.magnitude;
                if (distancia > 0.01f) // Evitar división por cero
                {
                    Vector3 fuerza = (cargaScript.fuerza / Mathf.Pow(distancia, 2)) * direccion.normalized;
                    fuerzaTotal += fuerza;
                }
            }
        }
        return fuerzaTotal;
    }
}
