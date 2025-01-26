using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SumaDeLasCargasManager : MonoBehaviour
{
    public Button botonSumaCargas;
    public GameObject prefabIndicadorFuerzaIndividual; // Prefab del indicador individual

    private IndicadorFuerza indicadorFuerza;
    private List<IndicadorFuerzaIndividual> indicadoresFuerzaIndividuales = new List<IndicadorFuerzaIndividual>();

    void Start()
    {
        // Buscar el objeto IndicadorFuerza en la escena
        indicadorFuerza = Object.FindFirstObjectByType<IndicadorFuerza>();
        if (indicadorFuerza != null)
        {
            Debug.Log("IndicadorFuerza encontrado.");
        }
        else
        {
            Debug.LogError("No se encontró un objeto IndicadorFuerza en la escena.");
        }

        if (botonSumaCargas != null)
        {
            botonSumaCargas.onClick.AddListener(CalcularSumaCargas);
        }
    }

    void CalcularSumaCargas()
    {
        if (indicadorFuerza == null)
        {
            Debug.LogError("No se encontró un objeto IndicadorFuerza en la escena.");
            return;
        }

        // Limpiar los indicadores anteriores
        foreach (var indicador in indicadoresFuerzaIndividuales)
        {
            Destroy(indicador.gameObject);
        }
        indicadoresFuerzaIndividuales.Clear();

        // Obtener la posición del sensor
        Vector3 posicionSensor = indicadorFuerza.transform.position;
        Debug.Log($"Posición del sensor: {posicionSensor}");

        // Crear nuevos indicadores para cada carga
        foreach (var carga in Object.FindObjectsByType<Carga>(FindObjectsSortMode.None))
        {
            Debug.Log($"Encontrada carga en posición: {carga.transform.position}");

            // Crear un nuevo indicador
            GameObject nuevoIndicador = Instantiate(prefabIndicadorFuerzaIndividual, transform);
            IndicadorFuerzaIndividual indicadorScript = nuevoIndicador.GetComponent<IndicadorFuerzaIndividual>();

            // Calcular la fuerza individual
            Vector3 fuerza = carga.fuerza * (carga.esPositiva ? Vector3.one : -Vector3.one); // Ajusta según cómo se define la fuerza
            Debug.Log($"Fuerza calculada: {fuerza}");

            // Actualizar la dirección del indicador
            indicadorScript.ActualizarDireccion(fuerza, carga.transform.position, posicionSensor, carga.esPositiva);

            // Guardar el indicador en la lista
            indicadoresFuerzaIndividuales.Add(indicadorScript);
        }
    }
}
