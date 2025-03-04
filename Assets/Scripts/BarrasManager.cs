using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MenuControl : MonoBehaviour
{
    [Header("Plano Configuration")]
    public Slider planoSlider;
    public GameObject planoPrefab;
    public float offsetPosicion = 15f;

    [Header("Botones Planos")]
    public List<Button> botonesPositivos = new List<Button>();
    public List<Button> botonesNegativos = new List<Button>();

    [Header("Carga Configuration")]
    public Slider horizontalSlider;
    public Slider verticalSlider;
    public Slider velocidadSlider;

    [Header("Spawn Point")]
    public Transform spawnPoint;


    [Header("Referencias Explícitas de Botones")]
    public Button closeButton;
    public GameObject subMenuCarga;  // Asegurar que está asignado en el Inspector
    public GameObject tituloSubMenu; // Asegurar que está asignado en el Inspector

    [Header("Dirección de Lanzamiento")]
    public GameObject direccionIndicatorPrefab; // Prefab con eje X como frente
    public Transform puntoReferenciaLanzamiento; // Punto de origen del lanzamiento
    private GameObject currentIndicator;

    [Header("UI de Ángulos")]
    public TMP_Text textoAnguloHorizontal; // Asignar en Inspector
    public TMP_Text textoAnguloVertical;   // Asignar en Inspector
    public TMP_Text textoAnguloFlecha;     // Asignar en Inspector

    [Header("Velocidad Configuration")]
    public TMP_Text textoVelocidad; // Nuevo texto para mostrar velocidad

    [Header("Botón de Lanzamiento")]
    public Button lanzarCargaButton; // Asignar en Inspector


    // Referencia al manager de cargas (debe asignarse en el Inspector)
    public CargaPuntualManager cargaManager;
    // Nuevo spawn point para mover la carga
    public Transform moverCargaSpawnPoint;
    // Botón que ejecutará la acción
    public Button moverCargaButton;


    private readonly string[] direcciones = { "Arriba", "Abajo", "Izquierda", "Derecha" };

    private void Start()
    {
        subMenuCarga.SetActive(false);
        tituloSubMenu.SetActive(false);
        moverCargaButton.onClick.AddListener(MoverCargaALaPosicionSpawn);
        // Añadir listener al closeButton (redundante por seguridad)
        closeButton.onClick.AddListener(CerrarSubMenu);
        horizontalSlider.onValueChanged.AddListener(UpdateDirectionIndicator);
        verticalSlider.onValueChanged.AddListener(UpdateDirectionIndicator);
        // Configurar sliders con pasos de 0.05
        horizontalSlider.onValueChanged.AddListener(v => horizontalSlider.value = Mathf.Round(v / 0.05f) * 0.05f);
        verticalSlider.onValueChanged.AddListener(v => verticalSlider.value = Mathf.Round(v / 0.05f) * 0.05f);

        // Configurar slider de velocidad con valores enteros
        velocidadSlider.wholeNumbers = true;
        velocidadSlider.minValue = 0;
        velocidadSlider.maxValue = 20;
        velocidadSlider.onValueChanged.AddListener(UpdateVelocidadText);
        UpdateVelocidadText(velocidadSlider.value);
        lanzarCargaButton.onClick.AddListener(LanzarCarga);


    }

    // Actualizar texto de velocidad
    private void UpdateVelocidadText(float value)
    {
        textoVelocidad.text = $"{value:F0} m/s";
    }

    private void UpdateAngleDisplays()
    {
        // Ángulos de los sliders
        float anguloHorizontal = Mathf.Lerp(0f, 180f, horizontalSlider.value);
        float anguloVertical = Mathf.Lerp(0f, 180f, verticalSlider.value);

        // Formatear textos
        textoAnguloHorizontal.text = $"Horizontal: {anguloHorizontal:F0}°";
        textoAnguloVertical.text = $"Vertical: {anguloVertical:F0}°";


        // Calcular dirección usando el FORWARD de la flecha
        Vector3 direccionFlecha = currentIndicator.transform.forward;
        float anguloFlecha = Mathf.Atan2(direccionFlecha.z, direccionFlecha.x) * Mathf.Rad2Deg;

        // Ajustar ángulo a rango 0-360
        if (anguloFlecha < 0) anguloFlecha += 360f;

        textoAnguloFlecha.text = $"Total: {anguloFlecha:F0}°";
    }


    public void CerrarSubMenu()
    {
        Debug.Log("Cerrando submenú (BarrasManager)");
        if (subMenuCarga == null) Debug.LogError("subMenuCarga no asignado en BarrasManager");
        if (tituloSubMenu == null) Debug.LogError("tituloSubMenu no asignado en BarrasManager");

        if (subMenuCarga != null) subMenuCarga.SetActive(false);
        if (tituloSubMenu != null) tituloSubMenu.SetActive(false);
        if (currentIndicator != null) Destroy(currentIndicator);
    }

    Vector3 CalcularDireccion()
    {
        return Quaternion.Euler(verticalSlider.value, horizontalSlider.value, 0) * Vector3.forward;
    }

    private void MoverCargaALaPosicionSpawn()
    {
        if (cargaManager == null)
        {
            Debug.LogError("No se ha asignado el CargaPuntualManager.");
            return;
        }

        // Obtener la lista de cargas desde el manager
        List<GameObject> cargas = cargaManager.Cargas;
        if (cargas == null || cargas.Count == 0)
        {
            Debug.Log("No existen cargas para mover.");
            return;
        }

        // Buscar la carga más cercana al spawn point nuevo
        GameObject cargaCercana = null;
        float minDistancia = Mathf.Infinity;
        foreach (GameObject carga in cargas)
        {
            float distancia = Vector3.Distance(carga.transform.position, moverCargaSpawnPoint.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                cargaCercana = carga;
            }
        }

        if (cargaCercana != null)
        {
            cargaCercana.transform.position = moverCargaSpawnPoint.position;
            Debug.Log("Carga movida a la posición del nuevo spawn point.");
        }
        else
        {
            Debug.Log("No se encontró ninguna carga cercana al spawn point.");
        }
    }
    private void UpdateDirectionIndicator(float value)
    {
        if (currentIndicator == null)
        {
            currentIndicator = Instantiate(direccionIndicatorPrefab, puntoReferenciaLanzamiento.position, Quaternion.identity);
        }

        // Ajustar ángulos con pasos de 0.05 (sin cambios)
        float verticalValue = Mathf.Round(verticalSlider.value / 0.05f) * 0.05f;
        float horizontalValue = Mathf.Round(horizontalSlider.value / 0.05f) * 0.05f;

        // 1. Invertir solo el vertical (0° = arriba, 180° = abajo)
        float verticalAngle = Mathf.Lerp(180f, 0f, verticalValue); // Único cambio necesario

        // 2. Rotación horizontal se mantiene igual
        float horizontalAngle = Mathf.Lerp(0f, 180f, horizontalValue);

        // 3. Aplicar rotaciones (misma lógica original)
        Quaternion baseRotation = Quaternion.Euler(0, -90f, 0);
        Quaternion rotVertical = Quaternion.Euler(0, 0, verticalAngle); // Eje Z (para mantener compatibilidad)
        Quaternion rotHorizontal = Quaternion.Euler(0, horizontalAngle, 0);

        currentIndicator.transform.rotation = baseRotation * rotHorizontal * rotVertical;
        currentIndicator.transform.position = puntoReferenciaLanzamiento.position;
        UpdateAngleDisplays();
    }

    public void LanzarCarga()
    {
        if (currentIndicator == null || cargaManager == null) return;

        GameObject carga = cargaManager.Cargas.Count > 0 ? cargaManager.Cargas[^1] : null;

        if (carga != null)
        {
            // Añadir componente EstelaCarga si no existe
            EstelaCarga estelaScript = carga.GetComponent<EstelaCarga>();
            if (estelaScript == null)
            {
                estelaScript = carga.AddComponent<EstelaCarga>();
                estelaScript.miniSpherePrefab = cargaManager.miniSpherePrefab; // Asignar prefab desde el manager
            }
            estelaScript.DetenerEstela(); // Limpiar estela previa
            estelaScript.IniciarEstela();

            Rigidbody rb = carga.GetComponent<Rigidbody>();
            if (rb == null) rb = carga.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            // 1. Obtener dirección inicial del indicador
            float anguloHorizontal = Mathf.Lerp(-90f, 90f, horizontalSlider.value) - 90f;
            float anguloVertical = Mathf.Lerp(90f, -90f, verticalSlider.value);
            Vector3 direccion = Quaternion.Euler(anguloVertical, anguloHorizontal, 0) * Vector3.forward;

            // 2. Aplicar fuerza inicial
            float velocidad = velocidadSlider.value;
            rb.AddForce(direccion * velocidad, ForceMode.VelocityChange);

            // 3. Activar el movimiento controlado
            Carga scriptCarga = carga.GetComponent<Carga>();
            if (scriptCarga != null)
            {
                scriptCarga.IniciarMovimiento();
            }
        }
    }
}
