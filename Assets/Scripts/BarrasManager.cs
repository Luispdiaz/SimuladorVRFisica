using UnityEngine;
using UnityEngine.UI;

public class BarrasCargadasManager : MonoBehaviour
{
    [Header("Elementos de UI")]
    public GameObject subMenuBarras;
    public GameObject tituloSubMenu;
    public Button insertarBarrasPositivasBtn;  // Botón para insertar barras positivas
    public Button insertarBarrasNegativasBtn;  // Botón para insertar barras negativas
    public Button insertarBarrasAlternadasBtn; // Botón para insertar barras alternadas
    public Button closeButton;
    public Material materialAzulTransparente;

    [Header("Configuración de Spawn")]
    public Vector3 spawnPosition;
    public float separacionVertical = 1.5f;
    public float largoBarra = 3.0f;
    public float profundidadCampo = 1.0f;

    [Header("Prefab")]
    public GameObject barraPositivaPrefab;
    public GameObject barraNegativaPrefab;

    [Header("Visualización del Campo Eléctrico Uniforme")]
    [Tooltip("Color del campo eléctrico con transparencia.")]
    public Color colorCampo = new Color(1f, 1f, 0f, 0.5f); // Amarillo semitransparente

    private GameObject campoUniforme;

    private void Start()
    {
        if (subMenuBarras != null)
            subMenuBarras.SetActive(false);
        if (tituloSubMenu != null)
            tituloSubMenu.SetActive(false);

        if (insertarBarrasPositivasBtn != null)
            insertarBarrasPositivasBtn.onClick.AddListener(InsertarBarrasPositivas);  // Asignamos función para barras positivas

        if (insertarBarrasNegativasBtn != null)
            insertarBarrasNegativasBtn.onClick.AddListener(InsertarBarrasNegativas);  // Asignamos función para barras negativas

        if (insertarBarrasAlternadasBtn != null)
            insertarBarrasAlternadasBtn.onClick.AddListener(InsertarBarrasAlternadas);  // Asignamos función para barras alternadas

        if (closeButton != null)
            closeButton.onClick.AddListener(CerrarSubMenu);
    }

    public void MostrarSubMenuBarras()
    {
        if (subMenuBarras != null)
            subMenuBarras.SetActive(true);
        if (tituloSubMenu != null)
            tituloSubMenu.SetActive(true);
    }

    public void CerrarSubMenu()
    {
        if (subMenuBarras != null)
            subMenuBarras.SetActive(false);
        if (tituloSubMenu != null)
            tituloSubMenu.SetActive(false);
    }

    // Insertar ambas barras positivas
    private void InsertarBarrasPositivas()
    {
        if (barraPositivaPrefab == null)
        {
            Debug.LogError("Falta asignar el prefab de la barra positiva.");
            return;
        }

        // Instanciar ambas barras positivas
        Vector3 posicionBarraInferior = spawnPosition;
        Instantiate(barraPositivaPrefab, posicionBarraInferior, Quaternion.identity);

        Vector3 posicionBarraSuperior = spawnPosition + Vector3.up * separacionVertical;
        Instantiate(barraPositivaPrefab, posicionBarraSuperior, Quaternion.identity);

    }

    // Insertar ambas barras negativas
    private void InsertarBarrasNegativas()
    {
        if (barraNegativaPrefab == null)
        {
            Debug.LogError("Falta asignar el prefab de la barra negativa.");
            return;
        }

        // Instanciar ambas barras negativas
        Vector3 posicionBarraInferior = spawnPosition;
        Instantiate(barraNegativaPrefab, posicionBarraInferior, Quaternion.identity);

        Vector3 posicionBarraSuperior = spawnPosition + Vector3.up * separacionVertical;
        Instantiate(barraNegativaPrefab, posicionBarraSuperior, Quaternion.identity);

    }

    // Insertar barras alternadas
    private void InsertarBarrasAlternadas()
    {
        if (barraPositivaPrefab == null || barraNegativaPrefab == null)
        {
            Debug.LogError("Faltan asignar los prefabs de las barras positiva y negativa.");
            return;
        }

        // Instanciar barra positiva
        Vector3 posicionBarraInferiorPositiva = spawnPosition;
        Instantiate(barraPositivaPrefab, posicionBarraInferiorPositiva, Quaternion.identity);

        // Instanciar barra negativa
        Vector3 posicionBarraInferiorNegativa = spawnPosition + Vector3.up * separacionVertical;
        Instantiate(barraNegativaPrefab, posicionBarraInferiorNegativa, Quaternion.identity);

    }

    
}
