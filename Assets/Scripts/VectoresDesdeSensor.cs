using UnityEngine;
using System.Collections.Generic;

public class VectoresDesdeSensor : MonoBehaviour
{
    public GameObject flechaPrefab; // Prefab de la flecha
    public List<GameObject> sensores = new List<GameObject>(); // Lista de sensores
    // Diccionario anidado: para cada fuente (carga o l�nea de carga), se asocia un diccionario que mapea cada sensor a su flecha
    public Dictionary<GameObject, Dictionary<GameObject, GameObject>> flechasPorFuentePorSensor = new Dictionary<GameObject, Dictionary<GameObject, GameObject>>();
    private Dictionary<GameObject, Vector3> posicionesFinalesPorSensor = new Dictionary<GameObject, Vector3>(); // (Opcional) Para almacenar posiciones finales si se requiere

    public float factorEscalaFuerza = 0.1f; // Factor de escala para la longitud de las flechas

    /// <summary>
    /// Crea o actualiza la flecha para una fuente de fuerza (carga o l�nea de carga) en todos los sensores detalle.
    /// </summary>
    /// <param name="fuente">La fuente de fuerza.</param>
    public void CrearOActualizarFlechaParaFuente(GameObject fuente)
    {
        foreach (GameObject sensor in sensores)
        {
            // Considerar solo sensores con el tag "sensor detalle"
            if (!sensor.CompareTag("sensor detalle"))
                continue;

            // Si no existe entrada para esta fuente, se crea un nuevo diccionario
            if (!flechasPorFuentePorSensor.ContainsKey(fuente))
            {
                flechasPorFuentePorSensor[fuente] = new Dictionary<GameObject, GameObject>();
            }
            Dictionary<GameObject, GameObject> flechasPorSensor = flechasPorFuentePorSensor[fuente];

            GameObject flecha;
            if (flechasPorSensor.TryGetValue(sensor, out flecha))
            {
                ActualizarFlechaParaSensor(sensor, fuente, flecha);
            }
            else
            {
                flecha = Instantiate(flechaPrefab);
                flechasPorSensor[sensor] = flecha;
                ActualizarFlechaParaSensor(sensor, fuente, flecha);
            }
        }
    }

    /// <summary>
    /// Actualiza la posici�n, rotaci�n y escala de la flecha para un sensor espec�fico y una fuente dada.
    /// Mantiene la apariencia original: la flecha se posiciona en el sensor, se escala y rota seg�n la fuerza.
    /// </summary>
    /// <param name="sensor">El sensor detalle.</param>
    /// <param name="fuente">La fuente de fuerza.</param>
    /// <param name="flecha">La flecha a actualizar.</param>
    private void ActualizarFlechaParaSensor(GameObject sensor, GameObject fuente, GameObject flecha)
    {
        Carga cargaScript = fuente.GetComponent<Carga>();
        LineaCarga lineaScript = fuente.GetComponent<LineaCarga>();
        PlanoCubo planoScript = fuente.GetComponent<PlanoCubo>();  // Usar tu componente

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
        else if (planoScript != null)  // L�gica para planos
        {
            direccionFuerza = CalcularFuerzaPlano(planoScript, sensor.transform.position);
        }

        // Si la fuerza es cero, no mostramos la flecha
        if (direccionFuerza.magnitude == 0f)
        {
            flecha.SetActive(false);  // Desactiva la flecha
            return;
        }
        else
        {
            flecha.SetActive(true);  // Asegura que la flecha est� activa
        }

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

        flecha.transform.rotation = Quaternion.LookRotation(direccionFuerza) * Quaternion.Euler(90, 0, 0);
        // Se posiciona la flecha en la posici�n del sensor (manteniendo la l�gica original)
        flecha.transform.position = sensor.transform.position;

        // Actualiza el color de la flecha seg�n el tipo de fuente
        Color color = Color.white; // Valor por defecto

        if (cargaScript != null)
        {
            color = cargaScript.esPositiva ? Color.red : Color.blue;
        }
        else if (lineaScript != null)
        {
            color = lineaScript.esPositiva ? Color.magenta : Color.cyan;
        }
        else if (planoScript != null)
        {
            color = planoScript.esPositivo ? new Color(1f, 0.4f, 0.6f) : new Color(0.5f, 0f, 0.5f);
        }

        ActualizarColor(flecha, color);
    }


    private Vector3 CalcularFuerzaCarga(Carga carga, Vector3 posicionSensor)
    {
        Vector3 direccion = posicionSensor - carga.transform.position;
        float distancia = direccion.magnitude;

        if (distancia > 0.01f)
        {
            float fuerzaMagnitud = carga.fuerza / Mathf.Pow(distancia, 2);
            if (!carga.esPositiva)
                fuerzaMagnitud *= -1;
            return fuerzaMagnitud * direccion.normalized;
        }
        return Vector3.zero;
    }

    private Vector3 CalcularFuerzaLinea(LineaCarga linea, Vector3 posicionSensor)
    {
        Collider collider = linea.GetComponent<Collider>();
        if (collider == null)
            return Vector3.zero;

        Vector3 puntoMasCercano = collider.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoMasCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f)
            return Vector3.zero;

        float magnitud = (2 * linea.densidadCarga) / distancia;
        if (!linea.esPositiva)
            magnitud *= -1;

        return magnitud * direccion.normalized;
    }
    private Vector3 CalcularFuerzaPlano(PlanoCubo plano, Vector3 posicionSensor)
    {
        Collider collider = plano.GetComponent<Collider>();
        if (collider == null)
            return Vector3.zero;

        Vector3 puntoCercano = collider.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f)
            return Vector3.zero;

        // Usar inverso cuadrado (igual que en SumaDeCargas)
        float magnitud = 2 * Mathf.PI * plano.fuerza;
        if (!plano.esPositivo)
            magnitud *= -1;

        return magnitud * direccion.normalized;
    }

    private void ActualizarColor(GameObject flecha, Color color)
    {
        Renderer[] renderers = flecha.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    /// <summary>
    /// Actualiza todas las flechas para cada fuente y sensor en LateUpdate.
    /// </summary>
    private void LateUpdate()
    {
        foreach (var fuenteEntry in flechasPorFuentePorSensor)
        {
            GameObject fuente = fuenteEntry.Key;
            Dictionary<GameObject, GameObject> flechasPorSensor = fuenteEntry.Value;
            foreach (var sensorEntry in flechasPorSensor)
            {
                GameObject sensor = sensorEntry.Key;
                GameObject flecha = sensorEntry.Value;
                ActualizarFlechaParaSensor(sensor, fuente, flecha);
            }
        }
    }

    /// <summary>
    /// Elimina todas las flechas creadas.
    /// </summary>
    public void EliminarTodasLasFlechas()
    {
        List<GameObject> flechasAEliminar = new List<GameObject>();
        foreach (var fuenteEntry in flechasPorFuentePorSensor)
        {
            foreach (var sensorEntry in fuenteEntry.Value)
            {
                if (sensorEntry.Value != null && sensorEntry.Value.activeInHierarchy)
                {
                    flechasAEliminar.Add(sensorEntry.Value);
                }
            }
        }
        flechasPorFuentePorSensor.Clear();

        foreach (var flecha in flechasAEliminar)
        {
            if (flecha != null)
            {
                flecha.SetActive(false);
                Destroy(flecha);
            }
        }
    }
}