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

    [Header("Referencia al diente")]
    // Referencia al script que maneja la suciedad (la lógica de "borrar" la mancha)
    public ToothDirtMask toothMask;

    [Header("Efectos")]
    // Sistema de partículas para simular burbujas o espuma de pasta
    public ParticleSystem foamParticles;

    // Sonido de cepillado
    public AudioClip brushSound;

    // Variables internas de control
    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 lastPosition;
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

        lastPosition = transform.position;
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
        if (!followMouse) return;

        Vector2 targetPos = Vector2.zero;

        // Soporte para pantallas táctiles (Móviles)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            targetPos = mainCamera.ScreenToWorldPoint(touch.position);
            isBrushing = (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
        }
        // Soporte para Mouse (PC)
        else
        {
            targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isBrushing = Input.GetMouseButton(0); // True si el clic está presionado
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

        lastPosition = transform.position;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    // Gestiona la lógica de limpieza y activa los efectos visuales/auditivos
    void HandleBrushing()
    {
        if (!isBrushing || toothMask == null) return;

        // Calcula el punto exacto donde las cerdas tocan el diente (restando el offset)
        Vector2 contactPoint = (Vector2)transform.position - brushOffset;

        // Llama al script del diente para borrar la suciedad en esa posición
        toothMask.EraseAt(contactPoint);

        // Activa la espuma si no se está reproduciendo
        if (foamParticles != null && !foamParticles.isPlaying)
            foamParticles.Play();

        // Activa el sonido en bucle mientras se cepilla
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

    // Permite que otros scripts (como una cinemática) controlen el cepillo manualmente
    public void BrushAt(Vector2 worldPosition, bool brushing)
    {
        isBrushing = brushing;
        if (brushing && toothMask != null)
            toothMask.EraseAt(worldPosition);
    }

    // Dibuja un círculo en el editor de Unity para visualizar el punto de contacto
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((Vector2)transform.position - brushOffset, 0.1f);
    }

    // Si el cepillo toca un objeto con el script ToothDirtMask, limpia el diente por completo (limpieza total)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ToothDirtMask>())
        {
            collision.gameObject.GetComponent<ToothDirtMask>().ClearTooth();
        }
    }
}