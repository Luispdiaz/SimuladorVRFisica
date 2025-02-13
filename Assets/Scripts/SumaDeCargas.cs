using UnityEngine;
using System.Collections.Generic;

public class SumaDeCargas : MonoBehaviour
{
    public GameObject flechaPrefab;
    public List<GameObject> sensores = new List<GameObject>();
    public Dictionary<GameObject, GameObject> flechasPorFuente = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, Vector3> posicionesFinalesPorSensor = new Dictionary<GameObject, Vector3>();

    public float factorEscalaFuerza = 0.1f;

    public void CrearOActualizarFlechaParaFuente(GameObject fuente)
    {
        GameObject flecha;

        if (flechasPorFuente.TryGetValue(fuente, out flecha))
        {
            ActualizarFlecha(flecha, fuente);
        }
        else
        {
            flecha = Instantiate(flechaPrefab);
            flechasPorFuente[fuente] = flecha;
            ActualizarFlecha(flecha, fuente);
        }
    }

    private GameObject ObtenerSensorMasCercano(GameObject fuente)
    {
        GameObject nearestSensor = null;
        float minDistance = Mathf.Infinity;

        foreach (var sensor in sensores)
        {
            float distance = Vector3.Distance(fuente.transform.position, sensor.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSensor = sensor;
            }
        }
        return nearestSensor;
    }

    private void ActualizarFlecha(GameObject flecha, GameObject fuente)
    {
        GameObject sensor = ObtenerSensorMasCercano(fuente);
        Carga cargaScript = fuente.GetComponent<Carga>();
        LineaCarga lineaScript = fuente.GetComponent<LineaCarga>();

        if (sensor != null && (cargaScript != null || lineaScript != null))
        {
            Vector3 direccionFuerza = Vector3.zero;

            if (cargaScript != null)
            {
                direccionFuerza = CalcularFuerzaCarga(cargaScript, sensor.transform.position);
            }
            else if (lineaScript != null)
            {
                direccionFuerza = CalcularFuerzaLinea(lineaScript, sensor.transform.position);
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

            Vector3 posicionInicial = posicionesFinalesPorSensor.ContainsKey(sensor)
                                    ? posicionesFinalesPorSensor[sensor]
                                    : sensor.transform.position;

            flecha.transform.position = posicionInicial;
            posicionesFinalesPorSensor[sensor] = posicionInicial + (direccionFuerza.normalized * longitudFlecha);

            ActualizarColor(flecha,
                cargaScript != null
                    ? (cargaScript.esPositiva ? Color.red : Color.blue)
                    : (lineaScript.esPositiva ? Color.magenta : Color.cyan));
        }
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

        float magnitud = linea.densidadCarga / distancia;
        if (!linea.esPositiva) magnitud *= -1;

        return magnitud * direccion.normalized;
    }

    private void LateUpdate()
    {
        posicionesFinalesPorSensor.Clear();
        foreach (var par in flechasPorFuente)
        {
            ActualizarFlecha(par.Value, par.Key);
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
        foreach (var flecha in flechasPorFuente.Values)
        {
            Destroy(flecha);
        }
        flechasPorFuente.Clear();
    }
}