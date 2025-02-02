using UnityEngine;
using UnityEngine.UI;

public class GeometriaPiramidal : MonoBehaviour
{
    // Prefabs para instanciar
    public GameObject piramidePrefab;
    public GameObject arrowPrefab;

    // Grosor de la flecha en X y Z
    public float arrowThickness = 0.5f;
    public float arrowLength = 1f;

    // Parámetros de la esfera
    public float radioEsfera = 5f;
    public float angularStepDegrees = 10f;
    public float alturaPiramide = 1f;
    public Vector3 sphereOrigin = Vector3.zero;

    // Parámetros del cilindro
    public float radioCilindro = 5f;
    public float alturaCilindro = 10f;
    public int divisionesAltura = 10; // Número de divisiones en altura

    // UI Elements
    public Button botonCrearEsfera;
    public Button botonCrearCilindro;
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
                    Vector3 arrowScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
                    arrow.transform.localScale = arrowScale;
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
            float nextAltura = alturaActual + alturaStep;

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
                    Vector3 arrowScale = new Vector3(arrowThickness, arrowLength, arrowThickness);
                    arrow.transform.localScale = arrowScale;
                }
            }
        }
        Debug.Log("Cilindro de pirámides creado.");
    }
}