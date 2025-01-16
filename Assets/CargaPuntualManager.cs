using UnityEngine;
using UnityEngine.UI;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // El panel con los botones de carga positiva y negativa
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Transform spawnPoint; // Punto de aparición de las cargas (SpawnPoint)
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva (Building Block esférico)
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa (Building Block esférico)

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

            // Aquí puedes agregar más lógica para manipular la carga si es necesario
            Debug.Log("Carga creada: " + carga.name);
        }
        else
        {
            Debug.LogError("Prefab de carga o SpawnPoint no asignados correctamente.");
        }
    }
}