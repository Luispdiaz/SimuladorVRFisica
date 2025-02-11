using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRBuildingBlock : MonoBehaviour
{
    public GameObject arrow; // Asigna la flecha en el Inspector
    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable == null)
        {
            Debug.LogError($"[VRBuildingBlock] No hay XRBaseInteractable en {gameObject.name}");
        }

        if (arrow != null)
        {
            arrow.SetActive(false);
            Debug.Log($"[VRBuildingBlock] Flecha oculta en: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[VRBuildingBlock] No se asignó una flecha en: {gameObject.name}");
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelectEnter);
            interactable.selectExited.RemoveListener(OnSelectExit);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log($"[VRBuildingBlock] Ray apuntando a: {gameObject.name}");
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log($"[VRBuildingBlock] Ray salió de: {gameObject.name}");
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (arrow != null)
        {
            arrow.SetActive(true);
            Debug.Log($"[VRBuildingBlock] SELECCIONADO: {gameObject.name} - Flecha ACTIVADA ?");
        }
        else
        {
            Debug.LogWarning($"[VRBuildingBlock] SELECCIONADO pero sin flecha en: {gameObject.name}");
        }
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        if (arrow != null)
        {
            arrow.SetActive(false);
            Debug.Log($"[VRBuildingBlock] DESELECCIONADO: {gameObject.name} - Flecha DESACTIVADA ?");
        }
    }
}
