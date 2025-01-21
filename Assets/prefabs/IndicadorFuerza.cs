using UnityEngine;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseEsfera;  // Transform de la base
    public Transform cuerpo;      // Transform del cuerpo (cilindro)
    public Transform punta;       // Transform de la punta (pirámide)

    private float distanciaBaseCuerpo = 0.5f; // Distancia ajustada entre la base y el cuerpo
    private float distanciaCuerpoPunta = 0.7f; // Distancia entre el cuerpo y la punta

    /// <summary>
    /// Actualiza la dirección del indicador basado en la fuerza neta.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    public void ActualizarDireccion(Vector3 fuerza)
    {
        if (fuerza.magnitude > 0.01f) // Asegurarse de que haya una fuerza significativa
        {
            // Normalizar la dirección de la fuerza
            Vector3 direccionNormalizada = fuerza.normalized;

            // Mantener la base fija en su posición
            if (baseEsfera != null)
            {
                baseEsfera.position = transform.position;
            }

            // Rotar el indicador completo para alinearlo con la dirección de la fuerza
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            transform.rotation = rotacionIndicador;

            // Ajustar la posición del cuerpo entre la base y la punta
            if (cuerpo != null)
            {
                cuerpo.position = baseEsfera.position + (direccionNormalizada * distanciaBaseCuerpo);
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0); // Corregir orientación
            }

            // Ajustar la posición y rotación de la punta
            if (punta != null)
            {
                punta.position = cuerpo.position + (direccionNormalizada * distanciaCuerpoPunta);
                punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0); // Corregir orientación
            }
        }
        else
        {
            // Si no hay fuerza significativa, mantener el indicador en su estado inicial
            ResetIndicator();
        }
    }

    /// <summary>
    /// Restaura el estado inicial del indicador.
    /// </summary>
    private void ResetIndicator()
    {
        if (baseEsfera != null)
        {
            baseEsfera.localPosition = Vector3.zero;
        }

        if (cuerpo != null)
        {
            cuerpo.localPosition = new Vector3(0, distanciaBaseCuerpo, 0);
            cuerpo.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0); // Corregir rotación inicial
        }

        if (punta != null)
        {
            punta.localPosition = new Vector3(0, distanciaBaseCuerpo + distanciaCuerpoPunta, 0);
            punta.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0); // Corregir rotación inicial
        }
    }
}
