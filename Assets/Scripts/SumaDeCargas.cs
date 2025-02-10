using UnityEngine;
using System.Collections.Generic;

public class SumaDeCargas : MonoBehaviour
{
    public GameObject flechaPrefab;
    public List<GameObject> sensores = new List<GameObject>();
    public Dictionary<GameObject, GameObject> flechasPorCarga = new Dictionary<GameObject, GameObject>();

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
            // 1. Calcular dirección y distancia
            Vector3 direccion = cargaScript.esPositiva
                ? (sensor.transform.position - carga.transform.position).normalized
                : (carga.transform.position - sensor.transform.position).normalized;

            float distancia = Vector3.Distance(carga.transform.position, sensor.transform.position);

            // 2. Calcular fuerza
            float fuerza = Mathf.Abs(cargaScript.fuerza) / Mathf.Pow(distancia, 2);

            // 3. Ajustar escala del cilindro (eje Y)
            Transform cuerpo = flecha.transform.Find("Cuerpo");
            Transform punta = flecha.transform.Find("Punta");
            if (cuerpo != null)
            {
                Vector3 nuevaEscala = cuerpo.localScale;
                nuevaEscala.y = fuerza * factorEscalaFuerza;
                cuerpo.localScale = nuevaEscala;

                // Posición del cilindro ajustada al centro de su altura
                cuerpo.localPosition = new Vector3(0, nuevaEscala.y / 2f, 0);

                // Posicionar la punta al final del cilindro (en coordenadas locales)
                if (punta != null)
                {
                    punta.localPosition = new Vector3(0, nuevaEscala.y, 0); // 100% de la altura del cilindro
                }
            }


            // 4. Aplicar rotación corregida (90° en X + dirección)
            Quaternion rotacionBase = Quaternion.LookRotation(direccion) * Quaternion.Euler(90, 0, 0);
            flecha.transform.rotation = rotacionBase;

            // 5. Invertir dirección para cargas negativas (180° en Y)
            if (!cargaScript.esPositiva)
            {
                flecha.transform.Rotate(0, 180f, 0, Space.Self); // Rotación local
            }

            // 6. Posicionamiento
            float factorPosicion = cargaScript.esPositiva ? 0.3f : 0.7f;
            flecha.transform.position = Vector3.Lerp(
                carga.transform.position,
                sensor.transform.position,
                factorPosicion
            );

            // 7. Color
            ActualizarColor(flecha, cargaScript.esPositiva ? Color.red : Color.blue);
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