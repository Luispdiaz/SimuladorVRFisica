using UnityEngine;

public class PlanoFisico : MonoBehaviour
{
    [SerializeField] private float fuerza;
    [SerializeField] private Renderer rend;
    [SerializeField] private float radioAccion = 20f;

    private void Start()
    {
        // Auto-referencia si no está asignado
        if (rend == null) rend = GetComponent<Renderer>();
    }

    public void ConfigurarFuerza(float nuevaFuerza) => fuerza = nuevaFuerza;

    public void CambiarColor(Color nuevoColor)
    {
        if (rend != null)
        {
            rend.material.color = nuevoColor;
        }
    }

    private void FixedUpdate()
    {
        AplicarFuerza();
    }

    void AplicarFuerza()
    {
        Collider[] objetos = Physics.OverlapBox(
            transform.position,
            new Vector3(radioAccion, 0.5f, radioAccion)
        );

        foreach (var col in objetos)
        {
            if (col.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direccion = fuerza > 0 ?
                    (col.transform.position - transform.position).normalized :
                    (transform.position - col.transform.position).normalized;

                rb.AddForce(direccion * Mathf.Abs(fuerza), ForceMode.Acceleration);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = fuerza > 0 ? Color.red : Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(radioAccion * 2, 1f, radioAccion * 2));
    }
}