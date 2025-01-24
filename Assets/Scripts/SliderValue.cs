using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider slider; // Referencia al Slider
    public Text sliderValueText; // Referencia al Text

    private void Start()
    {
        if (slider != null)
        {
            // Asignar función al evento de cambio de valor del slider
            slider.onValueChanged.AddListener(UpdateSliderValueText);
            // Inicializar el texto del slider
            UpdateSliderValueText(slider.value);
        }
        else
        {
            Debug.LogError("Slider reference is not assigned.");
        }
    }

    private void UpdateSliderValueText(float value)
    {
        if (sliderValueText != null)
        {
            sliderValueText.text = value.ToString("F2");
        }
        else
        {
            Debug.LogError("Text component reference is not assigned.");
        }
    }
}
