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
        }
    }

    // Detener simulación al colisionar con un plano
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

    // Método para activar el movimiento desde BarrasManager
    public void IniciarMovimiento()
    {
        enMovimiento = true;
        Time.timeScale = 1; // Asegurar que el tiempo esté activo
    }
}