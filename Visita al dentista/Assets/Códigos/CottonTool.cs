using UnityEngine;

public class CottonTool : MonoBehaviour
{
    [Header("Configuracin")]
    [Range(5f, 30f)]
    public float followSpeed = 15f;

    public bool followMouse = true;

    public Vector2 toolOffset =
        new Vector2(0f, 0.5f);

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)]
    public float velocidadLimpieza = 0.02f;

    private Camera mainCamera;
    private bool isCleaning = false;
    private Vector2 currentVelocity;

    // === NUEVA VARIABLE EXCLUSIVA PARA EL SONIDO ===
    private SonidoAlgodon controladorSonido;

    void Awake()
    {
        mainCamera = Camera.main;
        // Buscamos el componente de sonido exclusivo del algodón en la escena
        controladorSonido = Object.FindFirstObjectByType<SonidoAlgodon>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        isCleaning = Input.GetMouseButton(0);

        if (!followMouse) return;

        Vector2 targetPos;

        targetPos =
            mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );

        targetPos += toolOffset;

        Vector2 newPos =
            Vector2.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                1f / followSpeed
            );

        transform.position =
            new Vector3(
                newPos.x,
                newPos.y,
                transform.position.z
            );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[CottonTool] Trigger con: " + collision.gameObject.name);

        // === CONTACTO CON EL SCRIPT DE AUDIO ===
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isCleaning) return;
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null) return;

        if (!diente.EstaSiendoLimpiadoPor(TipoHerramienta.ALGODON)) return;

        Vector3 puntoContacto = collision.ClosestPoint(transform.position);
        FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);

        diente.LimpiarGradual(TipoHerramienta.ALGODON, velocidadLimpieza);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // === MANTENER EL CONTACTO CON EL AUDIO ===
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isCleaning)
        {
            Debug.Log("[CottonTool] Tocando " + collision.gameObject.name + " pero NO hay click");
            return;
        }
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null)
        {
            Debug.Log("[CottonTool] " + collision.gameObject.name + " NO tiene EstadoDiente");
            return;
        }
        Debug.Log("[CottonTool] Limpiando " + collision.gameObject.name + " | Estado: " + diente.estadoActual);
        diente.LimpiarGradual(TipoHerramienta.ALGODON, velocidadLimpieza);
    }

    // === CORTA EL SONIDO AL SALIR DEL DIENTE ===
    private void OnTriggerExit2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente != null)
        {
            if (controladorSonido != null) controladorSonido.SetTocandoDiente(false);
        }
    }
}