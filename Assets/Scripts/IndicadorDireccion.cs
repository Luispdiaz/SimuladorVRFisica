using TMPro;
using UnityEngine;
using static IndicadorFuerza;

public class IndicadorDireccion : MonoBehaviour
{
    public Transform baseEsfera;
    public Transform cuerpo;
    public Transform punta;

    [Header("Texto (Opcional)")]
    [SerializeField] private TextMeshPro textoFuerza;

    private void Start()
    {
        if (textoFuerza == null)
            textoFuerza = GetComponentInChildren<TextMeshPro>();
    }

    public void ActualizarDireccion(Vector3 fuerza)
    {
        float magnitudFuerza = fuerza.magnitude;

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

            if (cuerpo != null)
            {
                Vector3 nuevaEscala = cuerpo.localScale;
                cuerpo.localScale = nuevaEscala;
                cuerpo.position = baseEsfera.position + (direccionNormalizada * nuevaEscala.y);
                cuerpo.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            if (punta != null)
            {

                punta.position = cuerpo.position + (direccionNormalizada * 0.13f);
                punta.rotation = rotacionIndicador * Quaternion.Euler(90, 0, 0);
            }

            // Actualizar el texto de fuerza, si existe

        }
        else
        {
            // Si la fuerza es muy pequeña o no hay cargas => deshabilitar cuerpo y punta
            if (cuerpo != null) cuerpo.gameObject.SetActive(false);
            if (punta != null) punta.gameObject.SetActive(false);

            // Texto en 0 o vacío
            if (textoFuerza != null)
            {
                textoFuerza.text = "0.00";
            }
        }
    }
    private void LateUpdate()
    {
        if (textoFuerza != null && Camera.main != null)
            textoFuerza.transform.rotation = Camera.main.transform.rotation;
    }
}