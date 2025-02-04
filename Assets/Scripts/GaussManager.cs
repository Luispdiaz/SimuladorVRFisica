using UnityEngine;
using UnityEngine.UI;

public class GeometriaPiramidal : MonoBehaviour
{
    // Prefabs para instanciar
    public GameObject piramidePrefab;
    public GameObject arrowPrefab;
    // Prefab para el plano
    public GameObject planoPrefab;

    // Grosor de la flecha en X y Z
    public float arrowThickness = 0.5f;
    public float arrowLength = 1f;
    public float arrowHeight = 1f;  // Altura ajustable de los vectores
    public float arrowWidth = 0.5f; // Ancho ajustable de los vectores

    // Parámetros de la esfera
    public float radioEsfera = 5f;
    public float angularStepDegrees = 10f;
    public float alturaPiramide = 1f;
    public Vector3 sphereOrigin = Vector3.zero;

    // Parámetros del cilindro
    public float radioCilindro = 5f;
    public float alturaCilindro = 10f;
    public int divisionesAltura = 10; // Número de divisiones en altura

    // Parámetros del plano
    // Se define el tamaño total del plano en los ejes X e Y.
    public float anchoPlano = 10f;  // Tamaño en el eje X
    public float altoPlano = 10f;   // Tamaño en el eje Y
    // Offset para bajar la posición del plano (por ejemplo, 0.5 unidades)
    public float planeYOffset = 0.5f;

    // UI Elements
    public Button botonCrearEsfera;
    public Button botonCrearCilindro;
    public Button botonCrearPlano;   // Botón para crear el plano
    public GameObject subMenuCarga;
    public GameObject tituloSubMenu;
    public Button closeButton;

    private void Start()
    {
        // Ocultar submenús
        subMenuCarga.SetActive(false);
        tituloSubMenu.SetActive(false);

        // Asignar funciones a los botones
        closeButton.onClick.AddListener(CerrarSubMenu);
        botonCrearEsfera.onClick.AddListener(CrearEsferaDePiramides);
        botonCrearCilindro.onClick.AddListener(CrearCilindroDePiramides);
        botonCrearPlano.onClick.AddListener(CrearPlano);
    }

    public void CerrarSubMenu()
    {
        subMenuCarga.SetActive(false);
        tituloSubMenu.SetActive(false);
    }

    private void CrearEsferaDePiramides()
    {
        float angularStep = angularStepDegrees * Mathf.Deg2Rad;
        int latDivisions = Mathf.CeilToInt(Mathf.PI / angularStep);

        for (int i = 0; i < latDivisions; i++)
        {
            float lat1 = -Mathf.PI / 2 + i * angularStep;
            float lat2 = lat1 + angularStep;
            lat2 = Mathf.Min(lat2, Mathf.PI / 2);
            float latCenter = (lat1 + lat2) / 2f;
            float dLat = lat2 - lat1;

            int numLon = Mathf.Max(1, Mathf.CeilToInt((2 * Mathf.PI * Mathf.Cos(latCenter)) / angularStep));
            float dLon = 2 * Mathf.PI / numLon;

            for (int j = 0; j < numLon; j++)
            {
                float lonCenter = j * dLon + dLon / 2f;

                Vector3 normal = new Vector3(
                    Mathf.Cos(latCenter) * Mathf.Cos(lonCenter),
                    Mathf.Sin(latCenter),
                    Mathf.Cos(latCenter) * Mathf.Sin(lonCenter)
                );

                Vector3 centerPos = sphereOrigin + normal * radioEsfera;
                float patchWidth = radioEsfera * dLon * Mathf.Cos(latCenter);
                float patchHeight = radioEsfera * dLat;
                Vector3 escala = new Vector3(patchWidth, alturaPiramide, patchHeight);

                Vector3 inward = -normal;
                Vector3 tangent = Vector3.Cross(inward, Vector3.up);
                if (tangent.sqrMagnitude < 0.001f)
                {
                    tangent = Vector3.Cross(inward, Vector3.right);
                }
                tangent.Normalize();

                Quaternion rot = Quaternion.LookRotation(tangent, inward);
                GameObject piramide = Instantiate(piramidePrefab, centerPos, rot);
                piramide.transform.localScale = escala;

                if (arrowPrefab != null)
                {
                    Quaternion arrowRot = Quaternion.LookRotation(tangent, normal);
                    GameObject arrow = Instantiate(arrowPrefab, centerPos, arrowRot, piramide.transform);
                    arrow.transform.localPosition = Vector3.zero;
                    arrow.transform.localScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
                }
            }
        }
        Debug.Log("Esfera de pirámides creada.");
    }

    private void CrearCilindroDePiramides()
    {
        float angularStep = angularStepDegrees * Mathf.Deg2Rad;
        int numDivisionesCircunferencia = Mathf.CeilToInt(2 * Mathf.PI / angularStep);
        float alturaStep = alturaCilindro / divisionesAltura;

        for (int i = 0; i <= divisionesAltura; i++)
        {
            float alturaActual = -alturaCilindro / 2 + i * alturaStep;

            for (int j = 0; j < numDivisionesCircunferencia; j++)
            {
                float angulo = j * angularStep;

                Vector3 normal = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo));
                Vector3 centerPos = sphereOrigin + normal * radioCilindro + new Vector3(0, alturaActual, 0);

                float patchWidth = radioCilindro * angularStep;
                float patchHeight = alturaStep;
                Vector3 escala = new Vector3(patchWidth, alturaPiramide, patchHeight);

                Vector3 inward = -normal;
                Vector3 tangent = Vector3.Cross(inward, Vector3.up);
                if (tangent.sqrMagnitude < 0.001f)
                {
                    tangent = Vector3.Cross(inward, Vector3.right);
                }
                tangent.Normalize();

                Quaternion rot = Quaternion.LookRotation(tangent, inward);
                GameObject piramide = Instantiate(piramidePrefab, centerPos, rot);
                piramide.transform.localScale = escala;

                if (arrowPrefab != null)
                {
                    Quaternion arrowRot = Quaternion.LookRotation(tangent, normal);
                    GameObject arrow = Instantiate(arrowPrefab, centerPos, arrowRot, piramide.transform);
                    arrow.transform.localPosition = Vector3.zero;
                    arrow.transform.localScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
                }
            }
        }
        Debug.Log("Cilindro de pirámides creado.");
    }

    /// <summary>
    /// Inserta un único plano orientado en los ejes X e Y (acostado), es decir, su superficie se extiende en X e Y.
    /// Se aplica un pequeño offset para bajarlo.
    /// </summary>
    private void CrearPlano()
    {
        Vector3 planePosition = sphereOrigin + new Vector3(0, -planeYOffset, 0);
        Quaternion planeRot = Quaternion.Euler(90, 0, 0);

        GameObject plane = Instantiate(planoPrefab, planePosition, planeRot);
        plane.transform.localScale = new Vector3(anchoPlano, altoPlano, 1);

        // Establecer el tamaño de la cuadrícula a 3x3 para obtener 9 vectores
        int gridSizeX = 3;
        int gridSizeY = 3;
        float spacingX = anchoPlano / gridSizeX;
        float spacingY = altoPlano / gridSizeY;

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector3 position = planePosition + new Vector3(
                    -anchoPlano / 2 + i * spacingX + spacingX / 2,
                    planeYOffset,
                    -altoPlano / 2 + j * spacingY + spacingY / 2
                );

                if (arrowPrefab != null)
                {
                    Quaternion arrowRot = Quaternion.Euler(0, 0, 0); // Flechas apuntando hacia arriba
                    GameObject arrow = Instantiate(arrowPrefab, position - new Vector3(0, 1.5f, 0), arrowRot);
                    arrow.transform.localScale = new Vector3(arrowWidth, arrowHeight, arrowWidth);
                }
            }
        }

        Debug.Log("Plano insertado con 9 vectores.");
    }

}
