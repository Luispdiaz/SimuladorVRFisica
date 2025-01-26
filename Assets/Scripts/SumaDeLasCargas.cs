using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SumaDeLasCargasManager : MonoBehaviour
{
    public Button botonSumaCargas;
    public GameObject prefabIndicadorFuerzaIndividual; // Prefab del indicador individual
    public Transform posicionSensor; // Posición del sensor

    private List<IndicadorFuerzaIndividual> indicadoresFuerzaIndividuales = new List<IndicadorFuerzaIndividual>();

    void Start()
    {
        if (botonSumaCargas != null)
        {
            botonSumaCargas.onClick.AddListener(CalcularSumaCargas);
        }
    }

    void CalcularSumaCargas()
    {
        // Limpiar los indicadores anteriores
        foreach (var indicador in indicadoresFuerzaIndividuales)
        {
            Destroy(indicador.gameObject);
        }
        indicadoresFuerzaIndividuales.Clear();

        // Crear nuevos indicadores para cada carga
        foreach (var carga in FindObjectsOfType<Carga>())
        {
            // Crear un nuevo indicador
            GameObject nuevoIndicador = Instantiate(prefabIndicadorFuerzaIndividual, transform);
            IndicadorFuerzaIndividual indicadorScript = nuevoIndicador.GetComponent<IndicadorFuerzaIndividual>();

            // Calcular la fuerza individual
            Vector3 fuerza = carga.fuerza * (carga.esPositiva ? Vector3.one : -Vector3.one); // Ajusta según cómo se define la fuerza

            // Actualizar la dirección del indicador
            indicadorScript.ActualizarDireccion(fuerza, carga.transform.position, posicionSensor.position);

            // Guardar el indicador en la lista
            indicadoresFuerzaIndividuales.Add(indicadorScript);
        }
    }
}
