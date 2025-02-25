using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuControl : MonoBehaviour
{
    [Header("Plano Configuration")]
    public Slider planoSlider;
    public GameObject planoPrefab;
    public float offsetPosicion = 15f;

    [Header("Botones Planos")]
    public List<Button> botonesPositivos = new List<Button>();
    public List<Button> botonesNegativos = new List<Button>();

    [Header("Carga Configuration")]
    public Slider horizontalSlider;
    public Slider verticalSlider;
    public Slider velocidadSlider;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Referencias Explícitas de Botones")]
    public Button positivoArribaBtn;
    public Button positivoAbajoBtn;
    public Button positivoIzquierdaBtn;
    public Button positivoDerechaBtn;
    public Button negativoArribaBtn;
    public Button negativoAbajoBtn;
    public Button negativoIzquierdaBtn;
    public Button negativoDerechaBtn;

    private readonly string[] direcciones = { "Arriba", "Abajo", "Izquierda", "Derecha" };

    private void Start()
    {
        ConfigurarSliders();
        ConfigurarBotonesPlanosExplicitamente();
    }
    void ConfigurarBotonesPlanosExplicitamente()
    {
        // Botones positivos
        positivoArribaBtn.onClick.AddListener(() => CrearPlano("Arriba", true));
        positivoAbajoBtn.onClick.AddListener(() => CrearPlano("Abajo", true));
        positivoIzquierdaBtn.onClick.AddListener(() => CrearPlano("Izquierda", true));
        positivoDerechaBtn.onClick.AddListener(() => CrearPlano("Derecha", true));

        // Botones negativos
        negativoArribaBtn.onClick.AddListener(() => CrearPlano("Arriba", false));
        negativoAbajoBtn.onClick.AddListener(() => CrearPlano("Abajo", false));
        negativoIzquierdaBtn.onClick.AddListener(() => CrearPlano("Izquierda", false));
        negativoDerechaBtn.onClick.AddListener(() => CrearPlano("Derecha", false));
    }

    void ConfigurarSliders()
    {
        planoSlider.minValue = 0.1f;
        planoSlider.maxValue = 20f;
        horizontalSlider.minValue = -90f;
        horizontalSlider.maxValue = 90f;
        verticalSlider.minValue = -90f;
        verticalSlider.maxValue = 90f;
        velocidadSlider.minValue = 0f;
        velocidadSlider.maxValue = 100f;
    }

    void ConfigurarBotonesPlanos()
    {
        // Configurar botones positivos
        for (int i = 0; i < botonesPositivos.Count; i++)
        {
            string direccion = direcciones[i];
            botonesPositivos[i].onClick.AddListener(() => CrearPlano(direccion, true));
        }

        // Configurar botones negativos
        for (int i = 0; i < botonesNegativos.Count; i++)
        {
            string direccion = direcciones[i];
            botonesNegativos[i].onClick.AddListener(() => CrearPlano(direccion, false));
        }
    }

    public void CrearPlano(string direccion, bool esPositivo)
    {
        if (planoPrefab == null) return;

        Vector3 posicion = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Vector3 rotacion = Vector3.zero;

        switch (direccion)
        {
            case "Arriba":
                posicion = new Vector3(0, esPositivo ? offsetPosicion : -offsetPosicion, 0);
                rotacion = esPositivo ? new Vector3(0, 0, 0) : new Vector3(180, 0, 0);
                break;

            case "Abajo":
                posicion = new Vector3(0, esPositivo ? -offsetPosicion : offsetPosicion, 0);
                rotacion = esPositivo ? new Vector3(180, 0, 0) : new Vector3(0, 0, 0);
                break;

            case "Izquierda":
                posicion = new Vector3(esPositivo ? -offsetPosicion : offsetPosicion, 0, 0);
                rotacion = esPositivo ? new Vector3(0, 0, 90) : new Vector3(0, 0, 270);
                break;

            case "Derecha":
                posicion = new Vector3(esPositivo ? offsetPosicion : -offsetPosicion, 0, 0);
                rotacion = esPositivo ? new Vector3(0, 0, 270) : new Vector3(0, 0, 90);
                break;
        }

        GameObject nuevoPlano = Instantiate(planoPrefab, posicion, Quaternion.Euler(rotacion));
        ConfigurarPlano(nuevoPlano, esPositivo);
    }

    void ConfigurarPlano(GameObject plano, bool esPositivo)
    {
        PlanoFisico fisica = plano.GetComponent<PlanoFisico>();
        if (fisica != null)
        {
            float valor = esPositivo ? planoSlider.value : -planoSlider.value;
            fisica.ConfigurarFuerza(valor);
            fisica.CambiarColor(esPositivo ? Color.red : Color.blue);
        }
    }

    // Métodos para la Carga (mantenemos la estructura básica)
    public void LanzarCarga()
    {
        Vector3 direccion = CalcularDireccion();
        float velocidad = velocidadSlider.value;
        // Lógica de lanzamiento aquí
    }

    Vector3 CalcularDireccion()
    {
        return Quaternion.Euler(verticalSlider.value, horizontalSlider.value, 0) * Vector3.forward;
    }
}