using UnityEngine;

/// <summary>
/// Coloca este script en el GameObject del CEPILLO DE DIENTES.
/// El cepillo sigue al mouse/touch y borra la suciedad del diente al pasar encima.
/// </summary>
public class ToothBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)] public float followSpeed = 15f;
    public bool followMouse = true;
    public Vector2 brushOffset = new Vector2(0f, 0.5f);
    public bool rotateToBrushDirection = false;

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)] public float velocidadLimpieza = 0.02f;

    [Header("Efectos")]
    public ParticleSystem foamParticles;
    public AudioClip brushSound;

    // Variables internas de control
    private Camera mainCamera;
    private AudioSource audioSource;
    private bool isBrushing = false;
    private Vector2 currentVelocity;
    private float brushAnimTime = 0f;

    // === NUEVA VARIABLE EXCLUSIVA PARA EL SONIDO FLUIDO ===
    private SonidoCepilloNormal controladorSonido;

    [Header("Animación de cepillado")]
    public float brushAnimAmplitude = 0.05f;
    public float brushAnimSpeed = 8f;

    void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && brushSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Buscamos nuestro nuevo componente de sonido exclusivo en la escena
        controladorSonido = Object.FindFirstObjectByType<SonidoCepilloNormal>();
    }

    void Update()
    {
        HandleInput();
        HandleBrushing();
        HandleAnimation();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            isBrushing = (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
        }
        else
        {
            isBrushing = Input.GetMouseButton(0);
        }

        if (!followMouse) return;

        Vector2 targetPos = Vector2.zero;

        if (Input.touchCount > 0)
        {
            targetPos = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        targetPos += brushOffset;

        Vector2 newPos = Vector2.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            1f / followSpeed
        );

        if (rotateToBrushDirection)
        {
            Vector2 dir = newPos - (Vector2)transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.Euler(0, 0, angle),
                    Time.deltaTime * 10f
                );
            }
        }

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void HandleBrushing()
    {
        if (!isBrushing) return;

        if (foamParticles != null && !foamParticles.isPlaying)
            foamParticles.Play();

        // NOTA: Se deja esto intacto tal como lo tenías por si usas audio nativo alternativo, 
        // pero el control fluido real lo hará ahora el AudioManagerNormal de fondo.
        if (audioSource != null && brushSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = brushSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void HandleAnimation()
    {
        if (isBrushing && brushAnimAmplitude > 0)
        {
            brushAnimTime += Time.deltaTime * brushAnimSpeed;
            float sideOffset = Mathf.Sin(brushAnimTime) * brushAnimAmplitude;
            transform.position += new Vector3(sideOffset * Time.deltaTime, 0, 0);
        }

        if (!isBrushing)
        {
            brushAnimTime = 0f;
            if (foamParticles != null && foamParticles.isPlaying) foamParticles.Stop();
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[ToothBrush] Trigger con: " + collision.gameObject.name);

        // === CONTACTO CON EL SCRIPT DE AUDIO ===
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isBrushing) return;
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null) return;

        if (!diente.EstaSiendoLimpiadoPor(TipoHerramienta.CEPILLO)) return;

        Vector3 puntoContacto = collision.ClosestPoint(transform.position);
        FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);

        diente.LimpiarGradual(TipoHerramienta.CEPILLO, velocidadLimpieza);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // === MANTENER EL CONTACTO CON EL AUDIO ===
        if (controladorSonido != null) controladorSonido.SetTocandoDiente(true);

        if (!isBrushing)
        {
            Debug.Log("[ToothBrush] Tocando " + collision.gameObject.name + " pero NO hay click");
            return;
        }
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente == null)
        {
            Debug.Log("[ToothBrush] " + collision.gameObject.name + " NO tiene EstadoDiente");
            return;
        }
        Debug.Log("[ToothBrush] Limpiando " + collision.gameObject.name + " | Estado: " + diente.estadoActual);
        diente.LimpiarGradual(TipoHerramienta.CEPILLO, velocidadLimpieza);
    }

    // === NUEVO TRIGGER EXIT: Corta el sonido al salir del diente ===
    private void OnTriggerExit2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();
        if (diente != null)
        {
            if (controladorSonido != null) controladorSonido.SetTocandoDiente(false);
        }
    }
}