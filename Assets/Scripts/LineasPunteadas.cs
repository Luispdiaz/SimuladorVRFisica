using UnityEngine;
using System.Collections.Generic;

public class LineasPunteadas : MonoBehaviour
{
    public GameObject miniSpherePrefab;
    public int numberOfPoints = 30;

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
        // Eliminar líneas existentes entre estos mismos objetos
        EliminarLinea(start, end);

        // Crear nueva línea
        Linea nuevaLinea = new Linea
        {
            start = start,
            end = end,
            puntos = new List<GameObject>()
        };

        // Generar puntos iniciales
        for (int i = 0; i <= numberOfPoints; i++)
        {
            GameObject punto = Instantiate(miniSpherePrefab);
            nuevaLinea.puntos.Add(punto);
        }

        lineas.Add(nuevaLinea);
    }

    private void EliminarLinea(Transform start, Transform end)
    {
        // Buscar y eliminar líneas existentes entre estos transforms
        lineas.RemoveAll(linea =>
        {
            if ((linea.start == start && linea.end == end) ||
                (linea.start == end && linea.end == start))
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
        // Actualizar todas las líneas cada frame
        foreach (Linea linea in lineas)
        {
            if (linea.start == null || linea.end == null) continue;

            for (int i = 0; i <= numberOfPoints; i++)
            {
                float t = (float)i / numberOfPoints;
                Vector3 posicion = Vector3.Lerp(
                    linea.start.position,
                    linea.end.position,
                    t
                );
                linea.puntos[i].transform.position = posicion;
            }
        }
    }

    public void EliminarLineasDeCarga(Transform carga)
    {
        // Eliminar todas las líneas asociadas a una carga
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