using UnityEngine;

/// <summary>
/// Coloca este script en el GameObject del CEPILLO DE DIENTES.
/// El cepillo sigue al mouse/touch y borra la suciedad del diente al pasar encima.
/// </summary>
// Controla el comportamiento del cepillo: movimiento, sonido, partículas y limpieza del diente
public class ToothBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    // Velocidad de suavizado para que el cepillo no se mueva bruscamente
    [Range(5f, 30f)] public float followSpeed = 15f;

    // Determina si el script debe leer la posición del mouse automáticamente
    public bool followMouse = true;

    // Ajusta la posición visual del cepillo para que no tape exactamente el punto de limpieza
    public Vector2 brushOffset = new Vector2(0f, 0.5f);

    // Si es true, el cepillo girará mirando hacia donde se mueve el mouse
    public bool rotateToBrushDirection = false;

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)] public float velocidadLimpieza = 0.02f;

    [Header("Efectos")]
    // Sistema de partículas para simular burbujas o espuma de pasta
    public ParticleSystem foamParticles;

    // Sonido de cepillado
    public AudioClip brushSound;

    // Variables internas de control
    private Camera mainCamera;
    private AudioSource audioSource;
    private bool isBrushing = false;
    private Vector2 currentVelocity; // Usada por SmoothDamp para el movimiento suave

    private float brushAnimTime = 0f;
    [Header("Animación de cepillado")]
    // Qué tanto se mueve el cepillo de lado a lado al limpiar
    public float brushAnimAmplitude = 0.05f;
    // Qué tan rápido vibra el cepillo
    public float brushAnimSpeed = 8f;

    void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        // Si no hay AudioSource pero sí hay un sonido asignado, lo crea automáticamente
        if (audioSource == null && brushSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

    }

    void Update()
    {
        HandleInput();      // Paso 1: Leer entrada del usuario (Mouse/Touch)
        HandleBrushing();   // Paso 2: Procesar la limpieza y efectos
        HandleAnimation();  // Paso 3: Aplicar vibración visual
    }

    // Gestiona el movimiento del cepillo siguiendo al puntero
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

        targetPos += brushOffset; // Aplica el desfase visual configurado

        // Mueve el objeto hacia targetPos con un retraso suave para que se sienta orgánico
        Vector2 newPos = Vector2.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            1f / followSpeed
        );

        // Calcula la rotación basada en la dirección del movimiento
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

        if (audioSource != null && brushSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = brushSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Aplica una pequeña vibración (senoidal) para que el cepillo parezca estar limpiando de verdad
    void HandleAnimation()
    {
        if (isBrushing && brushAnimAmplitude > 0)
        {
            brushAnimTime += Time.deltaTime * brushAnimSpeed;
            // Calcula un desplazamiento oscilatorio en el eje X
            float sideOffset = Mathf.Sin(brushAnimTime) * brushAnimAmplitude;
            transform.position += new Vector3(sideOffset * Time.deltaTime, 0, 0);
        }

        // Si el usuario suelta el clic, detiene todos los efectos
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
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
}