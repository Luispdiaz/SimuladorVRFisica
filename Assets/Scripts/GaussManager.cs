using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [Header("Posiciones Independientes")]
    public Vector3 sphereOrigin = Vector3.zero;
    public Vector3 cylinderOrigin = new Vector3(0, 0, 5f); // Posición diferente por defecto

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
    private List<GameObject> sensores = new List<GameObject>();

    [Header("Simetría")]
    public Button botonSimetria;
    public CargaPuntualManager cargaManager; // Asigna este manager en el Inspector
    public float fuerzaAtraccion = 10f;

    // Agrega estas variables al inicio de tu clase
    private bool esferaGenerada = false;
    private bool cilindroGenerado = false;


    private List<GameObject> piramides = new List<GameObject>();
    public List<GameObject> Piramides => piramides; // Propiedad de solo lectura

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
        botonSimetria.onClick.AddListener(AplicarSimetria);
    }

    public void CerrarSubMenu()
    {
        subMenuCarga.SetActive(false);
        tituloSubMenu.SetActive(false);
    }

    private void CrearEsferaDePiramides()
    {
        esferaGenerada = true;
        float angularStep = angularStepDegrees * Mathf.Deg2Rad;
        int latDivisions = Mathf.CeilToInt(Mathf.PI / angularStep);

        // Crear un punto de referencia en el centro de la esfera (como una esfera pequeña)
        GameObject puntoReferencia = GameObject.CreatePrimitive(PrimitiveType.Sphere);  // Crear una esfera
        puntoReferencia.transform.position = sphereOrigin;  // Colocarlo en el centro
        puntoReferencia.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);  // Hacerla más pequeña
        puntoReferencia.GetComponent<Renderer>().material.color = new Color(0.5f, 1f, 0.5f);  // Verde claro (RGB)

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
                centerPos -= normal * 0.5f; // Acercar 0.5 unidades hacia el centro

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

                Quaternion rot = Quaternion.LookRotation(tangent, inward) * Quaternion.Euler(0, 45, 0);

                GameObject piramide = Instantiate(piramidePrefab, centerPos, rot);

                piramide.transform.localScale = escala;

                // Agrega la pirámide a la lista
                piramides.Add(piramide);

                // Asegúrate de que los objetos creados no afecten la física
                Rigidbody rb = piramide.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;  // Desactiva la física
                }

                Collider collider = piramide.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false; // Desactiva el collider
                }

                // Ignorar la colisión entre el sensor y la esfera
                foreach (var sensor in sensores)
                {
                    Collider sensorCollider = sensor.GetComponent<Collider>();
                    if (sensorCollider != null)
                    {
                        Physics.IgnoreCollision(sensorCollider, collider);
                    }
                }

                
            }
        }
        Debug.Log("Esfera de pirámides creada.");
    }




    private void CrearCilindroDePiramides()
    {
        cilindroGenerado = true;

        float angularStep = angularStepDegrees * Mathf.Deg2Rad;
        int numDivisionesCircunferencia = Mathf.CeilToInt(2 * Mathf.PI / angularStep);
        float alturaStep = alturaCilindro / divisionesAltura;

        // Para determinar un tamaño base (lateral) uniforme, calculamos una escala global:
        float patchWidthGlobal = radioCilindro * angularStep;
        float patchHeightGlobal = alturaStep;
        float sideGlobal = Mathf.Max(patchWidthGlobal, patchHeightGlobal);

        // --- CREAR SUPERFICIE LATERAL DEL CILINDRO ---
        for (int i = 0; i <= divisionesAltura; i++)
        {
            float alturaActual = -alturaCilindro / 2 + i * alturaStep;
            for (int j = 0; j < numDivisionesCircunferencia; j++)
            {
                float angulo = j * angularStep;
                Vector3 normal = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo));
                // Posición en el lateral del cilindro
                Vector3 centerPos = cylinderOrigin + normal * radioCilindro + new Vector3(0, alturaActual, 0);
                centerPos -= normal * 0.35f;

                // Usamos el tamaño global calculado
                Vector3 escala = new Vector3(sideGlobal, alturaPiramide, sideGlobal);

                Vector3 inward = -normal;
                Vector3 tangent = Vector3.Cross(inward, Vector3.up);
                if (tangent.sqrMagnitude < 0.001f)
                {
                    tangent = Vector3.Cross(inward, Vector3.right);
                }
                tangent.Normalize();

                // Rotación para el lateral: alineamos el forward con el vector tangente y le sumamos 45° en Y.
                Quaternion rot = Quaternion.LookRotation(tangent, inward) * Quaternion.Euler(0, 45, 0);

                GameObject piramide = Instantiate(piramidePrefab, centerPos, rot);
                piramide.transform.localScale = escala;
                piramides.Add(piramide);

                // Desactivar física y collider
                Rigidbody rb = piramide.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = true;
                Collider collider = piramide.GetComponent<Collider>();
                if (collider != null)
                    collider.enabled = false;
                // Ignorar colisiones con sensores
                foreach (var sensor in sensores)
                {
                    Collider sensorCollider = sensor.GetComponent<Collider>();
                    if (sensorCollider != null && collider != null)
                    {
                        Physics.IgnoreCollision(sensorCollider, collider);
                    }
                }
            }
        }

        // --- CREAR TAPAS (CAPS) DEL CILINDRO ---
        // Factor para reducir el tamaño de las tapas
        float factorEscalaTapa = 0.8f;
        Vector3 escalaTapa = new Vector3(sideGlobal * factorEscalaTapa, alturaPiramide, sideGlobal * factorEscalaTapa);

        int radialDivisiones = 3; // Número de anillos radiales para cubrir cada tapa
        float factorRadioCap = 0.5f; // Reducir el radio máximo de la distribución (para juntar más las pirámides)

        // Offset vertical para que las tapas queden fuera del cilindro
        float verticalTapaOffset = 0.05f; // Ajusta este valor según lo necesites

        // Se crearán dos tapas: capa 0 para la inferior y capa 1 para la superior.
        for (int capa = 0; capa < 2; capa++)
        {
            // Para la tapa superior, sumamos el offset; para la inferior, lo restamos.
            float yCap = (capa == 1) ? (cylinderOrigin.y + alturaCilindro / 2 + verticalTapaOffset)
                                      : (cylinderOrigin.y - alturaCilindro / 2 - verticalTapaOffset);
            Vector3 capCenter = new Vector3(cylinderOrigin.x, yCap, cylinderOrigin.z);

            // Distribuir las instancias en anillos radiales
            for (int r = 0; r <= radialDivisiones; r++)
            {
                // Radio para el anillo actual, reducido por el factor para juntar las pirámides
                float radioActual = (r / (float)radialDivisiones) * (radioCilindro * factorRadioCap);
                // Número de divisiones angulares proporcional al anillo (mínimo 1)
                int divisionesAngulares = (radioActual < 0.01f) ? 1 : Mathf.CeilToInt((2 * Mathf.PI * radioActual) / (sideGlobal * factorEscalaTapa));
                for (int a = 0; a < divisionesAngulares; a++)
                {
                    float angulo = a * (2 * Mathf.PI / divisionesAngulares);
                    // Posición en la tapa en coordenadas polares
                    Vector3 posCapLocal = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo)) * radioActual;
                    Vector3 finalPos = capCenter + posCapLocal;
                    // Calcular la dirección desde esta posición hacia el centro de la tapa
                    Vector3 direccion = (capCenter - finalPos).normalized;
                    // Obtener una rotación que alinee el forward con la dirección
                    Quaternion rotCap = Quaternion.LookRotation(direccion);
                    // Aplicar offset de rotación independiente:
                    if (capa == 0)
                    {
                        // Tapa inferior: se puede ajustar el offset aquí si es necesario.
                        rotCap *= Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        // Tapa superior: se aplica un offset de 180° en X para que la punta apunte hacia adentro.
                        rotCap *= Quaternion.Euler(180, 0, 0);
                    }

                    GameObject tapa = Instantiate(piramidePrefab, finalPos, rotCap);
                    tapa.transform.localScale = escalaTapa;
                    piramides.Add(tapa);

                    // Desactivar física y collider
                    Rigidbody rb = tapa.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.isKinematic = true;
                    Collider col = tapa.GetComponent<Collider>();
                    if (col != null)
                        col.enabled = false;
                }
            }
        }

        Debug.Log("Cilindro de pirámides con tapas creado.");
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

    private void AplicarSimetria()
    {
        // --- LÓGICA PARA ESFERA (CARGAS) ---
        if (esferaGenerada && cargaManager != null && cargaManager.Cargas.Count > 0)
        {
            GameObject cargaCercana = null;
            float minDistancia = Mathf.Infinity;

            foreach (var carga in cargaManager.Cargas)
            {
                float distancia = Vector3.Distance(carga.transform.position, sphereOrigin);
                if (distancia < minDistancia)
                {
                    minDistancia = distancia;
                    cargaCercana = carga;
                }
            }

            if (cargaCercana != null)
            {
                cargaCercana.transform.position = sphereOrigin;
                cargaCercana.transform.rotation = Quaternion.identity; // Aseguramos rotación neutra
                Debug.Log($"Carga {cargaCercana.name} centrada en esfera");
            }
        }
        else if (!esferaGenerada && cargaManager != null && cargaManager.Cargas.Count > 0)
        {
            Debug.LogWarning("No se puede reposicionar carga: Esfera no generada");
        }

        // --- LÓGICA PARA CILINDRO (LÍNEAS) ---
        if (cilindroGenerado && cargaManager != null && cargaManager.lineasCarga.Count > 0)
        {
            GameObject lineaCercana = null;
            float minDistancia = Mathf.Infinity;

            foreach (var linea in cargaManager.lineasCarga)
            {
                float distancia = Vector3.Distance(linea.transform.position, cylinderOrigin);
                if (distancia < minDistancia)
                {
                    minDistancia = distancia;
                    lineaCercana = linea;
                }
            }

            if (lineaCercana != null)
            {
                // Reposicionamiento en un solo paso
                lineaCercana.transform.SetPositionAndRotation(
                    cylinderOrigin - (Vector3.up * (lineaCercana.transform.localScale.y / 2f)),
                    Quaternion.identity
                );
            }
        }
        // --- LÓGICA PARA PLANOS EN CILINDRO ---
        if (cilindroGenerado && cargaManager != null)
        {
            // Obtener planos creados en CargaPuntualManager
            List<GameObject> planos = cargaManager.ObtenerPlanos()
                .Where(p => p != null && p.CompareTag("PlanoFisico"))
                .ToList();

            foreach (var plano in planos)
            {
                // Reposicionar exactamente en el centro del cilindro y aplicar rotación para que quede paralelo al suelo
                Vector3 posicionPlano = cylinderOrigin; // Usamos el centro del cilindro
                Quaternion rotacionPlano = Quaternion.Euler(180, 0, 0);
                plano.transform.SetPositionAndRotation(posicionPlano, rotacionPlano);
                plano.transform.localScale = new Vector3(5f, 0.001f, 5f);
            }
        }

        // --- LÓGICA PARA SENSORES ---
        if (piramides != null && piramides.Count > 0 && cargaManager != null && cargaManager.sensores.Count > 0)
        {
            foreach (var sensor in cargaManager.sensores)
            {
                if (!sensor.activeInHierarchy) continue;

                // Usar LINQ para encontrar la pirámide más cercana (de cualquier forma)
                GameObject piramideCercana = piramides
                    .OrderBy(p => Vector3.Distance(sensor.transform.position, p.transform.position))
                    .FirstOrDefault();

                if (piramideCercana != null)
                {
                    sensor.transform.SetPositionAndRotation(
                        piramideCercana.transform.position,
                        piramideCercana.transform.rotation
                    );
                    Debug.Log($"Sensor {sensor.name} movido a pirámide más cercana");
                }
            }
        }
    }
    // Método para limpiar todas las formas
    public void LimpiarFormas(bool limpiarEsfera = true, bool limpiarCilindro = true)
    {
        // Limpiar pirámides según la forma
        List<GameObject> piramidesABorrar = new List<GameObject>();
        foreach (var p in piramides)
        {
            if (limpiarEsfera && p.name.Contains("Esfera")) piramidesABorrar.Add(p);
            if (limpiarCilindro && p.name.Contains("Cilindro")) piramidesABorrar.Add(p);
        }

        foreach (var p in piramidesABorrar)
        {
            piramides.Remove(p);
            Destroy(p);
        }

        // Actualizar banderas
        if (limpiarEsfera) esferaGenerada = false;
        if (limpiarCilindro) cilindroGenerado = false;
    }
}
