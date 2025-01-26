using UnityEngine;

public class IndicadorFuerzaIndividual : MonoBehaviour
{
    public Transform baseEsfera;  // Transform de la base
    public Transform cuerpo;      // Transform del cuerpo (cilindro)
    public Transform punta;       // Transform de la punta (pirámide)

    private float distanciaBaseCuerpo = 0.11f; // Distancia ajustada entre la base y el cuerpo
    private float distanciaCuerpoPunta = 0.09f; // Distancia entre el cuerpo y la punta

    /// <summary>
    /// Actualiza la dirección del indicador basado en la fuerza individual.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    /// <param name="posicionCarga">Posición de la carga</param>
    /// <param name="posicionSensor">Posición del sensor general</param>
    /// <param name="esPositiva">Indica si la carga es positiva</param>
    public void ActualizarDireccion(Vector3 fuerza, Vector3 posicionCarga, Vector3 posicionSensor, bool esPositiva)
    {
        if (fuerza.magnitude > 0.01f) // Asegurarse de que haya una fuerza significativa
        {
            // Calcular la dirección desde la carga hacia el sensor para positiva, o desde el sensor hacia la carga para negativa
            Vector3 direccion;
            if (esPositiva)
            {
                direccion = posicionCarga - posicionSensor; // Repulsiva
            }
            else
            {
                direccion = posicionSensor - posicionCarga; // Atractiva
            }
            Vector3 direccionNormalizada = direccion.normalized;

            // Mantener la base fija en su posición
            if (baseEsfera != null)
            {
                baseEsfera.position = posicionSensor;
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
