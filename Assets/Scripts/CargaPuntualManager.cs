using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // Panel del submenú
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Button botonSensor; // Botón para crear sensores (indicadores de fuerza)
    public Button sumaDeCargasButton; // Botón para crear la suma de cargas
    public Button closeButton; // Botón para cerrar el submenú
    public Button recalcularButton; // Botón para recalcular las mini esferas
    public Button insertarLineaPositivaBtn; // Botón para línea positiva
    public Button insertarLineaNegativaBtn; // Botón para línea negativa
    public Transform spawnPoint; // Punto de aparición de las cargas e indicadores
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa
    public GameObject indicadorFuerzaPrefab; // Prefab del indicador de fuerza
    public GameObject miniSpherePrefab; // Prefab de la mini esfera para las líneas punteadas
    public Slider fuerzaSlider; // Slider para ajustar la fuerza de las cargas
    public Text fuerzaText; // Texto para mostrar la fuerza actual del slider
    public GameObject lineaCargaPositivaPrefab;
    public GameObject lineaCargaNegativaPrefab;

    private List<GameObject> lineasCarga = new List<GameObject>(); // Lista de líneas de carga
    private List<GameObject> cargas = new List<GameObject>(); // Lista para almacenar las cargas
    private List<GameObject> sensores = new List<GameObject>(); // Lista para almacenar los sensores creados
    private LineasPunteadas lineasPunteadas;
    private SumaDeCargas sumaDeCargas; // Nueva referencia para SumaDeCargas

    private void Start()
    {
        // Ocultar el submenú al inicio
        subMenuCarga.SetActive(false);

        // Asignar funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
        botonSensor.onClick.AddListener(CrearSensor);
        sumaDeCargasButton.onClick.AddListener(CrearSumaDeCargas); // Asignar la función de suma de cargas
        closeButton.onClick.AddListener(CerrarSubMenu);
        recalcularButton.onClick.AddListener(RecalcularLineasPunteadas); // Asignar la función de recalcular
        insertarLineaPositivaBtn.onClick.AddListener(IngresarLineaPositiva);
        insertarLineaNegativaBtn.onClick.AddListener(IngresarLineaNegativa);

        // Asignar función al slider
        fuerzaSlider.onValueChanged.AddListener(ActualizarTextoFuerza);

        // Encontrar el texto hijo del slider e inicializarlo
        fuerzaText = fuerzaSlider.GetComponentInChildren<Text>();
        if (fuerzaText == null)
        {
            Debug.LogError("Text component not found inside the Slider.");
            return;
        }

        ActualizarTextoFuerza(fuerzaSlider.value); // Actualizar el texto al inicio

        // Inicializar LineasPunteadas y SumaDeCargas
        lineasPunteadas = GetComponent<LineasPunteadas>();
        if (lineasPunteadas == null)
        {
            lineasPunteadas = gameObject.AddComponent<LineasPunteadas>();
        }
        lineasPunteadas.miniSpherePrefab = miniSpherePrefab;

        sumaDeCargas = GetComponent<SumaDeCargas>();
        if (sumaDeCargas == null)
        {
            sumaDeCargas = gameObject.AddComponent<SumaDeCargas>();
        }
    }

    private void Update()
    {
        // Actualizar la dirección y rotación de todos los sensores
        foreach (var sensor in sensores)
        {
            ActualizarSensor(sensor);
        }

        // Actualizar la fuerza mostrada de todas las cargas
        foreach (var carga in cargas)
        {
            ActualizarTextoFuerzaCarga(carga);
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
        CrearCarga(cargaPositivaPrefab, fuerzaSlider.value, true);
    }

    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab, fuerzaSlider.value, false);
    }

    public void IngresarLineaPositiva()
    {
        CrearLineaCarga(lineaCargaPositivaPrefab, true);
    }

    public void IngresarLineaNegativa()
    {
        CrearLineaCarga(lineaCargaNegativaPrefab, false);
    }

    private void CrearLineaCarga(GameObject prefab, bool esPositiva)
    {
        GameObject nuevaLinea = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        LineaCarga script = nuevaLinea.AddComponent<LineaCarga>();
        script.esPositiva = esPositiva;
        lineasCarga.Add(nuevaLinea);
    }

    private void CrearCarga(GameObject cargaPrefab, float fuerza, bool esPositiva)
    {
        Debug.Log($"Creando carga con fuerza: {fuerza}"); // Registro de valor de fuerza
        GameObject nuevaCarga = Instantiate(cargaPrefab, spawnPoint.position, spawnPoint.rotation);
        Carga cargaScript = nuevaCarga.AddComponent<Carga>();
        cargaScript.fuerza = fuerza;
        cargaScript.esPositiva = esPositiva;
        cargas.Add(nuevaCarga);

        // Crear flecha para la nueva carga
    }

    private void CrearSensor()
    {
        Debug.Log("Creando sensor");
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
                    float fuerzaMagnitud = cargaScript.fuerza / Mathf.Pow(distancia, 2);
                    if (!cargaScript.esPositiva)
                    {
                        fuerzaMagnitud = -fuerzaMagnitud; // Invertir la fuerza para cargas negativas
                    }
                    Vector3 fuerza = fuerzaMagnitud * direccion.normalized;
                    fuerzaTotal += fuerza;
                }
            }
        }
        // Fuerza de líneas de carga
        foreach (GameObject linea in lineasCarga)
        {
            LineaCarga scriptLinea = linea.GetComponent<LineaCarga>();
            Vector3 fuerzaLinea = CalcularFuerzaLinea(linea, posicionSensor, scriptLinea);
            fuerzaTotal += fuerzaLinea;
        }
        return fuerzaTotal;
    }

    private Vector3 CalcularFuerzaLinea(GameObject linea, Vector3 posicionSensor, LineaCarga scriptLinea)
    {
        // Dirección perpendicular más cercana a la línea
        Vector3 puntoMasCercano = linea.GetComponent<Collider>().ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoMasCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f) return Vector3.zero;

        // Campo eléctrico de línea infinita: E = (λ / (2πε₀r)) -> Simplificamos λ/(2πε₀) como "densidadCarga"
        float magnitud = scriptLinea.densidadCarga / distancia;
        if (!scriptLinea.esPositiva) magnitud *= -1;

        return magnitud * direccion.normalized;
    }

    private void ActualizarTextoFuerza(float nuevaFuerza)
    {
        if (fuerzaText != null)
        {
            fuerzaText.text = "Fuerza: " + nuevaFuerza.ToString("F2");
        }
        else
        {
            Debug.LogWarning("fuerzaText is null. Cannot update text.");
        }
    }

    private void ActualizarTextoFuerzaCarga(GameObject carga)
    {
        var cargaScript = carga.GetComponent<Carga>();
        var textComponent = carga.GetComponentInChildren<Text>();
        if (cargaScript != null && textComponent != null)
        {
            textComponent.text = cargaScript.fuerza.ToString("F2");
        }
    }


    private void RecalcularLineasPunteadas()
    {
        lineasPunteadas.EliminarTodasLasLineas();

        foreach (var sensor in sensores)
        {
            // Líneas entre cargas puntuales y sensores
            foreach (var carga in cargas)
            {
                lineasPunteadas.CrearLineasPunteadas(carga.transform, sensor.transform);
            }

            // Líneas entre líneas de carga y sensores
            foreach (var linea in lineasCarga)
            {
                lineasPunteadas.CrearLineasPunteadas(linea.transform, sensor.transform);
            }
        }
    }



    public void CrearSumaDeCargas()
    {
        // Update the list of sensors in SumaDeCargas
        sumaDeCargas.sensores = sensores;

        // Create or update arrows for all charges
        foreach (var carga in cargas)
        {
            sumaDeCargas.CrearOActualizarFlechaParaCarga(carga);
        }
    }
    // Método para eliminar una carga
    public void EliminarCarga(GameObject carga)
    {
        // Eliminar líneas asociadas
        lineasPunteadas.EliminarLineasDeCarga(carga.transform);

        cargas.Remove(carga);
        if (sumaDeCargas.flechasPorCarga.TryGetValue(carga, out var flecha))
        {
            Destroy(flecha);
            sumaDeCargas.flechasPorCarga.Remove(carga);
        }
        Destroy(carga);
    }
    public void EliminarLineaCarga(GameObject linea)
    {
        lineasCarga.Remove(linea);
        lineasPunteadas.EliminarLineasDeCarga(linea.transform);
        Destroy(linea);
    }
}