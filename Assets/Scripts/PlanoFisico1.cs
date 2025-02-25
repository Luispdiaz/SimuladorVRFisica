using UnityEngine;

public class PlanoFisico1 : MonoBehaviour
{
    public float fuerza = 1f;        // Se mantiene siempre positiva en el Inspector
    public bool esPositivo = true;   // Se decide en el Inspector si es positivo o no

    void Start()
    {
        // Puedes dejar vacío o eliminar
    }

    public Vector3 CalcularFuerzaPlano(Vector3 posicionSensor)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return Vector3.zero;

        // 1) Punto más cercano
        Vector3 puntoMasCercano = col.ClosestPoint(posicionSensor);

        // 2) Vector sensor - punto
        Vector3 direccion = posicionSensor - puntoMasCercano;
        float distancia = direccion.magnitude;
        if (distancia < 0.01f) return Vector3.zero;

        // 3) Magnitud (ej: 1/r)
        float magnitud = fuerza / distancia;

        // 4) Si es negativo => invierte la dirección
        if (!esPositivo)
        {
            magnitud = -magnitud;
        }

        // 5) Retornar el vector
        return magnitud * direccion.normalized;
    }

}
