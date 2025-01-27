using UnityEngine;

public class Flecha : MonoBehaviour
{
    public bool esPositiva;

    private new Renderer renderer; // Usar 'new' para ocultar el miembro heredado

    private void Start()
    {
        renderer = GetComponentInChildren<Renderer>(); // Asegúrate de buscar el renderer en los hijos
        if (renderer == null)
        {
            Debug.LogError("Renderer component not found.");
        }
        ActualizarColor();
    }

    public void ConfigurarFlecha(Vector3 startPoint, Vector3 endPoint, bool esPositiva)
    {
        this.esPositiva = esPositiva;
        ActualizarColor();
        Vector3 direccion = ActualizarDireccion(startPoint, endPoint);
        AjustarTamaño(startPoint, endPoint, direccion);
    }

    private void ActualizarColor()
    {
        if (renderer != null)
        {
            renderer.material.color = esPositiva ? Color.red : Color.blue; // Rojo para positiva, azul para negativa
        }
    }

    private Vector3 ActualizarDireccion(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direccion = esPositiva ? (endPoint - startPoint) : (startPoint - endPoint);
        transform.rotation = Quaternion.LookRotation(direccion);

        // Asegurarse de que la flecha está orientada en la dirección correcta
        transform.Rotate(90, 0, 0);

        return direccion;
    }

    private void AjustarTamaño(Vector3 startPoint, Vector3 endPoint, Vector3 direccion)
    {
        // Calcular la distancia entre startPoint y endPoint
        float distancia = Vector3.Distance(startPoint, endPoint);

        // Definir espacio fijo desde la partícula y desde el sensor
        float espacioParticula = 0.1f;
        float espacioSensor = 0.1f;

        // Calcular el tamaño ajustado
        float tamañoAjustado = Mathf.Max(0, distancia - (espacioParticula + espacioSensor));

        Transform cuerpo = transform.Find("Cuerpo");
        if (cuerpo != null)
        {
            Vector3 escalaOriginal = cuerpo.localScale;
            cuerpo.localScale = new Vector3(escalaOriginal.x, tamañoAjustado, escalaOriginal.z); // Ajustar solo en el eje Y
            cuerpo.localPosition = new Vector3(0, tamañoAjustado / 2, 0); // Asegurar que el cuerpo se extiende desde el centro
        }

        // Ajustar la posición de la punta de la flecha
        Transform punta = transform.Find("Punta");
        if (punta != null)
        {
            punta.localPosition = new Vector3(0, tamañoAjustado, 0); // Colocar la punta al final del cuerpo
        }

        // Ajustar la posición global de la flecha para que comience 0.1 unidades más cerca de la partícula
        Vector3 posicionAjustada = startPoint + (direccion.normalized * espacioParticula);
        transform.position = posicionAjustada;
    }
}
