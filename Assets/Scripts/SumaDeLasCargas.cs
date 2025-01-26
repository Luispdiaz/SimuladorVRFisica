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

        // Buscar exactamente un IndicadorFuerzaGeneral
        var indicadoresGenerales = Object.FindObjectsByType<IndicadorFuerza>(FindObjectsSortMode.None);
        if (indicadoresGenerales.Length == 0)
        {
            Debug.LogWarning("No se encontró ningún IndicadorFuerzaGeneral en la escena.");
            return;
        }

        if (indicadoresGenerales.Length > 1)
        {
            Debug.LogWarning("Hay más de un IndicadorFuerzaGeneral en la escena. Usando solo el primero.");
        }

        // Usar el primer IndicadorFuerzaGeneral
        var indicadorGeneral = indicadoresGenerales[0];

        // Buscar todas las cargas en la escena
        var cargas = Object.FindObjectsByType<Carga>(FindObjectsSortMode.None);
        if (cargas.Length == 0)
        {
            Debug.LogWarning("No se encontraron cargas en la escena.");
            return;
        }

        // Crear un indicador por cada carga
        foreach (var carga in cargas)
        {
            Debug.Log($"Creando indicador entre {indicadorGeneral.transform.position} y {carga.transform.position}");

            // Crear un nuevo indicador individual
            GameObject nuevoIndicador = Instantiate(prefabIndicadorFuerzaIndividual, indicadorGeneral.transform.position, Quaternion.identity);
            IndicadorFuerzaIndividual indicadorScript = nuevoIndicador.GetComponent<IndicadorFuerzaIndividual>();

            if (indicadorScript == null)
            {
                Debug.LogError("El prefab no tiene asignado el script IndicadorFuerzaIndividual.");
                continue;
            }

            // Calcular la fuerza desde el indicador general hacia la carga
            Vector3 fuerza = CalcularFuerza(indicadorGeneral.transform.position, carga.transform.position, carga.fuerza, carga.esPositiva);
            Debug.Log($"Fuerza calculada: {fuerza}");

            // Actualizar el indicador individual para que apunte correctamente
            indicadorScript.ActualizarDireccion(
                fuerza,
                carga.transform.position,
                indicadorGeneral.transform.position,
                carga.esPositiva
            );

            // Guardar el indicador en la lista
            indicadoresFuerzaIndividuales.Add(nuevoIndicador);
        }
    }

    /// <summary>
    /// Calcula la fuerza vectorial entre un punto origen (indicador) y un punto destino (carga).
    /// </summary>
    private Vector3 CalcularFuerza(Vector3 origen, Vector3 destino, float magnitudCarga, bool esPositiva)
    {
        Vector3 direccion = destino - origen; // Vector desde el indicador hacia la carga
        float distancia = direccion.magnitude;

        if (distancia > 0.01f) // Evitar división por cero
        {
            float fuerzaMagnitud = magnitudCarga / Mathf.Pow(distancia, 2);
            if (!esPositiva)
            {
                fuerzaMagnitud = -fuerzaMagnitud; // Invertir fuerza para cargas negativas
            }
            return fuerzaMagnitud * direccion.normalized;
        }
        return Vector3.zero;
    }
}
