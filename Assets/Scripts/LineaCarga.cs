using UnityEngine;

public class LineaCarga : MonoBehaviour
{
    public bool esPositiva; // true = positiva, false = negativa
    public float densidadCarga = 1f; // Carga por unidad de longitud

    // Opcional: Color para visualización
    void Start()
    {
        GetComponent<Renderer>().material.color = esPositiva ? Color.red : Color.blue;
    }
}