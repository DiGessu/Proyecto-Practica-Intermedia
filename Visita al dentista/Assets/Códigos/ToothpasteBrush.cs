using UnityEngine;

public class ToothpasteBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)] public float followSpeed = 15f;
    public Vector2 brushOffset = new Vector2(0f, 0.5f);

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)] public float velocidadLimpieza = 0.02f;

    private Camera mainCamera;
    private AudioSource miAudioSource;
    private bool isBrushing = false;
    private bool isDragging = false;
    private Vector2 currentVelocity;
    private bool estaTocandoDiente = false;

    void Awake()
    {
        mainCamera = Camera.main;
        miAudioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        isBrushing = false;
        ApagarSonido();
    }

    void Update()
    {
        if (isDragging)
        {
            isBrushing = Input.GetMouseButton(0);

            Vector2 targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPos += brushOffset;
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref currentVelocity, 1f / followSpeed);

            if (isBrushing && estaTocandoDiente)
            {
                EncenderSonido();
            }
            else
            {
                ApagarSonido();
            }
        }
    }

    private void EncenderSonido()
    {
        if (miAudioSource != null && !miAudioSource.isPlaying)
        {
            miAudioSource.Play();
        }
    }

    private void ApagarSonido()
    {
        if (miAudioSource != null && miAudioSource.isPlaying)
        {
            miAudioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EstadoDiente>() != null)
        {
            estaTocandoDiente = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EstadoDiente diente = collision.GetComponent<EstadoDiente>();

        // BLINDAJE
        if (diente == null) return;

        if (isBrushing && isDragging)
        {
            if (diente.EstaSiendoLimpiadoPor(global::TipoHerramienta.CEPILLO_CON_PASTA))
            {
                Vector3 puntoContacto = collision.ClosestPoint(transform.position);
                FindFirstObjectByType<GeneradorEspuma>()?.SoltarEspuma(puntoContacto);
            }

            diente.LimpiarGradual(global::TipoHerramienta.CEPILLO_CON_PASTA, velocidadLimpieza);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EstadoDiente>() != null)
        {
            estaTocandoDiente = false;
            ApagarSonido();
        }
    }
}