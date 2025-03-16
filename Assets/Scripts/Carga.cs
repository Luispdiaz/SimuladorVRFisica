using UnityEngine;

public class Carga : MonoBehaviour
{
    public bool esPositiva;
    public float fuerza = 1f;
    private Rigidbody rb;
    private bool enMovimiento = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (enMovimiento && rb != null && !rb.isKinematic)
        {
            // Aplicar fuerza de todos los planos
            foreach (var plano in FindObjectsOfType<PlanoCubo>())
            {
                Vector3 fuerzaPlano = plano.CalcularFuerza(esPositiva);
                rb.AddForce(fuerzaPlano, ForceMode.Acceleration);
            }

            // Aplicar fuerza de todas las líneas infinitas
            foreach (var linea in FindObjectsOfType<LineaCarga>())
            {
                Vector3 fuerzaLinea = CalcularFuerzaLinea(linea);
                rb.AddForce(fuerzaLinea, ForceMode.Acceleration);
            }
        }
    }

    Vector3 CalcularFuerzaLinea(LineaCarga linea)
    {
        Collider colliderLinea = linea.GetComponent<Collider>();
        if (colliderLinea == null) return Vector3.zero;

        // 1. Obtener punto más cercano en la línea
        Vector3 puntoMasCercano = colliderLinea.ClosestPoint(transform.position);

        // 2. Calcular dirección y distancia
        Vector3 direccion = transform.position - puntoMasCercano;
        float distancia = direccion.magnitude;
        if (distancia < 0.01f) return Vector3.zero;

        // 3. Calcular magnitud (k=1 simplificado)
        float k = 1f;
        float cargaLinea = linea.densidadCarga * (linea.esPositiva ? 1f : -1f);
        float magnitud = (2 * k * cargaLinea * fuerza) / distancia;

        // 4. Aplicar dirección y polaridad
        return direccion.normalized * magnitud * (esPositiva ? 1f : -1f);
    }

    // Resto del código sin cambios...
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlanoFisico") && enMovimiento)
        {
            Time.timeScale = 0;
            enMovimiento = false;

            EstelaCarga estelaScript = GetComponent<EstelaCarga>();
            if (estelaScript != null) estelaScript.DetenerEstela();
        }
    }

    public void IniciarMovimiento()
    {
        enMovimiento = true;
        Time.timeScale = 1;
    }
}