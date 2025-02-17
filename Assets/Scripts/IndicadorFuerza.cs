using UnityEngine;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseEsfera;  // Transform de la base
    public Transform cuerpo;      // Transform del cuerpo (cilindro)
    public Transform punta;       // Transform de la punta (pirámide)

    // Factor para ajustar la longitud del indicador según la magnitud de la fuerza
    public float factorEscala = 0.1f;

    // Estos valores representan la “configuración base” del indicador (longitud total = 0.11 + 0.09 = 0.20)
    private float distanciaBaseCuerpo = 0.11f;  // Distancia base entre la base y el cuerpo
    private float distanciaCuerpoPunta = 0.09f; // Distancia base entre el cuerpo y la punta

    private void Start()
    {
        // (Se elimina la lógica de aplicación de color)
    }

    /// <summary>
    /// Actualiza la dirección del indicador basado en la fuerza neta.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    public void ActualizarDireccion(Vector3 fuerza)
    {
        if (fuerza.magnitude > 0.01f) // Asegurarse de que haya una fuerza significativa
        {
            Vector3 direccionNormalizada = fuerza.normalized;

            // Orientar el objeto completo para que apunte en la dirección de la fuerza
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            transform.rotation = rotacionIndicador;

            // Mantener la base fija en su posición
            if (baseEsfera != null)
            {
                baseEsfera.position = transform.position;
            }

            // Calcular la longitud deseada (proporcional a la magnitud de la fuerza)
            float longitudDeseada = fuerza.magnitude * factorEscala;

            // Ajustar la escala y posición del cuerpo
            if (cuerpo != null)
            {
                Vector3 nuevaEscala = cuerpo.localScale;
                nuevaEscala.y = (longitudDeseada * 0.5f);
                cuerpo.localScale = nuevaEscala;

                // Reposicionar el cuerpo: se mueve hacia arriba la mitad de su altura
                cuerpo.position = baseEsfera.position + (direccionNormalizada * nuevaEscala.y);

                // Rotación del cuerpo
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            // Posicionar la punta al final
            if (punta != null)
            {
                // Se coloca al final: longitudDeseada - 0.08f (ajusta según tu prefab)
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
            baseEsfera.position = transform.position;
        }

        if (cuerpo != null)
        {
            cuerpo.position = transform.position + (transform.up * distanciaBaseCuerpo);
            cuerpo.rotation = Quaternion.Euler(90, 0, 0);
        }

        if (punta != null)
        {
            punta.position = transform.position + (transform.up * (distanciaBaseCuerpo + distanciaCuerpoPunta));
            punta.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
