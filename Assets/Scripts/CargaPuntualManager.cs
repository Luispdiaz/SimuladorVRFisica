using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga; // Panel del submenú
    public Button insertarCargaPositivaBtn; // Botón para insertar carga positiva
    public Button insertarCargaNegativaBtn; // Botón para insertar carga negativa
    public Button botonSensorDetalle; // Botón para crear sensor de detalle
    public Button botonSensorSuma; // Botón para crear sensor de suma
    public Button sumaDeCargasButton; // Botón para crear la suma de cargas
    public Button vectoresDesdeSensorButton; // Botón para vectores desde el sensor
    public Button closeButton; // Botón para cerrar el submenú
    public Button recalcularButton; // Botón para recalcular (Dirección por Carga)
    public Button insertarLineaPositivaBtn; // Botón para línea positiva
    public Button insertarLineaNegativaBtn; // Botón para línea negativa
    public Button botonSensorVoltaje; // Nuevo botón en el inspector
    public GameObject sensorVoltajePrefab; // Asigna el prefab en el inspector
    public Transform spawnPoint; // Punto de aparición de las cargas e indicadores
    public GameObject cargaPositivaPrefab; // Prefab de la carga positiva
    public GameObject cargaNegativaPrefab; // Prefab de la carga negativa
    public GameObject indicadorFuerzaPrefab; // Prefab del indicador de fuerza (sensor suma)
    public GameObject indicadorFuerzaDetallePrefab; // Prefab para el sensor de detalle
    public GameObject miniSpherePrefab; // Prefab de la mini esfera para las líneas punteadas
    public GameObject lineaCargaPositivaPrefab;
    public GameObject lineaCargaNegativaPrefab;

    private List<GameObject> lineasCarga = new List<GameObject>(); // Líneas de carga
    private List<GameObject> cargas = new List<GameObject>(); // Cargas
    private List<GameObject> sensores = new List<GameObject>(); // Sensores creados
    public List<GameObject> Cargas => cargas;
    public List<GameObject> LineasCarga => lineasCarga;
    private LineasPunteadas lineasPunteadas;
    private SumaDeCargas sumaDeCargas; // Para SumaDeCargas (actualizada)
    private VectoresDesdeSensor vectoresDesdeSensor; // Para VectoresDesdeSensor

    // Banderas para SumaDeCargas y VectoresDesdeSensor (lógica clásica)
    private bool turnoSumaDeCargasActivo = false;
    private bool turnoVectoresDesdeSensorActivo = false;

    // Banderas y contador para Recalcular (toggle con sensor count)
    private bool modoRecalcularActivo = false;
    private int lastSensorCountRecalcular = 0;

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
        botonSensorVoltaje.onClick.AddListener(() => CrearSensorVoltaje());

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

        vectoresDesdeSensor = GetComponent<VectoresDesdeSensor>();
        if (vectoresDesdeSensor == null)
        {
            vectoresDesdeSensor = gameObject.AddComponent<VectoresDesdeSensor>();
        }
    }

    private void Update()
    {
        // Actualizar dirección y rotación de cada sensor
        foreach (var sensor in sensores)
        {
            ActualizarSensor(sensor);
            ActualizarVoltajeSensor(sensor); // Nuevo método para voltaje
        }

        // Actualizar la fuerza mostrada de cada carga
        foreach (var carga in cargas)
        {
            ActualizarTextoFuerzaCarga(carga);
        }

        // Actualizar flechas o líneas según el modo activo (solo se actualiza el conjunto activo)
        if (cargas.Count > 0 || lineasCarga.Count > 0)
        {
            ActualizarFlechas();
        }
    }
    // Nuevo método para actualizar el voltaje
    private void ActualizarVoltajeSensor(GameObject sensor)
    {
        VoltageSensor voltageScript = sensor.GetComponent<VoltageSensor>();
        if (voltageScript != null)
        {
            voltageScript.ActualizarVoltaje();
        }
    }

    /// <summary>
    /// Actualiza las flechas según la bandera:
    /// - Si SumaDeCargas está activa, se actualizan solo sus flechas.
    /// - Si VectoresDesdeSensor está activa, se actualizan solo sus flechas.
    /// - Si ninguno está activo, no se actualiza nada.
    /// </summary>
    private void ActualizarFlechas()
    {
        if (turnoSumaDeCargasActivo)
        {
            foreach (var carga in cargas)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(carga);
            }
            foreach (var linea in lineasCarga)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(linea);
            }
        }
        else if (turnoVectoresDesdeSensorActivo)
        {
            foreach (var carga in cargas)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(carga);
            }
            foreach (var linea in lineasCarga)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(linea);
            }
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
        CrearCarga(cargaPositivaPrefab, true);
    }

    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaNegativaPrefab, false);
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

    private void CrearCarga(GameObject cargaPrefab, bool esPositiva)
    {
        GameObject nuevaCarga = Instantiate(cargaPrefab, spawnPoint.position, spawnPoint.rotation);
        Carga cargaScript = nuevaCarga.AddComponent<Carga>();
        cargaScript.esPositiva = esPositiva;
        cargas.Add(nuevaCarga);
    }

    private void CrearSensor(string tagSensor, GameObject prefabSensor)
    {
        Debug.Log($"Creando sensor con tag: {tagSensor}");
        GameObject nuevoSensor = Instantiate(prefabSensor, spawnPoint.position, spawnPoint.rotation);
        nuevoSensor.tag = tagSensor;
        IndicadorFuerza indicadorScript = nuevoSensor.AddComponent<IndicadorFuerza>();
        ActualizarSensor(nuevoSensor);
        sensores.Add(nuevoSensor);
    }
    private void CrearSensorVoltaje()
    {
        GameObject nuevoSensor = Instantiate(sensorVoltajePrefab, spawnPoint.position, Quaternion.identity);
        sensores.Add(nuevoSensor); // Añade el sensor a la lista para que se actualice
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
                if (distancia > 0.01f)
                {
                    float fuerzaMagnitud = cargaScript.fuerza / Mathf.Pow(distancia, 2);
                    if (!cargaScript.esPositiva)
                    {
                        fuerzaMagnitud = -fuerzaMagnitud;
                    }
                    Vector3 fuerza = fuerzaMagnitud * direccion.normalized;
                    fuerzaTotal += fuerza;
                }
            }
        }
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

    private void ActualizarTextoFuerzaCarga(GameObject carga)
    {
        var cargaScript = carga.GetComponent<Carga>();
        var textComponent = carga.GetComponentInChildren<Text>();
        if (cargaScript != null && textComponent != null)
        {
            textComponent.text = cargaScript.fuerza.ToString("F2");
        }
    }

    // Método helper para contar sensores con tag "sensor detalle"
    private int CountSensorsDetalle()
    {
        int count = 0;
        foreach (var sensor in sensores)
        {
            if (sensor.CompareTag("sensor detalle"))
                count++;
        }
        return count;
    }

    /// <summary>
    /// Toggle para Recalcular (Dirección por Carga) usando la nueva lógica:
    /// - Si el modo ya está activo y no hay sensores nuevos, se desactiva.
    /// - Si hay sensores nuevos, se actualiza la funcionalidad.
    /// - Si no estaba activo, se activa.
    /// 
    /// Nota: Esta lógica es independiente y no afecta las banderas de SumaDeCargas o VectoresDesdeSensor.
    /// </summary>
    public void RecalcularLineasPunteadasToggle()
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
                    foreach (var carga in cargas)
                    {
                        lineasPunteadas.CrearLineasPunteadas(carga.transform, sensor.transform);
                    }
                    foreach (var linea in lineasCarga)
                    {
                        lineasPunteadas.CrearLineasPunteadas(linea.transform, sensor.transform);
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
                foreach (var carga in cargas)
                {
                    lineasPunteadas.CrearLineasPunteadas(carga.transform, sensor.transform);
                }
                foreach (var linea in lineasCarga)
                {
                    lineasPunteadas.CrearLineasPunteadas(linea.transform, sensor.transform);
                }
            }
        }
    }

    /// <summary>
    /// Toggle para SumaDeCargas con la lógica clásica:
    /// - Si ya está activo y se presiona nuevamente, se elimina lo relacionado y se desactiva.
    /// - Si no está activo, se activa (desactivando el otro modo si estuviera activo).
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
            foreach (var carga in cargas)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(carga);
            }
            foreach (var linea in lineasCarga)
            {
                sumaDeCargas.CrearOActualizarFlechaParaFuente(linea);
            }
            turnoSumaDeCargasActivo = true;
            sumaDeCargas.sensores = sensores;
            sumaDeCargas.IniciarAnimacionSuma();
        }
    }

    /// <summary>
    /// Toggle para VectoresDesdeSensor con la lógica clásica:
    /// - Si ya está activo y se presiona nuevamente, se elimina lo relacionado y se desactiva.
    /// - Si no está activo, se activa (desactivando el otro modo si estuviera activo).
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
            foreach (var carga in cargas)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(carga);
            }
            foreach (var linea in lineasCarga)
            {
                vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(linea);
            }
        }
    }

    // Método para eliminar una carga
    public void EliminarCarga(GameObject carga)
    {
        lineasPunteadas.EliminarLineasDeCarga(carga.transform);
        cargas.Remove(carga);
        // Eliminamos todas las flechas asociadas a esta carga en SumaDeCargas (ahora usamos el diccionario anidado)
        if (sumaDeCargas.flechasPorFuentePorSensor.ContainsKey(carga))
        {
            var dict = sumaDeCargas.flechasPorFuentePorSensor[carga];
            foreach (var flecha in dict.Values)
            {
                Destroy(flecha);
            }
            sumaDeCargas.flechasPorFuentePorSensor.Remove(carga);
        }
        Destroy(carga);
    }

    // Método para eliminar una línea de carga
    public void EliminarLineaCarga(GameObject linea)
    {
        lineasCarga.Remove(linea);
        lineasPunteadas.EliminarLineasDeCarga(linea.transform);
        // Eliminamos todas las flechas asociadas a esta línea en SumaDeCargas
        if (sumaDeCargas.flechasPorFuentePorSensor.ContainsKey(linea))
        {
            var dict = sumaDeCargas.flechasPorFuentePorSensor[linea];
            foreach (var flecha in dict.Values)
            {
                Destroy(flecha);
            }
            sumaDeCargas.flechasPorFuentePorSensor.Remove(linea);
        }
        Destroy(linea);
    }
}
