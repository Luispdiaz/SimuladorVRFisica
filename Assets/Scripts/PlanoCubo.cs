using UnityEngine;

public class PlanoCubo : MonoBehaviour
{
    public bool esPositivo;
    public float fuerza;
    private Renderer rend;
    public bool invertirDireccion;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    public void CambiarColor(Color nuevoColor)
    {
        if (rend != null)
            rend.material.color = nuevoColor;
    }
    public Vector3 CalcularFuerza(bool esCargaPositiva)
    {
        // Usamos transform.forward como base (como en el código original)
        Vector3 direccion = esPositivo ? transform.forward : -transform.forward;
        // Rotamos la dirección 90° sobre el eje X (esto convierte una dirección horizontal en vertical)
        Vector3 direccionRotada = Quaternion.Euler(90, 0, 0) * direccion;
        // Aplicar inversión si es necesario
        if (invertirDireccion) direccionRotada *= -1;
        float signo = (esPositivo == esCargaPositiva) ? -1f : 1f; // Repulsión o atracción
        return direccionRotada * fuerza * signo;
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
        float magnitud = 2 * Mathf.PI * fuerza;
        if (!esPositivo) magnitud = -magnitud;

        return magnitud * direccion.normalized;
    }
}