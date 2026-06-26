using UnityEngine;

public class CottonTool : MonoBehaviour
{
    [Header("Configuración del Algodón")]
    [Range(5f, 30f)] public float followSpeed = 15f;
    public Vector2 toolOffset = new Vector2(0f, 0.5f);

    [Header("Limpieza")]
    [Range(0.01f, 0.1f)] public float velocidadLimpieza = 0.02f;

    private Camera mainCamera;
    private AudioSource miAudioSource;
    private bool isCleaning = false;
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
        isCleaning = false;
        ApagarSonido();
    }

    void Update()
    {
        if (isDragging)
        {
            isCleaning = Input.GetMouseButton(0);

            Vector2 targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPos += toolOffset;
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref currentVelocity, 1f / followSpeed);

            if (isCleaning && estaTocandoDiente)
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

        // BLINDAJE: Si no es un diente, nos salimos
        if (diente == null) return;

        if (isCleaning && isDragging)
        {
            // 1. Intentamos limpiar el diente (baja opacidad de la caries si existe)
            diente.LimpiarGradual(global::TipoHerramienta.ALGODON, velocidadLimpieza);

            // 2. LA MAGIA: Solo soltamos espuma si el diente realmente necesita el ALGODON (tiene caries)
            if (diente.EstaSiendoLimpiadoPor(global::TipoHerramienta.ALGODON))
            {
                Vector3 puntoContacto = collision.ClosestPoint(transform.position);
                GeneradorEspumaAlgodon generador = FindFirstObjectByType<GeneradorEspumaAlgodon>();

                if (generador != null)
                {
                    generador.SoltarEspumaNueva(puntoContacto);
                }
            }
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