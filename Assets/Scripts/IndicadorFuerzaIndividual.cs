using UnityEngine;

public class IndicadorFuerzaIndividual : MonoBehaviour
{
    public Transform flecha; // Transform de la flecha que muestra la fuerza

    /// <summary>
    /// Actualiza la dirección y posición del indicador basado en la fuerza.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    /// <param name="posicionCarga">Posición de la carga</param>
    public void ActualizarDireccion(Vector3 fuerza, Vector3 posicionCarga, Vector3 posicionSensor)
    {
        if (fuerza.magnitude > 0.01f) // Asegurarse de que haya una fuerza significativa
        {
            // Normalizar la dirección de la fuerza
            Vector3 direccionNormalizada = fuerza.normalized;

            // Rotar el indicador completo para alinearlo con la dirección de la fuerza
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            flecha.rotation = rotacionIndicador;

            // Ajustar la posición de la flecha tomando como referencia la posición del sensor
            flecha.position = posicionCarga + (direccionNormalizada * 0.1f); // Posición relativa a la carga
        }
    }
}
