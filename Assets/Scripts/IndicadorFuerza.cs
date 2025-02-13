using UnityEngine;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseEsfera;  // Transform de la base
    public Transform cuerpo;      // Transform del cuerpo (cilindro)
    public Transform punta;       // Transform de la punta (pirámide)

    // Factor para ajustar la longitud del indicador según la magnitud de la fuerza
    public float factorEscala = 0.1f;

    // Estos valores representan la “configuración base” del indicador (longitud total = 0.11 + 0.09 = 0.20)
    // Si en el prefab se configuraron otras distancias, puedes usarlas o modificarlas.
    private float distanciaBaseCuerpo = 0.11f; // Distancia base entre la base y el cuerpo
    private float distanciaCuerpoPunta = 0.09f; // Distancia base entre el cuerpo y la punta

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

            // Orientar el objeto completo para que apunte en la dirección de la fuerza
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            transform.rotation = rotacionIndicador;

            // Mantener la base fija en su posición
            if (baseEsfera != null)
            {
                baseEsfera.position = transform.position;
            }

            // Calcular la longitud deseada del indicador
            // (esta longitud será proporcional a la magnitud de la fuerza)
            float longitudDeseada = fuerza.magnitude * factorEscala;

            // Rotar el indicador completo para alinearlo con la dirección de la fuerza
            //Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            //transform.rotation = rotacionIndicador;

            // Para escalar de forma similar a la flecha de la suma de cargas,
            // usaremos el método de que el cuerpo (cilindro) se escala en Y
            // de forma que su altura efectiva sea la mitad de la longitud total.
            if (cuerpo != null)
            {
                // Ajustar la escala del cuerpo
                Vector3 nuevaEscala = cuerpo.localScale;
                nuevaEscala.y = (longitudDeseada * 0.5f);
                cuerpo.localScale = nuevaEscala;

                // Reposicionar el cuerpo de forma que “comience” en la base.
                // Suponiendo que el pivote está en el centro, se mueve hacia arriba la mitad de su altura.
                cuerpo.position = baseEsfera.position + (direccionNormalizada * nuevaEscala.y);

                // Ajustar la rotación del cuerpo (corrigiendo la orientación, como en la flecha de cargas)
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }


            // Posicionar la punta al final de la flecha
            if (punta != null)
            {
                // La punta se coloca al final: desde la base se recorre toda la longitud
                punta.position = baseEsfera.position + (direccionNormalizada * (longitudDeseada - 0.08f));
                punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
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
            // Colocar la base en la posición del objeto padre
            baseEsfera.position = transform.position;
        }

        if (cuerpo != null)
        {
            // Usar las distancias base (sin escalado) para reposicionar el cuerpo
            cuerpo.position = transform.position + (transform.up * distanciaBaseCuerpo);
            // Restaurar la rotación base (puedes ajustar si tu prefab requiere otra orientación)
            cuerpo.rotation = Quaternion.Euler(90, 0, 0);
        }

        if (punta != null)
        {
            // Posicionar la punta en el extremo “original” del indicador
            punta.position = transform.position + (transform.up * (distanciaBaseCuerpo + distanciaCuerpoPunta));
            punta.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}