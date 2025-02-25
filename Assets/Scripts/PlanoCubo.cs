using UnityEngine;

public class PlanoCubo : MonoBehaviour
{
    public bool esPositivo;
    public float fuerza;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    public void CambiarColor(Color nuevoColor)
    {
        if (rend != null)
            rend.material.color = nuevoColor;
    }

    public Vector3 CalcularFuerzaPlano(Vector3 posicionSensor)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return Vector3.zero;

        // Calcula el punto más cercano en el cubo al sensor
        Vector3 puntoCercano = col.ClosestPoint(posicionSensor);
        Vector3 direccion = posicionSensor - puntoCercano;
        float distancia = direccion.magnitude;

        if (distancia < 0.01f) return Vector3.zero;

        // Fórmula simplificada (similar a cargas puntuales)
        float magnitud = fuerza / distancia;
        if (!esPositivo) magnitud = -magnitud;

        return magnitud * direccion.normalized;
    }
}