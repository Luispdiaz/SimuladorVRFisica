using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga;
    public Button insertarCargaPositivaBtn;
    public Button insertarCargaNegativaBtn;
    public Button botonSensorDetalle;
    public Button botonSensorSuma;
    public Button sumaDeCargasButton;
    public Button vectoresDesdeSensorButton;
    public Button closeButton;
    public Button recalcularButton;
    public Button insertarLineaPositivaBtn;
    public Button insertarLineaNegativaBtn;
    public Button botonSensorVoltaje;       // Botón para crear sensor de voltaje
    public GameObject sensorVoltajePrefab;  // Prefab del sensor voltaje (con un texto en su interior)
    public Transform spawnPoint;            // Punto de aparición de las cargas e indicadores
    public GameObject cargaPositivaPrefab;
    public GameObject cargaNegativaPrefab;
    public GameObject indicadorFuerzaPrefab;
    public GameObject indicadorFuerzaDetallePrefab;
    public GameObject miniSpherePrefab;
    public GameObject lineaCargaPositivaPrefab;
    public GameObject lineaCargaNegativaPrefab;



    // Listas internas
    private List<GameObject> lineasCarga = new List<GameObject>();
    private List<GameObject> cargas = new List<GameObject>();
    private List<GameObject> sensores = new List<GameObject>();
    private List<GameObject> esferasEquipotenciales = new List<GameObject>();


    // Acceso a las listas (si lo necesitas en otros scripts)
    public List<GameObject> Cargas => cargas;
    public List<GameObject> LineasCarga => lineasCarga;

    // Referencias a otros sistemas
    private LineasPunteadas lineasPunteadas;
    private SumaDeCargas sumaDeCargas;
    private VectoresDesdeSensor vectoresDesdeSensor;

    // Banderas (ejemplo)
    private bool turnoSumaDeCargasActivo = false;
    private bool turnoVectoresDesdeSensorActivo = false;

    // Recalcular
    private bool modoRecalcularActivo = false;
    private int lastSensorCountRecalcular = 0;

    [Header("Voltaje Equipotencial")]
    public float voltajeDeseado = 3f; // Valor por defecto
    public Slider sliderVoltaje;      // Asignar en Inspector

    private void Start()
    {
        subMenuCarga.SetActive(false);

        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
        botonSensorDetalle.onClick.AddListener(() => CrearSensor("sensor detalle", indicadorFuerzaDetallePrefab));
        botonSensorSuma.onClick.AddListener(() => CrearSensor("sensor suma", indicadorFuerzaPrefab));
        sumaDeCargasButton.onClick.AddListener(CrearSumaDeCargas);
        vectoresDesdeSensorButton.onClick.AddListener(CrearVectoresDesdeSensor);
        closeButton.onClick.AddListener(CerrarSubMenu);
        recalcularButton.onClick.AddListener(RecalcularLineasPunteadasToggle);
        insertarLineaPositivaBtn.onClick.AddListener(IngresarLineaPositiva);
        insertarLineaNegativaBtn.onClick.AddListener(IngresarLineaNegativa);
        botonSensorVoltaje.onClick.AddListener(CrearSensorVoltaje);

        lineasPunteadas = GetComponent<LineasPunteadas>() ?? gameObject.AddComponent<LineasPunteadas>();
        lineasPunteadas.miniSpherePrefab = miniSpherePrefab;

        sumaDeCargas = GetComponent<SumaDeCargas>() ?? gameObject.AddComponent<SumaDeCargas>();
        vectoresDesdeSensor = GetComponent<VectoresDesdeSensor>() ?? gameObject.AddComponent<VectoresDesdeSensor>();

        if (sliderVoltaje != null)
        {
            sliderVoltaje.onValueChanged.AddListener(OnSliderVoltajeChanged);
            OnSliderVoltajeChanged(sliderVoltaje.value);
        }
    }

    private void Update()
    {
        // Actualizar fuerza (IndicadorFuerza) en cada sensor
        foreach (var sensor in sensores)
        {
            ActualizarSensorDeFuerza(sensor);
            ActualizarVoltajeSensor(sensor);
        }

        // Actualizar la fuerza mostrada en cada carga (opcional)
        foreach (var carga in cargas)
        {
            ActualizarTextoFuerzaCarga(carga);
        }

        // Actualizar flechas según el modo activo
        if (cargas.Count > 0 || lineasCarga.Count > 0)
        {
            ActualizarFlechas();
        }
    }

    /// <summary> Crea un sensor de voltaje. </summary>
    private void CrearSensorVoltaje()
    {
        GameObject nuevoSensor = Instantiate(sensorVoltajePrefab, spawnPoint.position, Quaternion.identity);
        // (Opcional) Asignar un tag "sensor voltaje" si lo deseas
        // nuevoSensor.tag = "sensor voltaje";

        sensores.Add(nuevoSensor);
    }

    /// <summary> Actualiza el indicador de fuerza (si el sensor tiene IndicadorFuerza). </summary>
    private void ActualizarSensorDeFuerza(GameObject sensor)
    {
        var indicadorScript = sensor.GetComponent<IndicadorFuerza>();
        if (indicadorScript != null)
        {
            Vector3 fuerzaTotal = CalcularFuerzaTotal(sensor.transform.position);
            indicadorScript.ActualizarDireccion(fuerzaTotal);
        }
    }

    /// <summary> Calcula y muestra el voltaje en el sensor de voltaje. </summary>
    private void ActualizarVoltajeSensor(GameObject sensor)
    {
        SensorVoltaje sensorVoltaje = sensor.GetComponent<SensorVoltaje>();
        if (sensorVoltaje != null)
        {
            float voltaje = CalcularVoltaje(sensor.transform.position);
            sensorVoltaje.ActualizarTextoVoltaje(voltaje);

            // Comprobamos si estamos cerca del voltajeDeseado
            if (Mathf.Abs(voltaje - voltajeDeseado) < 0.1f) // margen de tolerancia
            {
                // Llamar a método para pintar la superficie
                PintarSuperficieEquipotencial(voltajeDeseado);
            }
        }
    }


    /// <summary>
    /// Calcula la fuerza total que actúa en un punto, sumando cargas y líneas.
    /// (Usado por IndicadorFuerza).
    /// </summary>
    private Vector3 CalcularFuerzaTotal(Vector3 posicionSensor)
    {
        Vector3 fuerzaTotal = Vector3.zero;

        // Cargas puntuales
        foreach (var cargaObj in cargas)
        {
            var cargaScript = cargaObj.GetComponent<Carga>();
            if (cargaScript == null) continue;

            Vector3 direccion = posicionSensor - cargaObj.transform.position;
            float distancia = direccion.magnitude;
            if (distancia > 0.01f)
            {
                float fuerzaMagnitud = cargaScript.fuerza / (distancia * distancia);
                if (!cargaScript.esPositiva) fuerzaMagnitud = -fuerzaMagnitud;
                fuerzaTotal += fuerzaMagnitud * direccion.normalized;
            }
        }

        // Líneas de carga
        foreach (var lineaObj in lineasCarga)
        {
            var scriptLinea = lineaObj.GetComponent<LineaCarga>();
            if (scriptLinea == null) continue;

            Vector3 fuerzaLinea = CalcularFuerzaLinea(lineaObj, posicionSensor, scriptLinea);
            fuerzaTotal += fuerzaLinea;
        }

        return fuerzaTotal;
    }

    /// <summary>
    /// Calcula la contribución de una línea de carga (ya existente en tu código).
    /// </summary>
    private Vector3 CalcularFuerzaLinea(GameObject linea, Vector3 posicionSensor, LineaCarga scriptLinea)
    {
        Collider collider = linea.GetComponent<Collider>();
        if (collider == null) return Vector3.zero;

        Vector3 puntoMasCercano = collider.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoMasCercano;
        float distancia = direccion.magnitude;
        if (distancia < 0.01f) return Vector3.zero;

        float magnitud = scriptLinea.densidadCarga / distancia;
        if (!scriptLinea.esPositiva) magnitud *= -1;
        return magnitud * direccion.normalized;
    }

    /// <summary>
    /// Calcula el voltaje en un punto, sumando la contribución de todas las cargas y líneas.
    /// </summary>
private float CalcularVoltaje(Vector3 posicion)
{
    float k = 1f;
    float voltaje = 0f;

    // Cargas puntuales
    foreach (var cargaObj in cargas)
    {
        var scriptCarga = cargaObj.GetComponent<Carga>();
        if (scriptCarga == null) continue;

        float distancia = Vector3.Distance(posicion, cargaObj.transform.position);
        if (distancia < 0.01f) continue;

        float cargaElectrica = scriptCarga.esPositiva ? scriptCarga.fuerza : -scriptCarga.fuerza;
        float contrib = k * cargaElectrica / distancia;
        voltaje += contrib;
    }

    // Líneas de carga
    foreach (var lineaObj in lineasCarga)
    {
        var scriptLinea = lineaObj.GetComponent<LineaCarga>();
        if (scriptLinea == null) continue;

        float distancia = Vector3.Distance(posicion, lineaObj.transform.position);
        if (distancia < 0.01f) continue;

        float contrib = scriptLinea.densidadCarga * Mathf.Log(distancia); 
        voltaje += contrib;
    }

    return voltaje;
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

    private void ActualizarFlechas()
    {
        // Modo SumaDeCargas
        if (turnoSumaDeCargasActivo)
        {
            foreach (var c in cargas)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(c);
            }
            foreach (var l in lineasCarga)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(l);
            }
        }
        // Modo VectoresDesdeSensor
        else if (turnoVectoresDesdeSensorActivo)
        {
            foreach (var c in cargas)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(c);
            }
            foreach (var l in lineasCarga)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(l);
            }
        }
    }

    public void MostrarSubMenuCargas() => subMenuCarga.SetActive(true);
    public void CerrarSubMenu() => subMenuCarga.SetActive(false);

    public void IngresarCargaPositiva() => CrearCarga(cargaPositivaPrefab, true);
    public void IngresarCargaNegativa() => CrearCarga(cargaNegativaPrefab, false);
    public void IngresarLineaPositiva() => CrearLineaCarga(lineaCargaPositivaPrefab, true);
    public void IngresarLineaNegativa() => CrearLineaCarga(lineaCargaNegativaPrefab, false);

    private void OnSliderVoltajeChanged(float value)
    {
        voltajeDeseado = value;
        Debug.Log($"[Manager] Nuevo voltaje deseado: {voltajeDeseado}");
    }

    private void CrearLineaCarga(GameObject prefab, bool esPositiva)
    {
        var nuevaLinea = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        var script = nuevaLinea.AddComponent<LineaCarga>();
        script.esPositiva = esPositiva;
        lineasCarga.Add(nuevaLinea);
    }

    private void CrearCarga(GameObject prefab, bool esPositiva)
    {
        var nuevaCarga = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        var script = nuevaCarga.AddComponent<Carga>();
        script.esPositiva = esPositiva;
        cargas.Add(nuevaCarga);
    }

    private void CrearSensor(string tagSensor, GameObject prefabSensor)
    {
        Debug.Log($"Creando sensor con tag: {tagSensor}");
        var nuevoSensor = Instantiate(prefabSensor, spawnPoint.position, spawnPoint.rotation);
        nuevoSensor.tag = tagSensor;
        var indicadorScript = nuevoSensor.AddComponent<IndicadorFuerza>();
        // Se actualiza su fuerza de inmediato
        ActualizarSensorDeFuerza(nuevoSensor);
        sensores.Add(nuevoSensor);
    }

    // Recalcular punteadas
    private void RecalcularLineasPunteadasToggle()
    {
        int currentCount = CountSensorsDetalle();
        if (modoRecalcularActivo)
        {
            if (currentCount > lastSensorCountRecalcular)
            {
                lastSensorCountRecalcular = currentCount;
                lineasPunteadas.EliminarTodasLasLineas();
                foreach (var sensor in sensores)
                {
                    foreach (var c in cargas)
                    {
                        lineasPunteadas.CrearLineasPunteadas(c.transform, sensor.transform);
                    }
                    foreach (var l in lineasCarga)
                    {
                        lineasPunteadas.CrearLineasPunteadas(l.transform, sensor.transform);
                    }
                }
            }
            else
            {
                lineasPunteadas.EliminarTodasLasLineas();
                modoRecalcularActivo = false;
            }
        }
        else
        {
            modoRecalcularActivo = true;
            lastSensorCountRecalcular = currentCount;
            lineasPunteadas.EliminarTodasLasLineas();
            foreach (var sensor in sensores)
            {
                foreach (var c in cargas)
                {
                    lineasPunteadas.CrearLineasPunteadas(c.transform, sensor.transform);
                }
                foreach (var l in lineasCarga)
                {
                    lineasPunteadas.CrearLineasPunteadas(l.transform, sensor.transform);
                }
            }
        }
    }

    /// <summary>
    /// Crea las flechas de SumaDeCargas y elimina las de VectoresDesdeSensor.
    /// </summary>
    public void CrearSumaDeCargas()
    {
        if (turnoSumaDeCargasActivo)
        {
            sumaDeCargas.EliminarTodasLasFlechas();
            turnoSumaDeCargasActivo = false;
        }
        else
        {
            if (turnoVectoresDesdeSensorActivo)
            {
                vectoresDesdeSensor.EliminarTodasLasFlechas();
                turnoVectoresDesdeSensorActivo = false;
            }
            turnoSumaDeCargasActivo = true;
            sumaDeCargas.sensores = sensores;

            foreach (var c in cargas) sumaDeCargas.CrearOActualizarFlechaParaFuente(c);
            foreach (var l in lineasCarga) sumaDeCargas.CrearOActualizarFlechaParaFuente(l);

            // Llamar a la animación (si tu SumaDeCargas la tiene)
            sumaDeCargas.IniciarAnimacionSuma();
        }
    }

    /// <summary>
    /// Crea las flechas de VectoresDesdeSensor y elimina las de SumaDeCargas.
    /// </summary>
    public void CrearVectoresDesdeSensor()
    {
        if (turnoVectoresDesdeSensorActivo)
        {
            vectoresDesdeSensor.EliminarTodasLasFlechas();
            turnoVectoresDesdeSensorActivo = false;
        }
        else
        {
            if (turnoSumaDeCargasActivo)
            {
                sumaDeCargas.EliminarTodasLasFlechas();
                turnoSumaDeCargasActivo = false;
            }
            turnoVectoresDesdeSensorActivo = true;
            vectoresDesdeSensor.sensores = sensores;

            foreach (var c in cargas) vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(c);
            foreach (var l in lineasCarga) vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(l);
        }
    }

    // Eliminar
    public void EliminarCarga(GameObject carga)
    {
        lineasPunteadas.EliminarLineasDeCarga(carga.transform);
        cargas.Remove(carga);

        if (sumaDeCargas.flechasPorFuentePorSensor.ContainsKey(carga))
        {
            var dict = sumaDeCargas.flechasPorFuentePorSensor[carga];
            foreach (var flecha in dict.Values) Destroy(flecha);
            sumaDeCargas.flechasPorFuentePorSensor.Remove(carga);
        }
        Destroy(carga);
    }

    public void EliminarLineaCarga(GameObject linea)
    {
        lineasCarga.Remove(linea);
        lineasPunteadas.EliminarLineasDeCarga(linea.transform);

        if (sumaDeCargas.flechasPorFuentePorSensor.ContainsKey(linea))
        {
            var dict = sumaDeCargas.flechasPorFuentePorSensor[linea];
            foreach (var flecha in dict.Values) Destroy(flecha);
            sumaDeCargas.flechasPorFuentePorSensor.Remove(linea);
        }
        Destroy(linea);
    }

    private int CountSensorsDetalle()
    {
        int count = 0;
        foreach (var sensor in sensores)
        {
            if (sensor.CompareTag("sensor detalle")) count++;
        }
        return count;
    }

    public void PintarSuperficieEquipotencial(float voltajeObjetivo)
    {
        Debug.Log($"[Manager] Pintando equipotencial para V = {voltajeObjetivo}");

        // 1) Limpiar esferas previas (opcional)
        foreach (var go in esferasEquipotenciales) Destroy(go);
        esferasEquipotenciales.Clear();

        // 2) Definir límites de muestreo
        Vector3 center = Vector3.zero; // Podrías calcular un bounding box de tus cargas
        float size = 10f;              // radio de muestreo

        int resolution = 50; // Cuantos puntos por eje (20^3 = 8000 muestras)
        float step = (size * 2) / resolution;

        for (int ix = 0; ix < resolution; ix++)
        {
            for (int iy = 0; iy < resolution; iy++)
            {
                for (int iz = 0; iz < resolution; iz++)
                {
                    Vector3 samplePos = center + new Vector3(
                        -size + ix * step,
                        -size + iy * step,
                        -size + iz * step
                    );

                    float v = CalcularVoltaje(samplePos);
                    if (Mathf.Abs(v - voltajeObjetivo) < 0.2f) // tolerancia
                    {
                        // Instanciar pequeña esfera
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = samplePos;
                        sphere.transform.localScale = Vector3.one * (step * 0.5f);
                        sphere.GetComponent<Renderer>().material.color = Color.yellow;

                        esferasEquipotenciales.Add(sphere);
                    }
                }
            }
        }

        Debug.Log($"[Manager] Se crearon {esferasEquipotenciales.Count} puntos equipotenciales");
    }

}
