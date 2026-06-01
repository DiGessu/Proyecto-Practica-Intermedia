using UnityEngine;

public class ToothpasteBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)]
    public float followSpeed = 15f;

    public bool followMouse = true;

    public Vector2 brushOffset =
        new Vector2(0f, 0.5f);

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)]
    public float velocidadLimpieza = 0.02f;

    private Camera mainCamera;

    private bool isBrushing = false;

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
        isBrushing = Input.GetMouseButton(0);

        if (!followMouse) return;

        Vector2 targetPos;

        targetPos =
            mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );

        targetPos += brushOffset;

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
        Debug.Log("[ToothpasteBrush] Trigger con: " + collision.gameObject.name);

        if(!isBrushing) return; //
        EstadoDiente diente = collision.GetComponent<EstadoDiente>(); //
        if (diente == null) return; //

        // === NUEVA COMPROBACIÓN ===
        // Si el diente NO está siendo limpiado efectivamente por el CEPILLO CON PASTA, no hace espuma
        if (!diente.EstaSiendoLimpiadoPor(TipoHerramienta.CEPILLO_CON_PASTA)) return;

        Vector3 puntoContacto = collision.ClosestPoint(transform.position);
        FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);

        diente.LimpiarGradual(TipoHerramienta.CEPILLO_CON_PASTA, velocidadLimpieza); //
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isBrushing)
        {
            Debug.Log("[ToothpasteBrush] Tocando " + collision.gameObject.name + " pero NO hay click");
            return;
        }
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null)
        {
            Debug.Log("[ToothpasteBrush] " + collision.gameObject.name + " NO tiene EstadoDiente");
            return;
        }
        Debug.Log("[ToothpasteBrush] Limpiando " + collision.gameObject.name + " | Estado: " + diente.estadoActual);
        diente.LimpiarGradual(TipoHerramienta.CEPILLO_CON_PASTA, velocidadLimpieza);
    }
}