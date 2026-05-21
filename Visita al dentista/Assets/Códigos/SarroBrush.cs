using UnityEngine;

public class SarroBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)] public float followSpeed = 15f;
    public bool followMouse = true;
    public Vector2 brushOffset = new Vector2(0f, 0.5f);
    public bool rotateToBrushDirection = false;

    [Header("Sprites del Cepillo (Visual)")]
    public Sprite spriteNormal;
    public Sprite spriteConPasta;
    private SpriteRenderer spriteRenderer;

    [Header("Mecánica Pasta de Dientes")]
    public bool tienePasta = false;
    public float tiempoPasta = 6f;
    private float timerPasta;

    [Header("Velocidad de Desgaste del Sarro")]
    public float velocidadBorradoSarro = 0.04f; // Qué tanta opacidad baja por cada frame de cepillado

    [Header("Efectos")]
    public ParticleSystem foamParticles;
    public AudioClip brushSound;

    // Variables internas de control
    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 lastPosition;
    private bool isBrushing = false;
    private Vector2 currentVelocity;

    private float brushAnimTime = 0f;
    [Header("Animación de cepillado")]
    public float brushAnimAmplitude = 0.05f;
    public float brushAnimSpeed = 8f;

    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && brushSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        lastPosition = transform.position;
        timerPasta = tiempoPasta;
    }

    void Update()
    {
        HandleInput();      // Movimiento suave del ratón/touch
        HandlePastaTimer(); // Desgaste de la pasta dental con el tiempo
        HandleAnimation();  // Vibración visual senoidal
    }

    void HandleInput()
    {
        if (!followMouse) return;

        Vector2 targetPos = Vector2.zero;

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

        lastPosition = transform.position;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void HandlePastaTimer()
    {
        if (isBrushing && tienePasta)
        {
            timerPasta -= Time.deltaTime;
            if (timerPasta <= 0)
            {
                tienePasta = false;
                if (spriteRenderer != null && spriteNormal != null)
                    spriteRenderer.sprite = spriteNormal; // Quita la pasta visualmente

                if (foamParticles != null && foamParticles.isPlaying) foamParticles.Stop();
            }
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

    // ==========================================
    // SISTEMA DE DETECCIÓN Y LIMPIEZA POR TRIGGERS (TU LOGICA BASE)
    // ==========================================
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Si el usuario no está arrastrando/haciendo clic, no limpia nada
        if (!isBrushing) return;

        // 1. LIMPIEZA DE DIENTES SUCIOS NORMALES (Tu código original intacto)
        ToothDirtMask dirtMask = collision.gameObject.GetComponent<ToothDirtMask>();
        if (dirtMask != null)
        {
            dirtMask.ClearTooth(); // Llama a tu función original que baja opacidad
            ReproducirEfectosSonidoYEspuma();
        }

        // 2. LIMPIEZA DE DIENTES CON SARRO ALEATORIO (Solo funciona si el cepillo TIENE PASTA)
        if (tienePasta)
        {
            ToothSarro sarro = collision.gameObject.GetComponent<ToothSarro>();
            if (sarro == null) sarro = collision.gameObject.GetComponentInChildren<ToothSarro>();

            if (sarro != null)
            {
                sarro.LimpiarSarro(velocidadBorradoSarro * Time.deltaTime);
                ReproducirEfectosSonidoYEspuma();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar algodón
        if (collision.gameObject.name.Contains("Algodon")) return;

        // Cargar Pasta Dental
        PastaDientesController pasta = collision.GetComponent<PastaDientesController>();
        if (pasta != null && pasta.destapada)
        {
            tienePasta = true;
            timerPasta = tiempoPasta;
            if (spriteRenderer != null && spriteConPasta != null)
                spriteRenderer.sprite = spriteConPasta; // Cambia el aspecto a cepillo con pasta
        }
    }

    private void ReproducirEfectosSonidoYEspuma()
    {
        if (foamParticles != null && !foamParticles.isPlaying)
            foamParticles.Play();

        if (audioSource != null && brushSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = brushSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
