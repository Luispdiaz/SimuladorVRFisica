using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class CargaPuntualManager : MonoBehaviour
{
    // Referencias a UI y objetos
    public GameObject subMenuCarga;
    public GameObject subMenuCarga1;
    public Button insertarCargaPositivaBtn;
    public Button insertarCargaNegativaBtn;
    public Button botonSensorDetalle;
    public Button botonSensorSuma;
    public Button sumaDeCargasButton;
    public Button vectoresDesdeSensorButton;
    public Button closeButton;
    public Button closeButton1;
    public Button flexometroButton;
    public Flexometro flexometro;
    private bool modoFlexometroActivo = false;
    public Button recalcularButton;
    public Button insertarLineaPositivaBtn;
    public Button insertarLineaNegativaBtn;
    public Button leyCoulombBtn;
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

    [Header("Planos Configuration")]
    public GameObject planoPositivoPrefab;
    public GameObject planoNegativoPrefab;
    public float offsetPlano = 5f; // Distancia desde el spawnPoint

    [Header("Botones Planos")]
    public Button insertarPlanoPositivoBtn;
    public Button insertarPlanoNegativoBtn;

    [Header("Sensor Individual Dirección")]
    public Button sensorIndividualBtn; // Asignar en Inspector
    public GameObject sensorIndividualDireccionPrefab; // Asignar nuevo prefab en Inspector

    public Slider sliderFuerza;
    public float fuerzaDeseada = 1f;
    public TextMeshProUGUI textoFuerza;

    [Header("Mover Plano Arriba")]
    public Transform moverPlanoArribaSpawnPoint; // Nuevo spawn point para mover el plano
    public Button moverPlanoArribaButton;

    public Transform moverPlanoIzquierdaSpawnPoint; // Spawn point para mover el plano a la izquierda
    public Button moverPlanoIzquierdaButton;          // Botón para mover el plano a la izquierda

    public Transform moverPlanoDerechaSpawnPoint;    // Spawn point para mover el plano a la derecha
    public Button moverPlanoDerechaButton;             // Botón para mover el plano a la derecha

    public Transform moverPlanoAbajoSpawnPoint;        // Spawn point para mover el plano hacia abajo
    public Button moverPlanoAbajoButton;


    // Listas internas
    public List<GameObject> lineasCarga = new List<GameObject>();
    private List<GameObject> planos = new List<GameObject>();
    private List<GameObject> cargas = new List<GameObject>();
    public List<GameObject> sensores = new List<GameObject>();
    public List<GameObject> ObtenerPlanos() => planos;
    // Posición inicial de cada sensor detalle cuando activamos la Suma de Cargas
    private Dictionary<GameObject, Vector3> posInicialSensoresDetalle = new Dictionary<GameObject, Vector3>();
    private Color normalColorRecalcular;
    private Color normalColorSuma;
    private Color normalColorVectores;
    private Color selectedColor; // #464689

    // ============ NUEVAS variables para estado previo de banderas ============
    private bool oldRecalcular = false;
    private bool oldSuma = false;
    private bool oldVectores = false;

    // NUEVA variable para rastrear si antes teníamos sensor detalle
    private bool oldHasSensorDetalle = false;

    // Para almacenar la posición inicial de cada carga y línea cuando se activa SumaDeCargas
    private Dictionary<GameObject, Vector3> posInicialCargasLineas = new Dictionary<GameObject, Vector3>();


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

    // Agregar estas variables
    public GameObject indicadorDireccionPrefab; // Asignar en Inspector el nuevo prefab
    public float distanciaEntreSensores = 0.5f;
    public int rango = 5; // Sensores desde -5 a +5 en cada eje
    public Button lineasDeCampoBtn; // Asignar en Inspector
    private bool lineasDeCampoActivas = false;

    [Header("Configuración de Formaciones")]
    public Transform[] spawnPointsLinea = new Transform[2];
    public Transform[] spawnPointsTriangulo = new Transform[3];
    public Transform[] spawnPointsCuadrado = new Transform[4];
    public Transform[] spawnPointsPentagono = new Transform[5];
    public Button botonFormacionLinea;
    public Button botonFormacionTriangulo;
    public Button botonFormacionCuadrado;
    public Button botonFormacionPentagono;



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
        closeButton1.onClick.AddListener(CerrarSubMenu1);
        recalcularButton.onClick.AddListener(RecalcularLineasPunteadasToggle);
        insertarLineaPositivaBtn.onClick.AddListener(IngresarLineaPositiva);
        insertarLineaNegativaBtn.onClick.AddListener(IngresarLineaNegativa);
        botonSensorVoltaje.onClick.AddListener(CrearSensorVoltaje);
        leyCoulombBtn.onClick.AddListener(ActivarSensorDetalleEnCargaCercana);
        lineasDeCampoBtn.onClick.AddListener(ToggleLineasDeCampo);
        insertarPlanoPositivoBtn.onClick.AddListener(IngresarPlanoPositivo);
        insertarPlanoNegativoBtn.onClick.AddListener(IngresarPlanoNegativo);
        sensorIndividualBtn.onClick.AddListener(CrearSensorIndividual);
        moverPlanoArribaButton.onClick.AddListener(MoverPlanoArriba);
        moverPlanoIzquierdaButton.onClick.AddListener(MoverPlanoIzquierda);
        moverPlanoDerechaButton.onClick.AddListener(MoverPlanoDerecha);
        moverPlanoAbajoButton.onClick.AddListener(MoverPlanoAbajo);
        botonFormacionLinea.onClick.AddListener(FormacionLinea);
        botonFormacionTriangulo.onClick.AddListener(FormacionTriangulo);
        botonFormacionCuadrado.onClick.AddListener(FormacionCuadrado);
        botonFormacionPentagono.onClick.AddListener(FormacionPentagono);
        flexometroButton.onClick.AddListener(RecalcularFlexometroToggle);


        lineasPunteadas = GetComponent<LineasPunteadas>() ?? gameObject.AddComponent<LineasPunteadas>();
        lineasPunteadas.miniSpherePrefab = miniSpherePrefab;

        sumaDeCargas = GetComponent<SumaDeCargas>() ?? gameObject.AddComponent<SumaDeCargas>();
        vectoresDesdeSensor = GetComponent<VectoresDesdeSensor>() ?? gameObject.AddComponent<VectoresDesdeSensor>();

        if (sliderVoltaje != null)
        {
            sliderVoltaje.onValueChanged.AddListener(OnSliderVoltajeChanged);
            OnSliderVoltajeChanged(sliderVoltaje.value);
        }
        // 1) Guardar color original de cada botón
        normalColorRecalcular = recalcularButton.image.color;
        normalColorSuma = sumaDeCargasButton.image.color;
        normalColorVectores = vectoresDesdeSensorButton.image.color;

        // 2) Convertir #464689 a un Color de Unity
        ColorUtility.TryParseHtmlString("#464689", out selectedColor);

        // 3) Inicializar old* con los valores actuales de las banderas
        oldRecalcular = modoRecalcularActivo;
        oldSuma = turnoSumaDeCargasActivo;
        oldVectores = turnoVectoresDesdeSensorActivo;

        // (Opcional) Llamar una vez para estado inicial
        UpdateButtonsColor();

        // NUEVO: Revisamos si hay sensor detalle al inicio
        oldHasSensorDetalle = CountSensorsDetalle() > 0;
        // Llamamos a UpdateButtonsInteractable() con el estado inicial
        UpdateButtonsInteractable(oldHasSensorDetalle);

        if (sliderFuerza != null)
        {
            sliderFuerza.onValueChanged.AddListener(OnSliderFuerzaChanged);
            OnSliderFuerzaChanged(sliderFuerza.value);  // Inicializa el valor de la fuerza
        }

    }

    private void Update()
    {
        // Actualizar fuerza (IndicadorFuerza) en cada sensor
        foreach (var sensor in sensores)
        {
            GameObject cargaAsociada = ObtenerCargaMasCercana(sensor.transform.position);
            if (cargaAsociada != null &&
                Vector3.Distance(sensor.transform.position, cargaAsociada.transform.position) < 0.1f)
            {
                ActualizarSensorDeFuerza(sensor); // Forzar actualización constante
            }
            ActualizarSensorDeFuerza(sensor);
            ActualizarVoltajeSensor(sensor);
        }

        // Actualizar la fuerza mostrada en cada carga (opcional)
        foreach (var carga in cargas)
        {
            ActualizarTextoFuerzaCarga(carga);
        }

        foreach (var linea in lineasCarga)
        {
            ActualizarTextoFuerzaLinea(linea);
        }

        // Actualizar flechas según el modo activo
        if (cargas.Count > 0 || lineasCarga.Count > 0)
        {
            ActualizarFlechas();
        }
        // NUEVO: Si Suma de Cargas está activo, revisamos si algún sensor detalle cambió su posición
        if (turnoSumaDeCargasActivo)
        {
            bool algunSensorMovido = false;

            // Recorremos las posiciones iniciales guardadas
            foreach (var par in posInicialSensoresDetalle)
            {
                GameObject sensorDetalle = par.Key;
                Vector3 posInicial = par.Value;

                // Si el sensor ya no existe, lo ignoramos
                if (sensorDetalle == null) continue;

                // Comparamos su posición actual con la guardada
                float dist = Vector3.Distance(sensorDetalle.transform.position, posInicial);
                if (dist > 0.01f) // margen de tolerancia
                {
                    algunSensorMovido = true;
                    break;
                }
            }

            // Si detectamos que al menos un sensor se movió:
            if (algunSensorMovido)
            {
                // Eliminamos flechas de SumaDeCargas
                sumaDeCargas.EliminarTodasLasFlechas();
                turnoSumaDeCargasActivo = false; // Desactivamos la bandera
                Debug.Log("[Manager] Sensor detalle movido => Flechas de Suma de Cargas eliminadas");
            }
        }

        // Actualizar flechas según el modo activo
        if (cargas.Count > 0 || lineasCarga.Count > 0)
        {
            ActualizarFlechas();
        }
        // NUEVO: Detectar si las banderas han cambiado
        if (modoRecalcularActivo != oldRecalcular ||
            turnoSumaDeCargasActivo != oldSuma ||
            turnoVectoresDesdeSensorActivo != oldVectores)
        {
            // Se detectó un cambio => actualizar colores
            UpdateButtonsColor();

            // Guardar el nuevo estado como "previo"
            oldRecalcular = modoRecalcularActivo;
            oldSuma = turnoSumaDeCargasActivo;
            oldVectores = turnoVectoresDesdeSensorActivo;
        }
        // NUEVO: Cada frame, verificamos si ahora hay (o no) sensor detalle
        bool hasSensorDetalle = (CountSensorsDetalle() > 0);

        // Si cambió el estado respecto al frame anterior
        if (hasSensorDetalle != oldHasSensorDetalle)
        {
            oldHasSensorDetalle = hasSensorDetalle;
            UpdateButtonsInteractable(hasSensorDetalle);
        }

        // Solo si Suma de Cargas está activo
        if (turnoSumaDeCargasActivo)
        {
            bool algunSensorMovido = false;
            bool algunCargaLineaMovida = false;

            // Revisa sensores detalle (ya existente)
            foreach (var par in posInicialSensoresDetalle)
            {
                GameObject sensorDetalle = par.Key;
                Vector3 posInicial = par.Value;
                if (sensorDetalle == null) continue;

                float dist = Vector3.Distance(sensorDetalle.transform.position, posInicial);
                if (dist > 0.01f)
                {
                    algunSensorMovido = true;
                    break;
                }
            }

            // NUEVO: Revisa cargas y líneas
            foreach (var par in posInicialCargasLineas)
            {
                GameObject obj = par.Key; // carga o línea
                Vector3 posInicial = par.Value;
                if (obj == null) continue;

                float dist = Vector3.Distance(obj.transform.position, posInicial);
                if (dist > 0.01f)
                {
                    algunCargaLineaMovida = true;
                    break;
                }
            }

            // Si algún sensor, carga o línea se movió, desactivamos SumaDeCargas
            if (algunSensorMovido || algunCargaLineaMovida)
            {
                sumaDeCargas.EliminarTodasLasFlechas();
                turnoSumaDeCargasActivo = false;
                Debug.Log("[Manager] Se movió sensor/carga/línea => Flechas de Suma de Cargas eliminadas");

                // (Opcional) Si usas un método para actualizar colores de botones:
                // UpdateButtonsColor();
            }
        }
        foreach (var sensor in sensores)
        {
            if (sensor.CompareTag("SensorIndividualDireccion") ||
               sensor.CompareTag("sensor linea"))
            {
                // Calcular fuerza usando la posición ACTUAL del sensor
                Vector3 fuerza = CalcularFuerzaTotal(sensor.transform.position);
                var indicador = sensor.GetComponent<IndicadorDireccion>();
                indicador.ActualizarDireccion(fuerza);
            }
        }

        if (modoFlexometroActivo)
        {
            flexometro.ActualizarMedicion();
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
            GameObject cargaAsociada = ObtenerCargaMasCercana(sensor.transform.position);
            bool sobreCarga = false;

            if (cargaAsociada != null)
            {
                float distancia = Vector3.Distance(sensor.transform.position, cargaAsociada.transform.position);
                sobreCarga = distancia < 0.1f;

                if (sobreCarga)
                {
                    // Calcular fuerza específica para carga asociada
                    Vector3 campoTotal = CalcularFuerzaTotal(sensor.transform.position, cargaAsociada);
                    Carga cargaScript = cargaAsociada.GetComponent<Carga>();
                    Vector3 fuerzaFinal = campoTotal * cargaScript.fuerza * (cargaScript.esPositiva ? 1 : -1);

                    indicadorScript.tipoCampo = IndicadorFuerza.TipoMagnitud.LeyDeCoulomb;
                    indicadorScript.ActualizarDireccion(fuerzaFinal);
                    return; // Salir después de aplicar cálculo especializado
                }
            }

            // Cálculo normal si no está sobre carga
            indicadorScript.tipoCampo = IndicadorFuerza.TipoMagnitud.CampoElectrico;
            Vector3 fuerzaNormal = CalcularFuerzaTotal(sensor.transform.position);
            indicadorScript.ActualizarDireccion(fuerzaNormal);
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
        }
    }


    /// <summary>
    /// Calcula la fuerza total que actúa en un punto, sumando cargas y líneas.
    /// (Usado por IndicadorFuerza).
    /// </summary>
    private Vector3 CalcularFuerzaTotal(Vector3 posicionSensor, GameObject cargaExcluida = null)
    {
        Vector3 fuerzaTotal = Vector3.zero;

        // Cargas puntuales
        foreach (var cargaObj in cargas)
        {
            if (cargaObj == cargaExcluida) continue; // Excluir carga asociada

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

        // Añadir contribución de planos
        // Suponiendo que en CalcularFuerzaTotal ya recorres la lista `planos`
        foreach (var planoObj in planos)
        {
            var scriptPlano = planoObj.GetComponent<PlanoCubo>(); // Cambiado de PlanoFisico1 a PlanoCubo
            if (scriptPlano == null) continue;

            Vector3 fuerzaPlano = scriptPlano.CalcularFuerzaPlano(posicionSensor);
            fuerzaTotal += fuerzaPlano;
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

        float magnitud = (scriptLinea.densidadCarga * 2) / distancia;
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

        foreach (var planoObj in planos)
        {
            var scriptPlano = planoObj.GetComponent<PlanoCubo>();
            if (scriptPlano == null) continue;

            float distancia = Vector3.Distance(posicion, planoObj.transform.position);
            if (distancia < 0.01f) continue;

            float contrib = 2 * Mathf.PI * scriptPlano.fuerza * distancia;
            voltaje += contrib;
        }

        return voltaje;
    }

    private void ActualizarTextoFuerzaCarga(GameObject cargaObj)
    {
        var cargaScript = cargaObj.GetComponent<Carga>();
        if (cargaScript == null) return;

        var cargaTexto = cargaObj.GetComponentInChildren<CargaTexto>();
        if (cargaTexto != null)
        {
            // Pasamos la información de si es positiva o no, y la magnitud de la fuerza
            cargaTexto.ActualizarTextoFuerza(cargaScript.esPositiva, cargaScript.fuerza);
        }
    }

    private void ActualizarTextoFuerzaLinea(GameObject lineaObj)
    {
        var lineaScript = lineaObj.GetComponent<LineaCarga>();
        if (lineaScript == null) return;

        var lineaTexto = lineaObj.GetComponentInChildren<LineaTexto>();
        if (lineaTexto != null)
        {
            // Pasamos la densidad de carga de la línea
            lineaTexto.ActualizarTextoLinea(lineaScript.densidadCarga, lineaScript.esPositiva);
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
            foreach (var p in planos) sumaDeCargas.CrearOActualizarFlechaParaFuente(p);
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
        foreach (var p in planos)
        {
            if (turnoSumaDeCargasActivo) sumaDeCargas.CrearOActualizarFlechaParaFuente(p);
            if (turnoVectoresDesdeSensorActivo) vectoresDesdeSensor.CrearOActualizarFlechaParaFuente(p);
        }
    }

    public void MostrarSubMenuCargas() => subMenuCarga.SetActive(true);
    public void CerrarSubMenu() => subMenuCarga.SetActive(false);
    public void CerrarSubMenu1() => subMenuCarga1.SetActive(false);

    public void IngresarCargaPositiva() => CrearCarga(cargaPositivaPrefab, true);
    public void IngresarCargaNegativa() => CrearCarga(cargaNegativaPrefab, false);
    public void IngresarLineaPositiva() => CrearLineaCarga(lineaCargaPositivaPrefab, true);
    public void IngresarLineaNegativa() => CrearLineaCarga(lineaCargaNegativaPrefab, false);

    public void IngresarPlanoPositivo() => CrearPlano(true);
    public void IngresarPlanoNegativo() => CrearPlano(false);


    private void OnSliderVoltajeChanged(float value)
    {
        voltajeDeseado = value;
        Debug.Log($"[Manager] Nuevo voltaje deseado: {voltajeDeseado}");
    }

    private void CrearLineaCarga(GameObject prefab, bool esPositiva)
    {
        // 1) Desactiva banderas
        modoRecalcularActivo = false;
        turnoSumaDeCargasActivo = false;
        turnoVectoresDesdeSensorActivo = false;

        // 2) Elimina visuales si estaban activos
        sumaDeCargas.EliminarTodasLasFlechas();
        vectoresDesdeSensor.EliminarTodasLasFlechas();
        lineasPunteadas.EliminarTodasLasLineas();

        // 3) Crear la nueva línea
        var nuevaLinea = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        var script = nuevaLinea.GetComponent<LineaCarga>();
        script.esPositiva = esPositiva;

        // Asegúrate de que la densidad de carga se esté tomando del slider (fuerzaDeseada)
        script.densidadCarga = fuerzaDeseada;  // Asignar el valor del slider a la densidad de carga de la línea

        lineasCarga.Add(nuevaLinea);
    }


    private void CrearCarga(GameObject prefab, bool esPositiva)
    {
        // Resetear las banderas
        modoRecalcularActivo = false;
        turnoSumaDeCargasActivo = false;
        turnoVectoresDesdeSensorActivo = false;

        // Limpiar los elementos visuales existentes
        sumaDeCargas.EliminarTodasLasFlechas();
        vectoresDesdeSensor.EliminarTodasLasFlechas();
        lineasPunteadas.EliminarTodasLasLineas();

        // Instanciar la nueva carga
        var nuevaCarga = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        var cargaScript = nuevaCarga.GetComponent<Carga>();
        cargaScript.esPositiva = esPositiva;
        cargaScript.fuerza = fuerzaDeseada;  // Asignar el valor de la fuerza desde el slider

        cargas.Add(nuevaCarga);
    }

    private void CrearSensor(string tagSensor, GameObject prefabSensor)
    {
        Debug.Log($"Creando sensor con tag: {tagSensor}");
        var nuevoSensor = Instantiate(prefabSensor, spawnPoint.position, spawnPoint.rotation);
        nuevoSensor.tag = tagSensor;

        // El script IndicadorFuerza ya está en el prefab, o lo agregas:
        var indicadorScript = nuevoSensor.GetComponent<IndicadorFuerza>();
        if (indicadorScript == null)
        {
            indicadorScript = nuevoSensor.AddComponent<IndicadorFuerza>();
        }

        // Llamamos a ActualizarSensorDeFuerza para darle su valor inicial
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
                    foreach (var plano in planos) // Añade los planos
                    {
                        lineasPunteadas.CrearLineasPunteadas(plano.transform, sensor.transform);
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
                // Añadir planos aquí
                foreach (var p in planos)
                {
                    lineasPunteadas.CrearLineasPunteadas(p.transform, sensor.transform);
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

            // Guardar posición inicial de cada sensor detalle
            posInicialSensoresDetalle.Clear();
            foreach (var sensor in sensores)
            {
                if (sensor.CompareTag("sensor detalle"))
                {
                    posInicialSensoresDetalle[sensor] = sensor.transform.position;
                }
            }

            // Guardar posición inicial de cada carga, línea y plano
            posInicialCargasLineas.Clear();
            foreach (var c in cargas)
            {
                posInicialCargasLineas[c] = c.transform.position;
            }
            foreach (var l in lineasCarga)
            {
                posInicialCargasLineas[l] = l.transform.position;
            }
            // *** Agregamos los planos:
            foreach (var p in planos)
            {
                posInicialCargasLineas[p] = p.transform.position;
            }

            // Crear flechas de Suma de Cargas para cada fuente
            foreach (var c in cargas)
                sumaDeCargas.CrearOActualizarFlechaParaFuente(c);
            foreach (var l in lineasCarga)
                sumaDeCargas.CrearOActualizarFlechaParaFuente(l);
            // *** Crear flechas para cada plano:
            foreach (var p in planos)
                sumaDeCargas.CrearOActualizarFlechaParaFuente(p);

            // Iniciar la animación (si corresponde)
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

    private void OnSliderFuerzaChanged(float value)
    {
        fuerzaDeseada = value;  // Asigna el valor del slider a la variable fuerzaDeseada

        // Actualiza el texto con el nuevo valor del slider
        if (textoFuerza != null)
        {
            textoFuerza.text = $"Valor de carga: {fuerzaDeseada:F2}";  // Muestra el valor con dos decimales
        }

        Debug.Log($"[Manager] Nuevo valor de carga: {fuerzaDeseada}");
    }

    private void UpdateButtonsColor()
    {
        // Recalcular
        if (modoRecalcularActivo)
            recalcularButton.image.color = selectedColor;
        else
            recalcularButton.image.color = normalColorRecalcular;

        // Suma de Cargas
        if (turnoSumaDeCargasActivo)
            sumaDeCargasButton.image.color = selectedColor;
        else
            sumaDeCargasButton.image.color = normalColorSuma;

        // Vectores Desde Sensor
        if (turnoVectoresDesdeSensorActivo)
            vectoresDesdeSensorButton.image.color = selectedColor;
        else
            vectoresDesdeSensorButton.image.color = normalColorVectores;
    }

    // Método para habilitar/deshabilitar los tres botones (recalcular, suma, vectores)
    private void UpdateButtonsInteractable(bool hasDetail)
    {
        recalcularButton.interactable = hasDetail;
        sumaDeCargasButton.interactable = hasDetail;
        vectoresDesdeSensorButton.interactable = hasDetail;
    }

    public void ActivarSensorDetalleEnCargaCercana()
    {
        foreach (var sensor in sensores)
        {
            if (sensor.CompareTag("sensor detalle"))
            {
                GameObject cargaAsociada = ObtenerCargaMasCercana(sensor.transform.position);
                if (cargaAsociada != null)
                {
                    // Mover sensor a la posición de la carga
                    sensor.transform.position = cargaAsociada.transform.position;

                    // Forzar actualización inmediata
                    CalcularCampoElectricoSobreCarga(sensor, cargaAsociada);
                }
            }
        }
    }
    private GameObject ObtenerCargaMasCercana(Vector3 posicion)
    {
        GameObject cargaCercana = null;
        float minDistancia = Mathf.Infinity;

        foreach (var carga in cargas)
        {
            float distancia = Vector3.Distance(posicion, carga.transform.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                cargaCercana = carga;
            }
        }
        return cargaCercana;
    }
    private void ExcluirDeSumaDeCargas(GameObject cargaExcluida)
    {
        sumaDeCargas.flechasPorFuentePorSensor.Remove(cargaExcluida);  // Elimina la carga de la suma
    }

    private void CalcularCampoElectricoSobreCarga(GameObject sensor, GameObject cargaAsociadaAlSensor)
    {
        Carga scriptCargaSensor = cargaAsociadaAlSensor.GetComponent<Carga>();
        Vector3 campoTotal = CalcularFuerzaTotal(sensor.transform.position, cargaAsociadaAlSensor);
        Vector3 fuerzaFinal = campoTotal * scriptCargaSensor.fuerza * (scriptCargaSensor.esPositiva ? 1 : -1);

        var indicador = sensor.GetComponent<IndicadorFuerza>();
        if (indicador != null)
        {
            indicador.ActualizarDireccion(fuerzaFinal);

            // Cambiar el color del indicador de fuerza cuando se calcule la Ley de Coulomb
            Renderer indicadorRenderer = indicador.GetComponent<Renderer>();
            if (indicadorRenderer != null)
            {
                // Asignar un color cuando se esté calculando la Ley de Coulomb
                indicadorRenderer.material.color = Color.blue;  // Color azul para Ley de Coulomb
            }
        }
    }


    // Método para generar la grilla de sensores
    private void GenerarSensoresEnGrid()
    {
        // Eliminar sensores anteriores del tipo "línea"
        foreach (var sensor in sensores.FindAll(s => s.CompareTag("sensor linea")))
        {
            Destroy(sensor);
        }
        sensores.RemoveAll(s => s.CompareTag("sensor linea"));

        // Generar nuevos sensores
        for (int x = -rango; x <= rango; x++)
        {
            for (int y = -rango; y <= rango; y++)
            {
                for (int z = -rango; z <= rango; z++)
                {
                    Vector3 pos = spawnPoint.position + new Vector3(
                        x * distanciaEntreSensores,
                        y * distanciaEntreSensores,
                        z * distanciaEntreSensores
                    );

                    GameObject sensor = Instantiate(
                        indicadorDireccionPrefab,
                        pos,
                        Quaternion.identity
                    );
                    sensor.tag = "sensor linea";
                    sensores.Add(sensor);
                }
            }
        }
    }

    private void ToggleLineasDeCampo()
    {
        lineasDeCampoActivas = !lineasDeCampoActivas;

        if (lineasDeCampoActivas)
        {
            GenerarSensoresEnGrid();
            lineasDeCampoBtn.image.color = selectedColor;
        }
        else
        {
            // Eliminar solo los sensores de línea
            foreach (var sensor in sensores.FindAll(s => s.CompareTag("sensor linea")))
            {
                Destroy(sensor);
            }
            sensores.RemoveAll(s => s.CompareTag("sensor linea"));
            lineasDeCampoBtn.image.color = normalColorVectores; // O tu color normal
        }
    }
    private void CrearPlano(bool esPositivo)
    {
        GameObject prefab = esPositivo ? planoPositivoPrefab : planoNegativoPrefab;
        GameObject nuevoPlano = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        nuevoPlano.tag = "PlanoFisico"; // Asignar tag identificador
        PlanoCubo scriptPlano = nuevoPlano.GetComponent<PlanoCubo>(); // Cambiado de PlanoFisico1 a PlanoCubo
        if (scriptPlano != null)
        {
            scriptPlano.fuerza = sliderFuerza.value;
            scriptPlano.esPositivo = esPositivo;
        }
        planos.Add(nuevoPlano);
        if (turnoSumaDeCargasActivo) sumaDeCargas.CrearOActualizarFlechaParaFuente(nuevoPlano);
    }
    private void CrearSensorIndividual()
    {
        GameObject nuevoSensor = Instantiate(
            sensorIndividualDireccionPrefab, // Usar prefab específico
            spawnPoint.position,
            Quaternion.identity
        );

        nuevoSensor.tag = "SensorIndividualDireccion"; // Tag único
        sensores.Add(nuevoSensor);
        Debug.Log("Sensor individual creado con funcionalidad de arrastre");
    }

    private void MoverPlanoIzquierda()
    {
        if (planos.Count == 0)
        {
            Debug.Log("No hay planos disponibles para mover.");
            return;
        }

        GameObject planoCercano = null;
        float minDistancia = Mathf.Infinity;

        foreach (GameObject plano in planos)
        {
            float distancia = Vector3.Distance(plano.transform.position, moverPlanoIzquierdaSpawnPoint.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                planoCercano = plano;
            }
        }

        if (planoCercano != null)
        {
            PlanoCubo script = planoCercano.GetComponent<PlanoCubo>();
            if (script != null)
            {
                // Si el plano es NEGATIVO, invertir la dirección
                script.invertirDireccion = !script.esPositivo; // true para negativos, false para positivos
            }
            planoCercano.transform.position = moverPlanoIzquierdaSpawnPoint.position;
            planoCercano.transform.rotation = Quaternion.Euler(90, 0, 0);
            Debug.Log("Plano movido izquierda. Invertir dirección: " + script.invertirDireccion);
        }
    }


    private void MoverPlanoDerecha()
    {
        if (planos.Count == 0)
        {
            Debug.Log("No hay planos disponibles para mover.");
            return;
        }

        GameObject planoCercano = null;
        float minDistancia = Mathf.Infinity;

        foreach (GameObject plano in planos)
        {
            float distancia = Vector3.Distance(plano.transform.position, moverPlanoDerechaSpawnPoint.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                planoCercano = plano;
            }
        }

        if (planoCercano != null)
        {
            PlanoCubo script = planoCercano.GetComponent<PlanoCubo>();
            if (script != null)
            {
                // Invertir dirección si el plano es POSITIVO
                script.invertirDireccion = script.esPositivo; // true para positivos, false para negativos
            }
            planoCercano.transform.position = moverPlanoDerechaSpawnPoint.position;
            planoCercano.transform.rotation = Quaternion.Euler(90, 0, 0);
            Debug.Log("Plano movido derecha. Invertir dirección: " + script.invertirDireccion);
        }
    }

    private void MoverPlanoArriba()
    {
        if (planos.Count == 0)
        {
            Debug.Log("No hay planos disponibles para mover.");
            return;
        }

        GameObject planoCercano = null;
        float minDistancia = Mathf.Infinity;

        foreach (GameObject plano in planos)
        {
            float distancia = Vector3.Distance(plano.transform.position, moverPlanoArribaSpawnPoint.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                planoCercano = plano;
            }
        }

        if (planoCercano != null)
        {
            PlanoCubo script = planoCercano.GetComponent<PlanoCubo>();
            if (script != null)
            {
                // Invertir dirección si el plano es POSITIVO
                script.invertirDireccion = script.esPositivo; // true para positivos, false para negativos
            }
            planoCercano.transform.position = moverPlanoArribaSpawnPoint.position;
            planoCercano.transform.rotation = Quaternion.Euler(0, 0, 0); // Ajuste de rotación
            Debug.Log("Plano movido arriba. Invertir dirección: " + script.invertirDireccion);
        }
    }

    private void MoverPlanoAbajo()
    {
        if (planos.Count == 0)
        {
            Debug.Log("No hay planos disponibles para mover.");
            return;
        }

        GameObject planoCercano = null;
        float minDistancia = Mathf.Infinity;

        foreach (GameObject plano in planos)
        {
            float distancia = Vector3.Distance(plano.transform.position, moverPlanoAbajoSpawnPoint.position);
            if (distancia < minDistancia)
            {
                minDistancia = distancia;
                planoCercano = plano;
            }
        }

        if (planoCercano != null)
        {
            PlanoCubo script = planoCercano.GetComponent<PlanoCubo>();
            if (script != null)
            {
                // Invertir dirección si el plano es NEGATIVO
                script.invertirDireccion = !script.esPositivo; // true para negativos, false para positivos
            }
            planoCercano.transform.position = moverPlanoAbajoSpawnPoint.position;
            planoCercano.transform.rotation = Quaternion.Euler(0, 0, 0); // Ajuste de rotación
            Debug.Log("Plano movido abajo. Invertir dirección: " + script.invertirDireccion);
        }
    }


    public void EliminarPlano(GameObject plano)
    {
        planos.Remove(plano);

        if (sumaDeCargas.flechasPorFuentePorSensor.ContainsKey(plano))
        {
            var dict = sumaDeCargas.flechasPorFuentePorSensor[plano];
            foreach (var flecha in dict.Values)
                Destroy(flecha);
            sumaDeCargas.flechasPorFuentePorSensor.Remove(plano);
        }
        Destroy(plano);
    }

    private void MoverCargasAFormacion(Transform[] spawnPoints)
    {
        if (cargas.Count < spawnPoints.Length)
        {
            Debug.LogWarning($"No hay suficientes cargas. Se necesitan {spawnPoints.Length}");
            return;
        }

        List<GameObject> cargasNoMovidas = new List<GameObject>(cargas);

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject cargaMasCercana = null;
            float minDistancia = Mathf.Infinity;

            // Buscar la carga más cercana no asignada
            foreach (GameObject carga in cargasNoMovidas)
            {
                float distancia = Vector3.Distance(carga.transform.position, spawnPoint.position);
                if (distancia < minDistancia)
                {
                    minDistancia = distancia;
                    cargaMasCercana = carga;
                }
            }

            if (cargaMasCercana != null)
            {
                // Mover y eliminar de la lista de no movidas
                cargaMasCercana.transform.position = spawnPoint.position;
                cargasNoMovidas.Remove(cargaMasCercana);
            }
        }
    }

    public void FormacionLinea()
    {
        MoverCargasAFormacion(spawnPointsLinea);
        Debug.Log("Cargas organizadas en formación lineal");
    }

    public void FormacionTriangulo()
    {
        MoverCargasAFormacion(spawnPointsTriangulo);
        Debug.Log("Cargas organizadas en formación triangular");
    }

    public void FormacionCuadrado()
    {
        MoverCargasAFormacion(spawnPointsCuadrado);
        Debug.Log("Cargas organizadas en formación cuadrada");
    }

    public void FormacionPentagono()
    {
        MoverCargasAFormacion(spawnPointsPentagono);
        Debug.Log("Cargas organizadas en formación pentagonal");
    }

    private void RecalcularFlexometroToggle()
    {
        if (modoFlexometroActivo)
        {
            // Si ya estaba activo, lo desactivamos y destruimos sus líneas
            flexometro.FinalizarMedicion();
            modoFlexometroActivo = false;
        }
        else
        {
            // Si estaba inactivo, lo activamos y creamos sus líneas
            flexometro.IniciarMedicion();
            modoFlexometroActivo = true;
        }
    }



}