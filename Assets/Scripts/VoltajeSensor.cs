using UnityEngine;
using TMPro;

public class VoltageSensor : MonoBehaviour
{
    public TextMeshPro textoVoltaje;
    private CargaPuntualManager managerCargas;

    private void Start()
    {
        managerCargas = FindObjectOfType<CargaPuntualManager>();
        if (textoVoltaje == null)
        {
            textoVoltaje = GetComponentInChildren<TextMeshPro>();
        }
    }

    public void ActualizarVoltaje() // Método público para ser llamado desde el Manager
    {
        float voltajeTotal = CalcularVoltajeEnPunto(transform.position);
        textoVoltaje.text = $"V = {voltajeTotal.ToString("F2")} V";
    }

    private float CalcularVoltajeEnPunto(Vector3 posicionSensor)
    {
        float k = 1f; // Usa 1 para simplificar (ajusta según tu escala)
        float voltaje = 0f;

        // Contribución de cargas puntuales
        foreach (var carga in managerCargas.Cargas)
        {
            Carga scriptCarga = carga.GetComponent<Carga>();
            if (scriptCarga != null)
            {
                float distancia = Vector3.Distance(posicionSensor, carga.transform.position);
                if (distancia < 0.01f) continue;

                float cargaElectrica = scriptCarga.esPositiva ? scriptCarga.fuerza : -scriptCarga.fuerza;
                voltaje += k * cargaElectrica / distancia;
            }
        }

        // Contribución de líneas de carga (ajusta según tu implementación)
        foreach (var linea in managerCargas.LineasCarga)
        {
            LineaCarga scriptLinea = linea.GetComponent<LineaCarga>();
            voltaje += scriptLinea.densidadCarga * Mathf.Log(Vector3.Distance(posicionSensor, linea.transform.position));
        }

        return voltaje;
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            textoVoltaje.transform.rotation = Camera.main.transform.rotation;
        }
    }
}