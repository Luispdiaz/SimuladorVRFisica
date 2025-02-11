using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BotonInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable
{
    [Header("Configuraci�n de Flecha")]
    public GameObject arrowPrefab; // Prefab de la flecha
    public float arrowThickness = 0.5f; // Grosor de la flecha
    public float arrowLength = 1f; // Longitud de la flecha

    private GameObject currentArrow; // Referencia a la flecha instanciada

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        ToggleFlecha(); // Mostrar u ocultar la flecha
    }

    private void ToggleFlecha()
    {
        if (currentArrow == null)
        {
            CreateArrow(); // Crear la flecha si no existe
        }
        else
        {
            Destroy(currentArrow); // Destruir la flecha si ya existe
            currentArrow = null;
        }
    }

    private void CreateArrow()
    {
        // Calcular la direcci�n de la flecha (hacia arriba desde la base de la pir�mide)
        Vector3 normal = transform.up; // Normal de la pir�mide
        Vector3 tangent = Vector3.Cross(normal, transform.right).normalized; // Tangente

        // Instanciar la flecha
        currentArrow = Instantiate(
            arrowPrefab,
            transform.position,
            Quaternion.LookRotation(tangent, normal) * Quaternion.Euler(0, 90, 0),
            transform.parent // La flecha ser� hija de la pir�mide
        );

        // Ajustar la escala de la flecha
        currentArrow.transform.localScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
    }
}