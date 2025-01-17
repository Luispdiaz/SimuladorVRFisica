using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Necesario para usar List

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

    private List<GameObject> cargas = new List<GameObject>(); // Lista para almacenar las cargas
    private GameObject indicadorActual; // Referencia al indicador de fuerza instanciado

    private void Start()
    {
        // Inicialmente, el submenú estará oculto
        subMenuCarga.SetActive(false);

        // Asignar funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);

        // Crear el indicador de fuerza en la posición deseada
        CrearIndicador();
    }

    private void Update()
    {
        // Actualizar la dirección del indicador de fuerza en cada frame
        ActualizarIndicador();
    }

    // Método para activar el submenú de cargas (cuando se presiona el botón de "Cargas Puntuales")
    public void MostrarSubMenuCargas()
    {
        subMenuCarga.SetActive(true); // Muestra el submenú con los botones de carga
    }

    // Método para insertar una carga positiva
    public void IngresarCargaPositiva()
    {
        CrearCarga(cargaPositivaPrefab); // Crear la carga positiva en el SpawnPoint
    }

    // Método para insertar una carga negativa
    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab); // Crear la carga negativa en el SpawnPoint
    }

    // Método que crea una carga (positiva o negativa) en el punto de carga
    private void CrearCarga(GameObject cargaPrefab)
    {
        if (cargaPrefab != null && spawnPoint != null)
        {
            // Instanciamos la carga en el SpawnPoint especificado
            GameObject carga = Instantiate(cargaPrefab, spawnPoint.position, Quaternion.identity);
            carga.name = "Carga " + cargaPrefab.name;

            // Agregar la carga a la lista de cargas
            cargas.Add(carga);

            Debug.Log("Carga creada: " + carga.name);
        }
        else
        {
            Debug.LogError("Prefab de carga o SpawnPoint no asignados correctamente.");
        }
    }

    // Método para crear el indicador de fuerza
    private void CrearIndicador()
    {
        if (indicadorFuerzaPrefab != null)
        {
            // Instanciar el indicador en la posición fija
            indicadorActual = Instantiate(indicadorFuerzaPrefab, new Vector3(0.1f, 1.2f, 0.9f), Quaternion.identity);
            indicadorActual.name = "Indicador de Fuerza";

            // Asegurarnos de que no se mueva
            Rigidbody rb = indicadorActual.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Debug.Log("Indicador de fuerza creado.");
        }
        else
        {
            Debug.LogError("Prefab del indicador de fuerza no asignado.");
        }
    }

    // Método para calcular la dirección de la fuerza y rotar el indicador
    private void ActualizarIndicador()
    {
        if (indicadorActual != null)
        {
            Vector3 fuerzaTotal = Vector3.zero;

            foreach (GameObject carga in cargas)
            {
                // Calcula la dirección de la fuerza desde la carga hacia el indicador
                Vector3 direccion = indicadorActual.transform.position - carga.transform.position;
                float distancia = direccion.magnitude;

                direccion.Normalize();
                float magnitudFuerza = 1f / (distancia * distancia);

                // Aplica la fuerza dependiendo del tipo de carga
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

            // Actualiza la rotación del indicador
            if (fuerzaTotal != Vector3.zero)
            {
                // Calcula la rotación que hará que el indicador apunte en la dirección de la fuerza total
                Quaternion rotacion = Quaternion.LookRotation(fuerzaTotal, Vector3.up);

                // Ajusta la rotación para que apunte correctamente según la orientación de tu flecha
                indicadorActual.transform.rotation = rotacion;
            }
        }
    }
}
