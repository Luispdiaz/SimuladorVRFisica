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
    /// <param name="posicionSensor">Posición del sensor general</param>
    public void ActualizarDireccion(Vector3 fuerza, Vector3 posicionCarga, Vector3 posicionSensor, bool esPositiva)
    {
        if (fuerza.magnitude > 0.01f)
        {
            Vector3 direccion = esPositiva ? (posicionCarga - posicionSensor) : (posicionSensor - posicionCarga);
            Vector3 direccionNormalizada = direccion.normalized;

            if (baseEsfera != null)
            {
                baseEsfera.position = posicionSensor;
            }

            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            transform.rotation = rotacionIndicador;

            if (cuerpo != null)
            {
                cuerpo.position = baseEsfera.position + (direccionNormalizada * distanciaBaseCuerpo);
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

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
