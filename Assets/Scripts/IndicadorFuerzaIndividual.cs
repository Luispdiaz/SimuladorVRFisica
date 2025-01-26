using UnityEngine;

public class IndicadorFuerzaIndividual : MonoBehaviour
{
    public Transform baseEsfera;
    public Transform cuerpo;
    public Transform punta;

    private float distanciaBaseCuerpo = 0.11f;
    private float distanciaCuerpoPunta = 0.09f;

    /// <summary>
    /// Actualiza la dirección del indicador basado en la fuerza individual.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    /// <param name="posicionCarga">Posición de la carga</param>
    /// <param name="posicionSensor">Posición del sensor individual</param>
    public void ActualizarDireccion(Vector3 fuerza, Vector3 posicionCarga, Vector3 posicionSensor, bool esPositiva)
    {
        if (fuerza.magnitude > 0.01f)
        {
            // Determinar la dirección en función de si la carga es positiva o negativa
            Vector3 direccion = esPositiva ? (posicionSensor - posicionCarga) : (posicionCarga - posicionSensor);
            Vector3 direccionNormalizada = direccion.normalized;

            // Posicionar la base del indicador en la posición del sensor
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
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            // Ajustar la posición y rotación de la punta
            if (punta != null)
            {
                punta.position = cuerpo.position + (direccionNormalizada * distanciaCuerpoPunta);
                punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }
        }
        else
        {
            ResetIndicator();
        }
    }

    private void ResetIndicator()
    {
        if (baseEsfera != null)
        {
            baseEsfera.localPosition = Vector3.zero;
        }

        if (cuerpo != null)
        {
            cuerpo.localPosition = new Vector3(0, distanciaBaseCuerpo, 0);
            cuerpo.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
        }

        if (punta != null)
        {
            punta.localPosition = new Vector3(0, distanciaBaseCuerpo + distanciaCuerpoPunta, 0);
            punta.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
        }
    }
}
