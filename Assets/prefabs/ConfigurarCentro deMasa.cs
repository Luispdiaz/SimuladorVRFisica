using UnityEngine;

public class ConfigurarCentroDeMasa : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        // Obtén el componente Rigidbody del objeto
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("El objeto no tiene un Rigidbody. Por favor, agrega uno.");
            return;
        }

        // Calcula la posición más alta en Y
        Vector3 puntoMasAlto = ObtenerPuntoMasAlto();

        // Ajusta el centro de masa del Rigidbody
        rb.centerOfMass = transform.InverseTransformPoint(puntoMasAlto);

        Debug.Log($"Centro de masa configurado en: {rb.centerOfMass}");
    }

    private Vector3 ObtenerPuntoMasAlto()
    {
        // Obtén todos los hijos del objeto
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Vector3 puntoMasAlto = transform.position; // Punto inicial (posición del objeto raíz)
        float maxY = float.MinValue;

        foreach (Renderer renderer in renderers)
        {
            // Obtén el límite superior del renderizador
            Vector3 maxPoint = renderer.bounds.max;

            if (maxPoint.y > maxY)
            {
                maxY = maxPoint.y;
                puntoMasAlto = new Vector3(maxPoint.x, maxY, maxPoint.z);
            }
        }

        return puntoMasAlto;
    }
}