using UnityEngine;

public class CargaInteractable : MonoBehaviour
{
    public delegate void CargaSeleccionadaHandler(GameObject carga);
    public event CargaSeleccionadaHandler CargaSeleccionadaEvent;
    public event CargaSeleccionadaHandler CargaDesseleccionadaEvent; // Nuevo evento

    private void OnMouseDown()
    {
        CargaSeleccionadaEvent?.Invoke(gameObject);
    }

    private void OnMouseUp() // Detectar cuando se suelta la carga
    {
        CargaDesseleccionadaEvent?.Invoke(gameObject);
    }
}
