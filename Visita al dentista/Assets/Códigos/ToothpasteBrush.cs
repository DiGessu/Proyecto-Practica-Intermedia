using UnityEngine;

public class ToothpasteBrush : MonoBehaviour
{
    // ... TODA TU CONFIGURACIÓN ANTERIOR SE QUEDA IGUAL ...
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)]
    public float followSpeed = 15f;
    public bool followMouse = true;
    public Vector2 brushOffset = new Vector2(0f, 0.5f);

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)]
    public float velocidadLimpieza = 0.02f;

    // === NUEVA VARIABLE PARA EL SONIDO ===
    private ControladorSonidoCepillo controladorSonido;

    private Camera mainCamera;
    private bool isBrushing = false;
    private Vector2 currentVelocity;

    void Awake()
    {
        mainCamera = Camera.main;
        // Buscamos el componente de sonido en la escena
        controladorSonido = Object.FindFirstObjectByType<ControladorSonidoCepillo>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        isBrushing = Input.GetMouseButton(0);
        // ... (Tu código de movimiento SmoothDamp se queda exactamente igual) ...
        if (!followMouse) return;
        Vector2 targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetPos += brushOffset;
        Vector2 newPos = Vector2.SmoothDamp(transform.position, targetPos, ref currentVelocity, 1f / followSpeed);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null) return;

        // Avisamos al manager de audio
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isBrushing) return;
        if (!diente.EstaSiendoLimpiadoPor(TipoHerramienta.CEPILLO_CON_PASTA)) return;

        Vector3 puntoContacto = collision.ClosestPoint(transform.position);
        FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);
        diente.LimpiarGradual(TipoHerramienta.CEPILLO_CON_PASTA, velocidadLimpieza);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null) return;

        // Mantenemos activo el estado del audio mientras siga dentro del collider
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isBrushing) return;
        diente.LimpiarGradual(TipoHerramienta.CEPILLO_CON_PASTA, velocidadLimpieza);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente != null)
        {
            if (controladorSonido != null) controladorSonido.SetTocandoDiente(false);
        }
    }
}