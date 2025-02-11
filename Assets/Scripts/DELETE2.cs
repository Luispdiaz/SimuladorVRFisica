using UnityEngine;

public class PositionRelativeToObject : MonoBehaviour
{
    public Transform referenceObject; // El objeto de referencia (ejemplo: el cubo)
    public Vector3 offset = new Vector3(1, 0, 0); // Offset para posicionar la esfera

    private void OnEnable()
    {
        if (referenceObject != null)
        {
            // Posiciona la esfera al lado del objeto de referencia
            transform.position = referenceObject.position + offset;
            Debug.Log($"[PositionRelativeToObject] Esfera posicionada en {transform.position} con respecto a {referenceObject.name}");
        }
        else
        {
            Debug.LogWarning("[PositionRelativeToObject] No se asignó un objeto de referencia.");
        }
    }
}
