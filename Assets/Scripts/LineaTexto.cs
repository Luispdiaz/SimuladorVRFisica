using UnityEngine;
using TMPro; // Usamos TextMeshPro para el texto

public class LineaTexto : MonoBehaviour
{
    [Header("Referencia al texto")]
    [SerializeField] private TextMeshPro textoDensidadCarga; // El texto que mostrará la densidad de carga de la línea

    private void Start()
    {
        // Si el texto no se asigna en el Inspector, intenta buscarlo automáticamente en los hijos
        if (textoDensidadCarga == null)
        {
            textoDensidadCarga = GetComponentInChildren<TextMeshPro>();
        }
    }

    /// <summary>
    /// Método para actualizar el texto en base a la densidad de carga de la línea.
    /// Si la carga es negativa, añade el signo "-" al valor.
    /// </summary>
    public void ActualizarTextoLinea(float densidadCarga, bool esPositiva)
    {
        if (textoDensidadCarga != null)
        {
            // Si es negativa, anteponemos el signo "-" al valor
            if (!esPositiva)
            {
                textoDensidadCarga.text = $"-{densidadCarga:F2} µC/m";
            }
            else
            {
                textoDensidadCarga.text = $"{densidadCarga:F2} µC/m";
            }
        }
    }

    // Eliminado LateUpdate para orientación hacia la cámara, ya que no es necesario
}
