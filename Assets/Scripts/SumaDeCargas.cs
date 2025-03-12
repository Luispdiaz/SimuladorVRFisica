using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

// Nuevo componente para almacenar la dirección original
public class FlechaData : MonoBehaviour
{
    public Vector3 direccion;
}


public class SumaDeCargas : MonoBehaviour
{
    public GameObject flechaPrefab;
    // Lista de sensores (deben tener el tag "sensor detalle")
    public List<GameObject> sensores = new List<GameObject>();
    // Ahora, para cada fuente (carga o línea) se guarda un diccionario que asocia cada sensor a su flecha
    public Dictionary<GameObject, Dictionary<GameObject, GameObject>> flechasPorFuentePorSensor = new Dictionary<GameObject, Dictionary<GameObject, GameObject>>();
    // Para llevar un seguimiento de la posición final de la flecha para cada sensor
    private Dictionary<GameObject, Vector3> posicionesFinalesPorSensor = new Dictionary<GameObject, Vector3>();

    public float factorEscalaFuerza = 0.1f;
    [Header("Animación")]
    public float duracionAnimacion = 3.0f;
    public float retardoEntreFlechas = 0.5f;
    private bool animacionEnCurso = false;
    private bool animacionActiva = false; // Nuevo flag

    public void IniciarAnimacionSuma()
    {
        if (!animacionEnCurso)
        {
            StartCoroutine(AnimacionSumaCoroutine());
        }
    }
    private IEnumerator AnimacionSumaCoroutine()
    {
        animacionEnCurso = true;
        animacionActiva = true;

        yield return new WaitForEndOfFrame();

        foreach (GameObject sensor in sensores)
        {
            if (!sensor.CompareTag("sensor detalle")) continue;

            List<GameObject> flechasDelSensor = new List<GameObject>();
            Vector3 posicionSensor = sensor.transform.position;

            // 1. Recolectar datos de las flechas (dirección y longitud)
            List<Vector3> direcciones = new List<Vector3>();
            List<float> longitudes = new List<float>();
            foreach (var fuente in flechasPorFuentePorSensor.Keys)
            {
                if (flechasPorFuentePorSensor[fuente].TryGetValue(sensor, out GameObject flecha))
                {
                    // Verificar si la flecha y el componente FlechaData no son nulos antes de continuar
                    if (flecha != null)
                    {
                        FlechaData data = flecha.GetComponent<FlechaData>();
                        if (data != null)
                        {
                            flechasDelSensor.Add(flecha);
                            float longitud = flecha.transform.Find("Cuerpo").localScale.y * 2;
                            direcciones.Add(data.direccion.normalized); // Dirección normalizada
                            longitudes.Add(longitud); // Longitud del vector
                        }
                        else
                        {
                            Debug.LogWarning("FlechaData es null en una de las flechas. Asegúrate de que todas las flechas tengan el componente FlechaData.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Flecha es null. No se puede acceder a los datos de esta flecha.");
                    }
                }
            }

            // 2. Calcular posiciones finales acumuladas
            List<Vector3> posicionesFinales = new List<Vector3>();
            Vector3 acumulador = Vector3.zero; // Acumula el desplazamiento total
            for (int i = 0; i < direcciones.Count; i++)
            {
                acumulador += direcciones[i] * longitudes[i]; // Suma vectorial
                posicionesFinales.Add(posicionSensor + acumulador);
            }

            // 3. Resetear todas las flechas al sensor
            foreach (var flecha in flechasDelSensor)
            {
                if (flecha != null)
                {
                    flecha.transform.position = posicionSensor;
                }
            }

            // 4. Animar secuencialmente desde el sensor
            for (int i = 1; i < flechasDelSensor.Count; i++)
            {
                GameObject flecha = flechasDelSensor[i];
                if (flecha != null)
                {
                    Vector3 fin = posicionesFinales[i - 1];

                    if (i == 0)
                    {
                        // Primera flecha estática
                        flecha.transform.position = fin;
                    }
                    else
                    {
                        // Flechas posteriores: animar desde el sensor hasta la posición acumulada
                        yield return StartCoroutine(AnimarFlecha(
                            flecha.transform,
                            posicionSensor, // Siempre inician en el sensor
                            fin,
                            flecha.transform.rotation
                        ));
                    }
                }
            }
        }

        animacionActiva = false;
        animacionEnCurso = false;
    }

    private IEnumerator AnimarFlecha(Transform flecha, Vector3 inicio, Vector3 fin, Quaternion rotacion)
    {
        if (flecha == null) yield break; // Salir si la flecha fue destruida

        float tiempo = 0;
        flecha.rotation = rotacion;

        while (tiempo < duracionAnimacion)
        {
            if (flecha == null) yield break; // Validar en cada frame
            float t = Mathf.SmoothStep(0f, 1f, tiempo / duracionAnimacion);
            flecha.position = Vector3.Lerp(inicio, fin, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        if (flecha != null)
            flecha.position = fin;
    }
    /// <summary>
    /// Crea o actualiza las flechas para la fuente en todos los sensores detalle.
    /// </summary>
    public void CrearOActualizarFlechaParaFuente(GameObject fuente)
    {
        // Iteramos sobre todos los sensores
        foreach (GameObject sensor in sensores)
        {
            // Solo consideramos sensores con el tag "sensor detalle"
            if (!sensor.CompareTag("sensor detalle"))
                continue;

            // Si no existe una entrada para esta fuente en el diccionario, la creamos
            if (!flechasPorFuentePorSensor.ContainsKey(fuente))
            {
                flechasPorFuentePorSensor[fuente] = new Dictionary<GameObject, GameObject>();
            }

            Dictionary<GameObject, GameObject> flechasPorSensor = flechasPorFuentePorSensor[fuente];

            // Revisamos si ya existe una flecha para este (fuente, sensor)
            GameObject flecha;
            if (flechasPorSensor.TryGetValue(sensor, out flecha))
            {
                // Si existe, solo la actualizamos
                ActualizarFlechaParaSensor(sensor, fuente, flecha);
            }
            else
            {
                // Si no existe, la creamos y luego la actualizamos
                flecha = Instantiate(flechaPrefab);
                flechasPorSensor[sensor] = flecha;
                ActualizarFlechaParaSensor(sensor, fuente, flecha);
            }
        }
    }

    /// <summary>
    /// Actualiza la flecha para un sensor específico y una fuente dada.
    /// </summary>
    private void ActualizarFlechaParaSensor(GameObject sensor, GameObject fuente, GameObject flecha)
    {
        Carga cargaScript = fuente.GetComponent<Carga>();
        LineaCarga lineaScript = fuente.GetComponent<LineaCarga>();
        PlanoCubo planoScript = fuente.GetComponent<PlanoCubo>(); // Nuevo: Componente Plano

        if (cargaScript == null && lineaScript == null && planoScript == null)
            return;

        Vector3 direccionFuerza = Vector3.zero;
        if (cargaScript != null)
        {
            direccionFuerza = CalcularFuerzaCarga(cargaScript, sensor.transform.position);
        }
        else if (lineaScript != null)
        {
            direccionFuerza = CalcularFuerzaLinea(lineaScript, sensor.transform.position);
        }
        else if (planoScript != null) // Nuevo: Caso para planos
        {
            direccionFuerza = CalcularFuerzaPlano(planoScript, sensor.transform.position);
        }

        // Si la fuerza es cero (el sensor está en la misma posición que la carga), no mostramos la flecha
        if (direccionFuerza.magnitude <= 0.01f)
        {
            flecha.SetActive(false); // Desactivamos la flecha si la fuerza es cero
            return;
        }

        // Si la fuerza no es cero, activamos la flecha
        flecha.SetActive(true);

        // Ajustar escala y posición de las partes de la flecha
        float magnitudFuerza = direccionFuerza.magnitude;
        float longitudFlecha = 0f;

        Transform cuerpo = flecha.transform.Find("Cuerpo");
        Transform punta = flecha.transform.Find("Punta");

        if (cuerpo != null)
        {
            Vector3 nuevaEscala = cuerpo.localScale;
            nuevaEscala.y = (magnitudFuerza * factorEscalaFuerza) * 0.5f;
            cuerpo.localScale = nuevaEscala;
            cuerpo.localPosition = new Vector3(0, nuevaEscala.y, 0);

            if (punta != null)
            {
                float posicionYPunta = (nuevaEscala.y * 2) - 0.073f;
                punta.localPosition = new Vector3(0, posicionYPunta, 0);
            }

            longitudFlecha = nuevaEscala.y * 2;
        }

        // Establecer la rotación de la flecha
        flecha.transform.rotation = Quaternion.LookRotation(direccionFuerza) * Quaternion.Euler(90, 0, 0);

        // Utilizar una posición inicial para la flecha basada en el sensor
        Vector3 posicionInicial = posicionesFinalesPorSensor.ContainsKey(sensor)
                                        ? posicionesFinalesPorSensor[sensor]
                                        : sensor.transform.position;
        // Actualizar la posición final para este sensor
        posicionesFinalesPorSensor[sensor] = posicionInicial + (direccionFuerza.normalized * longitudFlecha);

        FlechaData data = flecha.GetComponent<FlechaData>();
        if (data == null) data = flecha.AddComponent<FlechaData>();
        data.direccion = direccionFuerza.normalized;

        // Actualizar el color de la flecha según el tipo de carga o línea
        Color color;
        if (cargaScript != null)
            color = cargaScript.esPositiva ? Color.red : Color.blue;
        else if (lineaScript != null)
            color = lineaScript.esPositiva ? Color.magenta : Color.cyan;
        else
            color = planoScript.esPositivo ? new Color(1f, 0.4f, 0.6f) : new Color(0.5f, 0f, 0.5f);  // Usar tu variable

        ActualizarColor(flecha, color);
    }

    private Vector3 CalcularFuerzaCarga(Carga carga, Vector3 posicionSensor)
    {
        Vector3 direccion = posicionSensor - carga.transform.position;
        float distancia = direccion.magnitude;

        if (distancia > 0.01f)
        {
            float fuerzaMagnitud = carga.fuerza / Mathf.Pow(distancia, 2);
            if (!carga.esPositiva) fuerzaMagnitud *= -1;
            return fuerzaMagnitud * direccion.normalized;
        }
        return Vector3.zero;
    }

    private Vector3 CalcularFuerzaLinea(LineaCarga linea, Vector3 posicionSensor)
    {
        Collider collider = linea.GetComponent<Collider>();
        if (collider == null) return Vector3.zero;

        Vector3 puntoMasCercano = collider.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoMasCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f) return Vector3.zero;

        float magnitud = (2 * linea.densidadCarga) / distancia;
        if (!linea.esPositiva) magnitud *= -1;

        return magnitud * direccion.normalized;
    }
    private Vector3 CalcularFuerzaPlano(PlanoCubo plano, Vector3 posicionSensor)
    {
        Collider collider = plano.GetComponent<Collider>();
        if (collider == null) return Vector3.zero;

        Vector3 puntoCercano = collider.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f) return Vector3.zero;

        float magnitud = 2 * Mathf.PI * plano.fuerza;// <--- Ajuste aquí
        if (!plano.esPositivo) magnitud *= -1;

        return magnitud * direccion.normalized;
    }
    /// <summary>
    /// Se actualizan todas las flechas para todas las fuentes y sensores.
    /// </summary>
 

    public void LimpiarFuentesInvalidas()
    {
        var fuentesInvalidas = flechasPorFuentePorSensor
            .Where(entry => entry.Key == null)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var fuente in fuentesInvalidas)
        {
            flechasPorFuentePorSensor.Remove(fuente);
        }
    }

    private void ActualizarColor(GameObject flecha, Color color)
    {
        Renderer[] renderers = flecha.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void EliminarTodasLasFlechas()
    {
        // Recorre todas las flechas almacenadas y destrúyelas
        foreach (var fuenteEntry in flechasPorFuentePorSensor)
        {
            foreach (var sensorEntry in fuenteEntry.Value)
            {
                if (sensorEntry.Value != null)
                {
                    Destroy(sensorEntry.Value);
                }
            }
        }
        // Limpia completamente el diccionario
        flechasPorFuentePorSensor.Clear();
        posicionesFinalesPorSensor.Clear();
    }

}