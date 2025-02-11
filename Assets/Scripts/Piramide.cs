using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PiramideInteractable : XRSimpleInteractable
{
    [Header("Configuración de Flecha")]
    public GameObject arrowPrefab;
    public float arrowThickness = 0.5f;
    public float arrowLength = 1f;

    private GameObject currentArrow;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        ToggleFlecha();
    }

    private void ToggleFlecha()
    {
        if (currentArrow == null)
        {
            CreateArrow();
        }
        else
        {
            Destroy(currentArrow);
            currentArrow = null;
        }
    }

    private void CreateArrow()
    {
        Vector3 normal = transform.up;
        Vector3 tangent = Vector3.Cross(normal, transform.right).normalized;

        currentArrow = Instantiate(
            arrowPrefab,
            transform.position,
            Quaternion.LookRotation(tangent, normal) * Quaternion.Euler(0, 90, 0),
            transform
        );

        currentArrow.transform.localScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
    }
}