using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class DesactivadorObjetos : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonToggle; // Asignar desde el Inspector
    [SerializeField] private Text textoBoton;   // Texto del botón (opcional)

    [Header("Búsqueda Automática")]
    [SerializeField] private bool usarEtiqueta = true;
    [SerializeField] private string etiqueta = "Desactivable";
    [SerializeField] private bool usarNombre = true;
    [SerializeField] private string[] nombresContener = { "Roof", "Light", "Fan", "Wall" };

    private List<GameObject> objetosTotales = new List<GameObject>();
    private bool objetosActivos = true;

    void Start()
    {
        InicializarLista();
        if (botonToggle != null)
        {
            botonToggle.onClick.AddListener(ToggleObjetos);
            ActualizarTextoBoton();
        }
        else Debug.LogError("¡Asigna el botón en el Inspector!");
    }

    private void InicializarLista()
    {
        objetosTotales.Clear();
        var objetosUnicos = new HashSet<GameObject>();

        // Obtener todos los objetos activos de la escena
        GameObject[] objetosEscena = SceneManager.GetActiveScene()
            .GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<Transform>(true))
            .Where(t => t.gameObject.activeInHierarchy)
            .Select(t => t.gameObject)
            .ToArray();

        // Filtrar por etiqueta
        if (usarEtiqueta)
        {
            foreach (GameObject obj in objetosEscena)
            {
                if (obj.CompareTag(etiqueta)) objetosUnicos.Add(obj);
            }
        }

        // Filtrar por nombres (case-insensitive)
        if (usarNombre)
        {
            foreach (GameObject obj in objetosEscena)
            {
                foreach (string nombre in nombresContener)
                {
                    if (obj.name.IndexOf(nombre, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        objetosUnicos.Add(obj);
                    }
                }
            }
        }

        objetosTotales = objetosUnicos.ToList();
    }

    private void ToggleObjetos()
    {
        objetosActivos = !objetosActivos;
        foreach (GameObject obj in objetosTotales)
        {
            if (obj != null) obj.SetActive(objetosActivos);
        }
        ActualizarTextoBoton();
    }

    private void ActualizarTextoBoton()
    {
        if (textoBoton != null)
        {
            textoBoton.text = objetosActivos ? "Desactivar Todo" : "Activar Todo";
        }
    }

    // Método para actualizar lista dinámicamente (opcional)
    public void ActualizarLista()
    {
        InicializarLista();
    }
}