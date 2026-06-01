using UnityEngine;

public class CottonTool : MonoBehaviour
{
    [Header("Configuraci�n")]
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

    void Awake()
    {
        mainCamera = Camera.main;
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

        if(!isCleaning) return; //
        EstadoDiente diente = collision.GetComponent<EstadoDiente>(); //
        if (diente == null) return; //

        // === NUEVA COMPROBACIÓN ===
        // Si el diente NO está siendo limpiado efectivamente por el ALGODÓN, no hace espuma
        if (!diente.EstaSiendoLimpiadoPor(TipoHerramienta.ALGODON)) return;

        Vector3 puntoContacto = collision.ClosestPoint(transform.position);
        FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);

        diente.LimpiarGradual(TipoHerramienta.ALGODON, velocidadLimpieza); //
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
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
}