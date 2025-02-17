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
        // Actualiza la posición de cada punto en todas las líneas cada frame
        foreach (Linea linea in lineas)
        {
            if (linea.start == null || linea.end == null) continue;

            // Para las cargas (punto) se utiliza directamente la posición del transform.
            // Para otros objetos (por ejemplo, cilindros), se usa el collider para obtener el punto más cercano.
            Vector3 startPoint;
            if (linea.start.GetComponent<Carga>() != null)
            {
                startPoint = linea.start.position;
            }
            else
            {
                Collider col = linea.start.GetComponent<Collider>();
                if (col != null)
                    startPoint = col.ClosestPoint(linea.end.position);
                else
                    startPoint = linea.start.position;
            }

            for (int i = 0; i <= numberOfPoints; i++)
            {
                float t = (float)i / numberOfPoints;
                Vector3 posicion = Vector3.Lerp(startPoint, linea.end.position, t);
                if (i < linea.puntos.Count && linea.puntos[i] != null)
                    linea.puntos[i].transform.position = posicion;
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
