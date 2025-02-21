using UnityEngine;
using TMPro; // O "using UnityEngine.UI;" si usas Text de UI

public class CargaTexto : MonoBehaviour
{
    [Header("Referencia al texto")]
    [SerializeField] private TextMeshPro textoFuerza;

    private void Start()
    {
        if (textoFuerza == null)
        {
            textoFuerza = GetComponentInChildren<TextMeshPro>();
        }
    }

    /// <summary>
    /// Método para que el Manager (o quien sea) actualice el texto con la fuerza de la carga.
    /// Si la carga es negativa (esPositiva == false), muestra un signo - delante.
    /// </summary>
    public void ActualizarTextoFuerza(bool esPositiva, float fuerza)
    {
        if (textoFuerza == null) return;

        // Si la carga es positiva, mostramos el valor tal cual.
        // Si es negativa, anteponemos el signo - al valor.
        if (esPositiva)
        {
            textoFuerza.text = $"{fuerza:F2} µC";
        }
        else
        {
            textoFuerza.text = $"-{fuerza:F2} µC";
        }
    }

    private void LateUpdate()
    {
        if (Camera.main != null && textoFuerza != null)
        {
            textoFuerza.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
