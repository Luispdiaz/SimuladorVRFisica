using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CrearPentagono : MonoBehaviour
{
    void Start()
    {
        var texto = GetComponent<TextMeshProUGUI>();
        texto.text = "<sprite=0>"; // Usa un sprite en el texto
    }
}