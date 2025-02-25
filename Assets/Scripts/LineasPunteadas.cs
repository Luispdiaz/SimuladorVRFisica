using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineasPunteadas : MonoBehaviour
{
    public GameObject miniSpherePrefab;
    public int numberOfPoints = 30;
    public float delayTime = 2f; // Delay entre la creación de cada punto

    // Clase para almacenar la información de cada línea
    private class Linea
    {
        public Transform start;
        public Transform end;
        public List<GameObject> puntos;
    }

    private List<Linea> lineas = new List<Linea>();

    public void CrearLineasPunteadas(Transform start, Transform end)
    {
        // Solo crear la línea si el 'end' es un sensor detalle
        if (!end.CompareTag("sensor detalle")) return;

        // Eliminar cualquier línea existente entre estos mismos objetos
        EliminarLinea(start, end);

        Linea nuevaLinea = new Linea
        {
            start = start,
            end = end,
            puntos = new List<GameObject>()
        };

        lineas.Add(nuevaLinea);
        StartCoroutine(CrearPuntosConDelay(nuevaLinea));
    }

    private IEnumerator CrearPuntosConDelay(Linea linea)
    {
        for (int i = 0; i <= numberOfPoints; i++)
        {
            GameObject punto = Instantiate(miniSpherePrefab);
            linea.puntos.Add(punto);
            yield return new WaitForSeconds(delayTime);
        }
    }

    private void EliminarLinea(Transform start, Transform end)
    {
        lineas.RemoveAll(linea =>
        {
            if ((linea.start == start && linea.end == end) || (linea.start == end && linea.end == start))
            {
                foreach (GameObject punto in linea.puntos)
                {
                    Destroy(punto);
                }
                return true;
            }
            return false;
        });
    }

    void Update()
    {
        foreach (Linea linea in lineas)
        {
            if (linea.start == null || linea.end == null) continue;

            Vector3 startPosition;
            Vector3 endPosition = linea.end.position; // Centro del sensor

            // Determinar tipo de objeto inicial
            bool esCarga = linea.start.GetComponent<Carga>() != null;
            Collider startCollider = linea.start.GetComponent<Collider>();

            if (esCarga)
            {
                // Cargas: centro del collider
                startPosition = startCollider != null ?
                    startCollider.bounds.center :
                    linea.start.position;
            }
            else if (startCollider != null)
            {
                // Líneas/Planos: punto más cercano al sensor
                startPosition = startCollider.ClosestPoint(endPosition);
            }
            else
            {
                // Caso por defecto
                startPosition = linea.start.position;
            }

            Vector3 direction = (endPosition - startPosition).normalized;
            Quaternion lineRotation = Quaternion.LookRotation(direction);

            // Actualizar puntos
            for (int i = 0; i <= numberOfPoints; i++)
            {
                float t = (float)i / numberOfPoints;
                Vector3 posicion = Vector3.Lerp(startPosition, endPosition, t);

                if (i < linea.puntos.Count && linea.puntos[i] != null)
                {
                    linea.puntos[i].transform.position = posicion;
                    linea.puntos[i].transform.rotation = lineRotation;
                }
            }
        }
    }

    public void EliminarLineasDeCarga(Transform carga)
    {
        lineas.RemoveAll(linea =>
        {
            if (linea.start == carga || linea.end == carga)
            {
                foreach (GameObject punto in linea.puntos)
                {
                    Destroy(punto);
                }
                return true;
            }
            return false;
        });
    }

    public void EliminarTodasLasLineas()
    {
        foreach (Linea linea in lineas)
        {
            foreach (GameObject punto in linea.puntos)
            {
                Destroy(punto);
            }
        }
        lineas.Clear();
    }
}
