using TMPro;
using UnityEngine;

public class IndicadorFuerza : MonoBehaviour
{
    public Transform baseEsfera;
    public Transform cuerpo;
    public Transform punta;

    [Header("Par�metros de Fuerza")]
    public float factorEscala = 0.1f;

    // Distancias base (prefab)
    private float distanciaBaseCuerpo = 0.11f;
    private float distanciaCuerpoPunta = 0.09f;

    private Vector3 ultimaPosicion;
    private float umbralMovimiento = 0.001f;

    [Header("Texto de Fuerza (Opcional)")]
    [SerializeField] private TextMeshPro textoFuerza;

    // Enum para el tipo de magnitud
    public enum TipoMagnitud { LeyDeCoulomb, CampoElectrico }
    public TipoMagnitud tipoCampo;  // Cambi� el nombre de "tipoMagnitud" a "tipoCampo" para coincidir con tu c�digo

    private void Start()
    {
        // Intentar obtener el TextMeshPro si no se asign� en el Inspector
        if (textoFuerza == null)
        {
            textoFuerza = GetComponentInChildren<TextMeshPro>();
        }
        // Guardar la posici�n inicial
        ultimaPosicion = transform.position;
    }

    // M�todo para actualizar la direcci�n y la visualizaci�n de la fuerza
    public void ActualizarDireccion(Vector3 fuerza)
    {
        float magnitudFuerza = fuerza.magnitude;

        // Detectar si el objeto se est� moviendo
        if (Vector3.Distance(ultimaPosicion, transform.position) > umbralMovimiento)
        {
            // Si el objeto se mueve, desactivamos el cuerpo y la punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(false);
            if (punta != null) punta.gameObject.SetActive(false);
        }
        else
        {
            // Si no est� movi�ndose, habilitar cuerpo y punta si hay una fuerza significativa
            if (magnitudFuerza > 0.01f)
            {
                // Habilitar cuerpo y punta
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
                    punta.position = baseEsfera.position + (direccionNormalizada * (longitudDeseada - 0.08f));
                    punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
                }

                // Actualizar el texto de fuerza, si existe
                if (textoFuerza != null)
                {
                    // Dependiendo del tipo de magnitud, cambiar la unidad mostrada
                    if (tipoCampo == TipoMagnitud.LeyDeCoulomb)
                    {
                        textoFuerza.text = $"{magnitudFuerza:F2} F<sub>µ</sub>"; // Ley de Coulomb
                    }
                    else if (tipoCampo == TipoMagnitud.CampoElectrico)
                    {
                        // Asegúrate de tener habilitado Rich Text en tu TextMeshPro
                        textoFuerza.text = $"{magnitudFuerza:F2} E<sub>µ</sub>";
                        // Campo El�ctrico
                    }
                }
            }
            else
            {
                // Si la fuerza es muy peque�a o no hay cargas => deshabilitar cuerpo y punta
                if (cuerpo != null) cuerpo.gameObject.SetActive(false);
                if (punta != null) punta.gameObject.SetActive(false);

                // Texto en 0 o vac�o
                if (textoFuerza != null)
                {
                    textoFuerza.text = "0.00";
                }
            }
        }

        // Guardar la posici�n actual para detectar movimiento en el pr�ximo frame
        ultimaPosicion = transform.position;
    }

    public void CambiarColor(Color nuevoColor)
    {
        Renderer indicadorRenderer = GetComponent<Renderer>();
        if (indicadorRenderer != null)
        {
            indicadorRenderer.material.color = nuevoColor;
        }
    }


    private void LateUpdate()
    {
        // Asegurarse de que el texto siempre apunte hacia la c�mara
        if (textoFuerza != null && Camera.main != null)
        {
            textoFuerza.transform.rotation = Camera.main.transform.rotation;
        }
    }
}