using UnityEngine;
using TMPro;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseEsfera;
    public Transform cuerpo;
    public Transform punta;

    [Header("Parámetros de Fuerza")]
    public float factorEscala = 0.1f;

    // Distancias base (prefab)
    private float distanciaBaseCuerpo = 0.11f;
    private float distanciaCuerpoPunta = 0.09f;

    private Vector3 ultimaPosicion;
    private float umbralMovimiento = 0.001f;

    [Header("Texto de Fuerza (Opcional)")]
    [SerializeField] private TextMeshPro textoFuerza;

    private void Start()
    {
        // Intentar obtener el TextMeshPro si no se asignó en el Inspector
        if (textoFuerza == null)
        {
            textoFuerza = GetComponentInChildren<TextMeshPro>();
        }
        // NUEVO: Guardar la posición inicial
        ultimaPosicion = transform.position;
    }

    /// <summary>
    /// Actualiza la dirección del indicador basado en la fuerza neta.
    /// </summary>
    /// <param name="fuerza">Vector de fuerza que determina la dirección</param>
    public void ActualizarDireccion(Vector3 fuerza)
    {
        float magnitudFuerza = fuerza.magnitude;

        if (magnitudFuerza > 0.01f)
        {
            // Se considera que hay fuerza => habilitar cuerpo y punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(true);
            if (punta != null) punta.gameObject.SetActive(true);

            Vector3 direccionNormalizada = fuerza.normalized;
            Quaternion rotacionIndicador = Quaternion.LookRotation(direccionNormalizada, Vector3.up);
            transform.rotation = rotacionIndicador;

            if (baseEsfera != null)
            {
                baseEsfera.position = transform.position;
            }

            float longitudDeseada = magnitudFuerza * factorEscala;

            if (cuerpo != null)
            {
                Vector3 nuevaEscala = cuerpo.localScale;
                nuevaEscala.y = (longitudDeseada * 0.5f);
                cuerpo.localScale = nuevaEscala;
                cuerpo.position = baseEsfera.position + (direccionNormalizada * nuevaEscala.y);
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            if (punta != null)
            {
                punta.position = baseEsfera.position +
                                 (direccionNormalizada * (longitudDeseada - 0.08f));
                punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            // Actualizar el texto de fuerza, si existe
            if (textoFuerza != null)
            {
                textoFuerza.text = $"{magnitudFuerza:F2}";
            }
        }
        else
        {
            // Si la fuerza es muy pequeña o no hay cargas => deshabilitar cuerpo y punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(false);
            if (punta != null) punta.gameObject.SetActive(false);

            // Opcional: puedes reposicionar la base en ResetIndicator(), o dejarlo
            if (baseEsfera != null)
            {
                baseEsfera.position = transform.position;
            }

            // Texto en 0 o vacío
            if (textoFuerza != null)
            {
                textoFuerza.text = "0.00";
            }
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
            punta.position = transform.position +
                             (transform.up * (distanciaBaseCuerpo + distanciaCuerpoPunta));
            punta.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void LateUpdate()
    {
        // Mantener texto mirando a la cámara (ya existente)
        if (textoFuerza != null && Camera.main != null)
        {
            textoFuerza.transform.rotation = Camera.main.transform.rotation;
        }

        // NUEVO: Detectar si el sensor se ha movido
        float distancia = Vector3.Distance(transform.position, ultimaPosicion);

        if (distancia > umbralMovimiento)
        {
            // Está en movimiento => desactivar cuerpo y punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(false);
            if (punta != null) punta.gameObject.SetActive(false);
        }
        else
        {
            // Está estático => activar cuerpo y punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(true);
            if (punta != null) punta.gameObject.SetActive(true);
        }

        // Actualizar la ultimaPosicion para el siguiente frame
        ultimaPosicion = transform.position;
    }
}
