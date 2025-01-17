using UnityEngine;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseFlecha;  // La parte inferior de la flecha
    public Transform puntaFlecha; // La parte superior de la flecha

    public float fuerzaMaxima = 10f; // La fuerza máxima que se puede aplicar
    private Vector3 direccionFuerza;

    void Start()
    {
        // Aseguramos que las referencias de la flecha estén asignadas si no se asignaron desde el prefabricado
        if (baseFlecha == null)
            baseFlecha = transform.GetChild(0);  // Suponiendo que la base es el primer hijo del prefab

        if (puntaFlecha == null)
            puntaFlecha = transform.GetChild(1); // Suponiendo que la punta es el segundo hijo del prefab
    }

    void Update()
    {
        // Aquí puedes manejar la entrada de la fuerza. Por ejemplo, aplicando la dirección del mouse:
        Vector3 fuerza = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - baseFlecha.position);
        fuerza.z = 0; // Para mantener la flecha en el plano 2D

        // Aplicar un límite a la fuerza
        fuerza = Vector3.ClampMagnitude(fuerza, fuerzaMaxima);
        direccionFuerza = fuerza.normalized;

        // Rotamos la flecha para que apunte en la dirección de la fuerza
        if (direccionFuerza.magnitude > 0.01f)
        {
            float angulo = Mathf.Atan2(direccionFuerza.y, direccionFuerza.x) * Mathf.Rad2Deg;
            baseFlecha.rotation = Quaternion.Euler(0, 0, angulo);
        }

        // Movemos la punta de la flecha según la magnitud de la fuerza
        puntaFlecha.position = baseFlecha.position + direccionFuerza * fuerza.magnitude;
    }
}
