using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CambiarImagenTMP : MonoBehaviour
{
    public Sprite nuevaImagen; // Asigna tu PNG en el Inspector
    private Image imagenBoton;

    void Start()
    {
        imagenBoton = GetComponent<Image>();
    }

    public void CambiarImagen()
    {
        imagenBoton.sprite = nuevaImagen;
    }
}