using UnityEngine;
using TMPro; // O "using UnityEngine.UI;" si usas Text de UI en lugar de TextMeshPro

public class SensorVoltaje : MonoBehaviour
{
    [Header("Referencia al texto")]
    [SerializeField] private TextMeshPro textoVoltaje;
    // Si usas Text de UI, cambia a "Text textoVoltaje;"

    private void Start()
    {
        // Intentar obtener el texto si no se asign� en el Inspector
        if (textoVoltaje == null)
        {
            textoVoltaje = GetComponentInChildren<TextMeshPro>();
        }
    }

    /// <summary>
    /// M�todo para que el manager actualice el texto con el voltaje calculado.
    /// </summary>
    public void ActualizarTextoVoltaje(float voltaje)
    {
        if (textoVoltaje != null)
        {
            textoVoltaje.text = $"{voltaje:F2} V";
        }
    }

    private void LateUpdate()
    {
        // Orientar el texto hacia la c�mara principal (si existe)
        if (Camera.main != null && textoVoltaje != null)
        {
            textoVoltaje.transform.rotation = Camera.main.transform.rotation;
        }
    }
}