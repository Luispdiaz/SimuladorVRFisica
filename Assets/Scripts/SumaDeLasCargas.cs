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
        // Buscar el objeto IndicadorFuerza por nombre en la escena
        GameObject objetoIndicadorFuerza = GameObject.Find("IndicadorFuerzaGeneral");
        if (objetoIndicadorFuerza != null)
        {
            indicadorFuerza = objetoIndicadorFuerza.GetComponent<IndicadorFuerza>();
            if (indicadorFuerza != null)
            {
                Debug.Log("IndicadorFuerza encontrado.");
            }
            else
            {
                Debug.LogError("El objeto IndicadorFuerzaGeneral no tiene el script IndicadorFuerza.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto IndicadorFuerzaGeneral en la escena.");
        }

        if (botonSumaCargas != null)
        {
            Debug.Log("Asignando evento de clic al botón.");
            botonSumaCargas.onClick.AddListener(CalcularSumaCargas);
        }
        else
        {
            Debug.LogError("No se encontró el botón Suma de las cargas.");
        }
    }

    void CalcularSumaCargas()
    {
        Debug.Log("Botón Suma de las cargas presionado.");

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
