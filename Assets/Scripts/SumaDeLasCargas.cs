using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SumaDeLasCargasManager : MonoBehaviour
{
    public Button botonSumaCargas;
    public GameObject prefabIndicadorFuerzaIndividual; // Prefab del indicador individual

    private List<GameObject> indicadoresFuerzaIndividuales = new List<GameObject>();

    void Start()
    {
        if (botonSumaCargas != null)
        {
            botonSumaCargas.onClick.AddListener(CrearIndicadoresIndividuales);
        }
        else
        {
            Debug.LogError("No se encontró el botón Suma de las cargas.");
        }
    }

    private void CrearIndicadoresIndividuales()
    {
        // Limpiar los indicadores anteriores
        foreach (var indicador in indicadoresFuerzaIndividuales)
        {
            Destroy(indicador);
        }
        indicadoresFuerzaIndividuales.Clear();

        // Crear nuevos indicadores para cada carga
        foreach (var carga in Object.FindObjectsByType<Carga>(FindObjectsSortMode.None))
        {
            // Crear un nuevo indicador
            GameObject nuevoIndicador = Instantiate(prefabIndicadorFuerzaIndividual, carga.transform.position, Quaternion.identity);
            IndicadorFuerzaIndividual indicadorScript = nuevoIndicador.GetComponent<IndicadorFuerzaIndividual>();

            // Calcular la fuerza individual basada en la posición del sensor general
            Vector3 fuerza = CalcularFuerzaIndividual(carga, indicadorScript.transform.position);

            // Actualizar la dirección del indicador
            indicadorScript.ActualizarDireccion(fuerza, carga.transform.position, indicadorScript.transform.position, carga.esPositiva);

            // Guardar el indicador en la lista
            indicadoresFuerzaIndividuales.Add(nuevoIndicador);
        }
    }

    private Vector3 CalcularFuerzaIndividual(Carga carga, Vector3 posicionSensor)
    {
        Vector3 direccion = posicionSensor - carga.transform.position;
        float distancia = direccion.magnitude;
        if (distancia > 0.01f) // Evitar división por cero
        {
            float fuerzaMagnitud = carga.fuerza / Mathf.Pow(distancia, 2);
            if (!carga.esPositiva)
            {
                fuerzaMagnitud = -fuerzaMagnitud; // Invertir la fuerza para cargas negativas
            }
            return fuerzaMagnitud * direccion.normalized;
        }
        return Vector3.zero;
    }
}
