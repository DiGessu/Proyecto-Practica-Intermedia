using UnityEngine;

/// <summary>
/// Coloca este script en el GameObject del CEPILLO DE DIENTES.
/// El cepillo sigue al mouse/touch y borra la suciedad del diente al pasar encima.
/// </summary>
public class ToothBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Tooltip("Velocidad con que el cepillo sigue al cursor (más alto = más inmediato)")]
    [Range(5f, 30f)]
    public float followSpeed = 15f;

    [Tooltip("¿El cepillo sigue al mouse? (false = tú mueves el cepillo por código o animación)")]
    public bool followMouse = true;

    [Tooltip("Offset visual del cepillo respecto al punto de contacto")]
    public Vector2 brushOffset = new Vector2(0f, 0.5f);

    [Tooltip("¿Rotar el cepillo según la dirección de movimiento?")]
    public bool rotateToBrushDirection = false;

    [Header("Referencia al diente")]
    [Tooltip("Arrastra aquí el GameObject del diente sucio (el que tiene ToothDirtMask)")]
    public ToothDirtMask toothMask;

    [Header("Efectos")]
    [Tooltip("Partículas de espuma al cepillar (opcional)")]
    public ParticleSystem foamParticles;

    [Tooltip("AudioClip del sonido de cepillado (opcional)")]
    public AudioClip brushSound;

    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 lastPosition;
    private bool isBrushing = false;
    private Vector2 currentVelocity;

    // Para animar el cepillo con efecto de "cepillado" (oscilación)
    private float brushAnimTime = 0f;
    [Header("Animación de cepillado")]
    [Tooltip("Amplitud del movimiento de cepillado automático")]
    public float brushAnimAmplitude = 0.05f;
    [Tooltip("Velocidad del movimiento de cepillado")]
    public float brushAnimSpeed = 8f;

    void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && brushSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        lastPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        HandleBrushing();
        HandleAnimation();
    }

    void HandleInput()
    {
        if (!followMouse) return;

        Vector2 targetPos = Vector2.zero;

        // Soporte mouse y touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            targetPos = mainCamera.ScreenToWorldPoint(touch.position);
            isBrushing = (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
        }
        else
        {
            targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isBrushing = Input.GetMouseButton(0);
        }

        // Aplicar offset
        targetPos += brushOffset;

        // Mover el cepillo suavemente hacia la posición del cursor
        Vector2 newPos = Vector2.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            1f / followSpeed
        );

        // Rotar según dirección de movimiento
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

        lastPosition = transform.position;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void HandleBrushing()
    {
        if (!isBrushing || toothMask == null) return;

        // Punto de contacto del cepillo (sin el offset visual)
        Vector2 contactPoint = (Vector2)transform.position - brushOffset;

        // Borrar en la máscara del diente
        toothMask.EraseAt(contactPoint);

        // Efectos opcionales
        if (foamParticles != null && !foamParticles.isPlaying)
            foamParticles.Play();

        if (audioSource != null && brushSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = brushSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void HandleAnimation()
    {
        // Si el cepillo se está usando y tiene animación activa
        if (isBrushing && brushAnimAmplitude > 0)
        {
            brushAnimTime += Time.deltaTime * brushAnimSpeed;
            // Pequeña oscilación lateral que simula el movimiento de cepillado
            float sideOffset = Mathf.Sin(brushAnimTime) * brushAnimAmplitude;
            transform.position += new Vector3(sideOffset * Time.deltaTime, 0, 0);
        }

        // Detener efectos cuando no se cepilla
        if (!isBrushing)
        {
            brushAnimTime = 0f;

            if (foamParticles != null && foamParticles.isPlaying)
                foamParticles.Stop();

            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    /// <summary>
    /// Llama este método si controlas el cepillo desde otro script
    /// (por ejemplo, con animaciones o botones en lugar del mouse).
    /// </summary>
    public void BrushAt(Vector2 worldPosition, bool brushing)
    {
        isBrushing = brushing;
        if (brushing && toothMask != null)
            toothMask.EraseAt(worldPosition);
    }

    void OnDrawGizmosSelected()
    {
        // Mostrar el área de contacto en el editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((Vector2)transform.position - brushOffset, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ToothDirtMask>())
        {
            
            collision.gameObject.GetComponent<ToothDirtMask>().ClearTooth();
        }
    }
}