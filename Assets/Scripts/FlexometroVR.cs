using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Flexometro : MonoBehaviour
{
    public GameObject esferaInicioPrefab;
    public GameObject esferaFinPrefab;
    public GameObject miniEsferaPunteadaPrefab;

    [Range(0.05f, 1f)] public float densidadEsferas = 0.2f;
    public int decimales = 2;
    public float offsetTexto = 0.3f;

    private GameObject esferaInicio;
    private GameObject esferaFin;
    private TextMeshPro textoDistancia3D;
    private List<GameObject> esferasPunteadas = new List<GameObject>();

    /// <summary>
    /// Llamado por el Manager cuando "activas" el flexómetro
    /// </summary>
    public void IniciarMedicion()
    {
        // 1) Crear esferas
        Vector3 posBase = new Vector3(0, 1, 0);
        esferaInicio = Instantiate(esferaInicioPrefab, posBase, Quaternion.identity);
        esferaFin = Instantiate(esferaFinPrefab, posBase + Vector3.right, Quaternion.identity);

        // 2) Crear el objeto de texto (TextMeshPro) directamente
        GameObject textoGO = new GameObject("TextoDistancia3D");
        textoDistancia3D = textoGO.AddComponent<TextMeshPro>();

        // Configurar algunas propiedades iniciales
        textoDistancia3D.fontSize = 0.5f;             // Tamaño de fuente más pequeño
        textoDistancia3D.alignment = TextAlignmentOptions.Center;
        textoDistancia3D.color = Color.white;         // Texto blanco
        textoDistancia3D.text = "... m";
    }

    /// <summary>
    /// Llamado por el Manager cuando "desactivas" el flexómetro
    /// </summary>
    public void FinalizarMedicion()
    {
        if (esferaInicio) Destroy(esferaInicio);
        if (esferaFin) Destroy(esferaFin);
        if (textoDistancia3D) Destroy(textoDistancia3D.gameObject);
        LimpiarEsferasPunteadas();
    }

    /// <summary>
    /// Llamado cada frame por el Manager si el flexómetro está activo
    /// </summary>
    public void ActualizarMedicion()
    {
        if (!esferaInicio || !esferaFin || !textoDistancia3D) return;

        // 1) Calcular la distancia
        float distancia = Vector3.Distance(esferaInicio.transform.position, esferaFin.transform.position);
        textoDistancia3D.text = distancia.ToString($"F{decimales}") + " m";

        // 2) Posicionar el texto en el punto medio + offset vertical
        Vector3 puntoMedio = (esferaInicio.transform.position + esferaFin.transform.position) * 0.5f;
        textoDistancia3D.transform.position = puntoMedio + Vector3.up * offsetTexto;

        // 3) Hacer que el texto mire a la cámara
        Camera cam = Camera.main;
        if (cam != null)
        {
            textoDistancia3D.transform.LookAt(cam.transform);
            // Girar 180° en Y para que no se vea al revés
            textoDistancia3D.transform.Rotate(0, 180f, 0);
        }

        // 4) Generar la línea punteada
        GenerarEsferasPunteadas(esferaInicio.transform.position, esferaFin.transform.position, distancia);
    }

    private void GenerarEsferasPunteadas(Vector3 inicio, Vector3 fin, float distancia)
    {
        LimpiarEsferasPunteadas();

        // Ajusta el paso para acercar más las esferas (si así lo deseas).
        float pasoReal = densidadEsferas * 0.12f;
        int cantidad = Mathf.CeilToInt(distancia / pasoReal);

        for (int i = 0; i < cantidad; i++)
        {
            float t = i / (float)cantidad;
            Vector3 pos = Vector3.Lerp(inicio, fin, t);

            // 1) Instancia con rotación neutra (Quaternion.identity)
            GameObject miniEsfera = Instantiate(miniEsferaPunteadaPrefab, pos, Quaternion.identity);

            // 2) Fijar la rotación en el espacio mundial
            miniEsfera.transform.rotation = Quaternion.Euler(0, 0, 90);

            // 3) (Opcional) Asegurarte de que no queden parentadas a un objeto que rote
            miniEsfera.transform.SetParent(null, true);

            esferasPunteadas.Add(miniEsfera);
        }
    }



    private void LimpiarEsferasPunteadas()
    {
        foreach (var e in esferasPunteadas)
        {
            if (e) Destroy(e);
        }
        esferasPunteadas.Clear();
    }
}
