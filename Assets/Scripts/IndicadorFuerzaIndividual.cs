using UnityEngine;

public class IndicadorFuerzaIndividual : MonoBehaviour
{
    public Transform flecha; // Transform de la flecha que muestra la fuerza

    /// <summary>
    /// Actualiza la dirección y posición del indicador basado en la fuerza.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    /// <param name="posicionCarga">Posición de la carga</param>
    /// <param name="posicionSensor">Posición del sensor</param>
    public void ActualizarDireccion(Vector3 fuerza, Vector3 posicionCarga, Vector3 posicionSensor, bool esPositiva)
    {
        if (fuerza.magnitude > 0.01f) // Asegurarse de que haya una fuerza significativa
        {
            // Calcular la dirección desde el sensor hacia la carga (para positiva) o viceversa (para negativa)
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

            // Rotar el indicador completo para alinearlo con la dirección
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            flecha.rotation = rotacionIndicador;

            // Ajustar la posición de la flecha tomando como referencia la posición del sensor
            flecha.position = posicionSensor;
        }
    }
}
