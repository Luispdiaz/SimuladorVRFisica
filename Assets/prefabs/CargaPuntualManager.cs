using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // El panel con los botones de carga positiva y negativa
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Transform spawnPoint; // Punto de aparición de las cargas (SpawnPoint)
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva (Building Block esférico)
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa (Building Block esférico)
    public GameObject indicadorFuerzaPrefab; // Prefab del objeto indicador de fuerzas (como una flecha)

    // Lista para almacenar las cargas generadas
    private List<GameObject> cargas = new List<GameObject>();

    private void Start()
    {
        // Inicialmente, el submenú estará oculto
        subMenuCarga.SetActive(false);

        // Asignar funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
    }

    // Método para activar el submenú de cargas (cuando se presiona el botón de "Cargas Puntuales")
    public void MostrarSubMenuCargas()
    {
        subMenuCarga.SetActive(true); // Muestra el submenú con los botones de carga
    }

    // Método para insertar una carga positiva
    public void IngresarCargaPositiva()
    {
        CrearCarga(cargaPositivaPrefab, true); // Crear la carga positiva en el SpawnPoint
    }

    // Método para insertar una carga negativa
    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab, false); // Crear la carga negativa en el SpawnPoint
    }

    // Método que crea una carga (positiva o negativa) en el punto de carga
    private void CrearCarga(GameObject cargaPrefab, bool esPositiva)
    {
        if (cargaPrefab != null && spawnPoint != null)
        {
            // Instanciamos la carga en el SpawnPoint especificado
            GameObject carga = Instantiate(cargaPrefab, spawnPoint.position, Quaternion.identity);
            carga.name = "Carga " + (esPositiva ? "Positiva" : "Negativa") + " " + cargas.Count;

            // Añadir Rigidbody para manejar las fuerzas físicas
            Rigidbody rb = carga.AddComponent<Rigidbody>();
            rb.useGravity = false; // Desactivar gravedad para simular cargas puntuales

            // Configurar el tipo de carga (repulsión o atracción)
            Carga cargaScript = carga.AddComponent<Carga>();
            cargaScript.esPositiva = esPositiva;

            // Añadimos la carga a la lista para futuras referencias
            cargas.Add(carga);

            Debug.Log("Carga creada: " + carga.name);
        }
        else
        {
            Debug.LogError("Prefab de carga o SpawnPoint no asignados correctamente.");
        }
    }

    // Método para actualizar el indicador de fuerza
    public void ActualizarIndicadorFuerza(Vector3 posicionIndicador)
    {
        if (indicadorFuerzaPrefab != null && cargas.Count > 0)
        {
            Vector3 fuerzaTotal = Vector3.zero;

            // Calcular la fuerza neta en el punto del indicador
            foreach (GameObject carga in cargas)
            {
                if (carga != null)
                {
                    Carga cargaScript = carga.GetComponent<Carga>();
                    Vector3 direccion = posicionIndicador - carga.transform.position;
                    float distancia = direccion.magnitude;

                    // Calcular la magnitud de la fuerza (simulación simplificada)
                    float magnitudFuerza = (cargaScript.esPositiva ? 1 : -1) / Mathf.Pow(distancia, 2);

                    // Sumar la fuerza al total
                    fuerzaTotal += direccion.normalized * magnitudFuerza;
                }
            }

            // Instanciar o mover el indicador al punto especificado
            GameObject indicador = Instantiate(indicadorFuerzaPrefab, posicionIndicador, Quaternion.identity);
            indicador.transform.forward = fuerzaTotal.normalized; // Orientar el indicador hacia la dirección de la fuerza
        }
        else
        {
            Debug.LogError("No hay cargas creadas o indicadorFuerzaPrefab no asignado.");
        }
    }
}
