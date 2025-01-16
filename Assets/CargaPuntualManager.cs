using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con botones de UI

public class CargaPuntualManager : MonoBehaviour
{
    public GameObject subMenuCarga; // Referencia al submenú donde se encuentran las opciones de carga
    public Button insertarCargaPositivaBtn; // Referencia al botón de carga positiva
    public Button insertarCargaNegativaBtn; // Referencia al botón de carga negativa
    public Transform puntoDeCarga; // El punto de la simulación donde se insertarán las cargas
    public float cargaValorPositiva = 10f; // Valor de la carga positiva
    public float cargaValorNegativa = -10f; // Valor de la carga negativa

    private void Start()
    {
        // Desactivamos el submenú de carga al inicio
        subMenuCarga.SetActive(false);

        // Asignamos las funciones a los botones
        insertarCargaPositivaBtn.onClick.AddListener(IngresarCargaPositiva);
        insertarCargaNegativaBtn.onClick.AddListener(IngresarCargaNegativa);
    }

    // Método para mostrar el submenú cuando se presiona el botón "Simulación de Cargas Puntuales"
    public void MostrarSubMenuCargas()
    {
        subMenuCarga.SetActive(true);
    }

    // Método para insertar una carga puntual positiva en la simulación
    public void IngresarCargaPositiva()
    {
        CrearCarga(cargaValorPositiva);
    }

    // Método para insertar una carga puntual negativa en la simulación
    public void IngresarCargaNegativa()
    {
        CrearCarga(cargaValorNegativa);
    }

    // Método para crear una carga en el punto de carga especificado
    private void CrearCarga(float cargaValor)
    {
        if (puntoDeCarga != null)
        {
            // Creación de la carga como un cubo (por ejemplo) que simboliza la carga puntual
            GameObject carga = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carga.transform.position = puntoDeCarga.position;

            // Establecemos un valor de "carga" como nombre del objeto, o puedes agregarle un componente para representar la carga
            carga.name = "Carga " + cargaValor;

            // Opcional: puedes añadirle un componente que represente la carga (por ejemplo, un script o un visualizador)
            Debug.Log("Carga insertada: " + cargaValor);
        }
        else
        {
            Debug.LogError("Punto de carga no asignado.");
        }
    }
}