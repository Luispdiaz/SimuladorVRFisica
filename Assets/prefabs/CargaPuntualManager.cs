using UnityEngine;
using UnityEngine.UI; // Asegúrate de importar este espacio de nombres
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
    public GameObject popupFuerza; // Panel del popup para ajustar la fuerza
    public GameObject inputFuerzaObject; // Campo de entrada para la fuerza como GameObject
    public Button confirmarFuerzaBtn; // Botón para confirmar el cambio de fuerza

    private InputField inputFuerza; // Campo de entrada para la fuerza
    private List<GameObject> cargas = new List<GameObject>(); // Lista para almacenar las cargas
    private List<GameObject> sensores = new List<GameObject>(); // Lista para almacenar los sensores creados
    private int cargaSeleccionada = -1; // Índice de la carga seleccionada

    private XRGrabInteractable grabbableObject; // Para detectar el objeto agarrado

    private void Start()
    {
        // Obtener el componente InputField del GameObject asignado
        if (inputFuerzaObject != null)
        {
            inputFuerza = inputFuerzaObject.GetComponent<InputField>();
        }
        else
        {
            Debug.LogError("InputFuerzaObject no asignado correctamente en el Inspector.");
        }

        // Ocultar el submenú y el popup al inicio
        subMenuCarga.SetActive(false);
        popupFuerza.SetActive(false);

        // Asignar funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
        botonSensor.onClick.AddListener(CrearSensor); // Nuevo botón para sensores
        closeButton.onClick.AddListener(CerrarSubMenu); // Botón para cerrar el submenú
        confirmarFuerzaBtn.onClick.AddListener(ConfirmarCambioFuerza); // Botón para confirmar el cambio de fuerza
    }

    private void Update()
    {
        // Detectar la selección de cargas con el mando izquierdo (botón A) mientras se agarra un objeto
        if (Input.GetButtonDown("Fire1")) // Asegúrate de que "Fire1" esté correctamente mapeado al botón A
        {
            DetectarCargaAgarrada();
        }

        // Actualizar la dirección y rotación de todos los sensores
        foreach (var sensor in sensores)
        {
            ActualizarSensor(sensor);
        }
    }

    private void DetectarCargaAgarrada()
    {
        // Obtener el objeto que se está agarrando
        if (grabbableObject != null)
        {
            GameObject carga = grabbableObject.gameObject;
            Debug.Log("Objeto agarrado detectado: " + carga.name); // Añadir Debug.Log para ver qué objeto fue detectado

            if (carga.CompareTag("Destructible"))
            {
                cargaSeleccionada = cargas.IndexOf(carga);
                Debug.Log("Carga seleccionada: " + carga.name); // Añadir Debug.Log para ver qué carga fue seleccionada

                if (cargaSeleccionada >= 0)
                {
                    MostrarPopupFuerza();
                }
            }
        }
    }

    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        var grabbable = interactor.GetOldestInteractableSelected();
        if (grabbable is XRGrabInteractable interactable)
        {
            grabbableObject = interactable;
        }
    }

    public void OnSelectExit(XRBaseInteractor interactor)
    {
        var grabbable = interactor.GetOldestInteractableSelected();
        if (grabbable is XRGrabInteractable interactable && grabbableObject == interactable)
        {
            grabbableObject = null;
        }
    }

    private void MostrarPopupFuerza()
    {
        if (cargaSeleccionada >= 0 && cargaSeleccionada < cargas.Count)
        {
            Carga cargaScript = cargas[cargaSeleccionada].GetComponent<Carga>();
            if (cargaScript != null)
            {
                inputFuerza.text = cargaScript.fuerza.ToString();
                popupFuerza.SetActive(true);
            }
        }
    }

    private void ConfirmarCambioFuerza()
    {
        if (cargaSeleccionada >= 0 && cargaSeleccionada < cargas.Count)
        {
            Carga cargaScript = cargas[cargaSeleccionada].GetComponent<Carga>();
            if (cargaScript != null)
            {
                float nuevaFuerza;
                if (float.TryParse(inputFuerza.text, out nuevaFuerza))
                {
                    cargaScript.fuerza = nuevaFuerza;
                }
            }
        }

        popupFuerza.SetActive(false);
    }

    // Métodos existentes no modificados
    public void MostrarSubMenuCargas() { /*...*/ }
    public void CerrarSubMenu() { /*...*/ }
    public void IngresarCargaPositiva() { /*...*/ }
    public void IngresarCargaNegativa() { /*...*/ }
    private void CrearCarga(GameObject cargaPrefab, bool esPositiva) { /*...*/ }
    private void CrearSensor() { /*...*/ }
    private void ActualizarSensor(GameObject sensor) { /*...*/ }
    private Vector3 CalcularFuerzaTotal(GameObject sensor) { /*...*/ }
}
