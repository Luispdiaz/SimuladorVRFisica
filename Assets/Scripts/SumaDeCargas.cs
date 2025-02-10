using UnityEngine;
using System.Collections.Generic;

public class SumaDeCargas : MonoBehaviour
{
    public GameObject flechaPrefab;
    public List<GameObject> sensores = new List<GameObject>();
    public Dictionary<GameObject, GameObject> flechasPorCarga = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, Vector3> posicionesFinalesPorSensor = new Dictionary<GameObject, Vector3>();

    public float factorEscalaFuerza = 0.1f;

    public void CrearOActualizarFlechaParaCarga(GameObject carga)
    {
        GameObject flecha;

        if (flechasPorCarga.TryGetValue(carga, out flecha))
        {
            ActualizarFlecha(flecha, carga);
        }
        else
        {
            flecha = Instantiate(flechaPrefab);
            flechasPorCarga[carga] = flecha;
            ActualizarFlecha(flecha, carga);
        }
    }

    private GameObject ObtenerSensorMasCercano(GameObject carga)
    {
        GameObject nearestSensor = null;
        float minDistance = Mathf.Infinity;

        foreach (var sensor in sensores)
        {
            float distance = Vector3.Distance(carga.transform.position, sensor.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSensor = sensor;
            }
        }
        return nearestSensor;
    }

    private void ActualizarFlecha(GameObject flecha, GameObject carga)
    {
        GameObject sensor = ObtenerSensorMasCercano(carga);
        Carga cargaScript = carga.GetComponent<Carga>();

        if (sensor != null && cargaScript != null)
        {
            Vector3 direccionFuerza = CalcularFuerzaIndividual(carga, sensor.transform.position);
            float magnitudFuerza = direccionFuerza.magnitude;
            float longitudFlecha = 0f;

            Transform cuerpo = flecha.transform.Find("Cuerpo");
            Transform punta = flecha.transform.Find("Punta");

            if (cuerpo != null)
            {
                // 1. Calcular escala a MITAD de la longitud deseada (por el pivote central)
                Vector3 nuevaEscala = cuerpo.localScale;
                nuevaEscala.y = (magnitudFuerza * factorEscalaFuerza) * 0.5f; // <-- Mitad de la longitud real
                cuerpo.localScale = nuevaEscala;

                // 2. Compensar posición (el cilindro crecerá el doble desde el centro)
                cuerpo.localPosition = new Vector3(0, nuevaEscala.y, 0); // Mover solo 1/4 de la longitud total

                if (punta != null)
                {
                    // 3. Posicionar punta en el extremo final real
                    punta.localPosition = new Vector3(0, nuevaEscala.y * 2, 0); // Doble de la escala
                }

                // 4. Longitud real = escala * 2 (por el pivote central)
                longitudFlecha = nuevaEscala.y * 2;
            }

            flecha.transform.rotation = Quaternion.LookRotation(direccionFuerza) * Quaternion.Euler(90, 0, 0);

            Vector3 posicionInicial = posicionesFinalesPorSensor.ContainsKey(sensor) ?
                                    posicionesFinalesPorSensor[sensor] :
                                    sensor.transform.position;

            flecha.transform.position = posicionInicial;
            posicionesFinalesPorSensor[sensor] = posicionInicial + (direccionFuerza.normalized * longitudFlecha);

            ActualizarColor(flecha, cargaScript.esPositiva ? Color.red : Color.blue);
        }
    }

    private Vector3 CalcularFuerzaIndividual(GameObject carga, Vector3 posicionSensor)
    {
        Carga cargaScript = carga.GetComponent<Carga>();
        Vector3 direccion = posicionSensor - carga.transform.position;
        float distancia = direccion.magnitude;

        if (distancia > 0.01f)
        {
            float fuerzaMagnitud = cargaScript.fuerza / Mathf.Pow(distancia, 2);
            if (!cargaScript.esPositiva)
            {
                fuerzaMagnitud = -fuerzaMagnitud;
            }
            return fuerzaMagnitud * direccion.normalized;
        }
        return Vector3.zero;
    }

    private void LateUpdate()
    {
        // Reiniciar posiciones acumuladas cada frame
        posicionesFinalesPorSensor.Clear();

        // Actualizar todas las flechas en orden de influencia
        foreach (var par in flechasPorCarga)
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
        foreach (var flecha in flechasPorCarga.Values)
        {
            Destroy(flecha);
        }
        flechasPorCarga.Clear();
    }
}