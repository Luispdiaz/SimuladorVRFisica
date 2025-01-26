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

            // Calcular la fuerza individual
            Vector3 fuerza = carga.fuerza * (carga.esPositiva ? Vector3.one : -Vector3.one);

            // Actualizar la dirección del indicador
            indicadorScript.ActualizarDireccion(fuerza, carga.transform.position, carga.transform.position, carga.esPositiva);

            // Guardar el indicador en la lista
            indicadoresFuerzaIndividuales.Add(nuevoIndicador);
        }
    }
}
