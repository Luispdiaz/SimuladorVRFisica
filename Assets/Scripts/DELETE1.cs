using UnityEngine;

public class OVRRaycastLogger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[OVRRaycastLogger] El rayo está sobre: {gameObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[OVRRaycastLogger] El rayo salió de: {gameObject.name}");
    }
}
