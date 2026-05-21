using UnityEngine;

public class PastaController : MonoBehaviour
{
    [Header("Visualización del Cepillo (Un solo objeto)")]
    public Sprite spriteNormal;
    public Sprite spriteConPasta;
    private SpriteRenderer spriteRenderer;

    [Header("Mecánica Pasta")]
    public bool tienePasta = false;
    public float tiempoPasta = 6f;
    private float timer;
    private bool estaArrastrando = false;

    // Guardará la referencia del sarro que estemos tocando en tiempo real
    private ToothSarroMask sarroActual;

    [Header("Efectos")]
    public ParticleSystem foamParticles;
    public AudioClip brushSound;
    private AudioSource audioSource;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && brushSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        timer = tiempoPasta;
    }

    void Update()
    {
        if (estaArrastrando && tienePasta)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                tienePasta = false;
                spriteRenderer.sprite = spriteNormal; // Cambia la visual al cepillo limpio
                DetenerEfectos();
            }
        }
    }

    private void OnMouseDrag()
    {
        estaArrastrando = true;

        // Seguir al puntero
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        // Si no tiene pasta, no limpia absolutamente nada
        if (!tienePasta)
        {
            if (foamParticles != null && foamParticles.isPlaying) foamParticles.Stop();
            return;
        }

        // Si el BoxCollider2D está sobre un objeto con máscara de sarro
        if (sarroActual != null)
        {
            ReproducirEfectos();
            // Borra píxel a píxel usando la RenderTexture del sarro
            sarroActual.EraseAt((Vector2)transform.position);
        }
    }

    private void OnMouseUp()
    {
        estaArrastrando = false;
        DetenerEfectos();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitar conflictos con algodones
        if (collision.gameObject.name.Contains("Algodon")) return;

        // Detectar la recarga de pasta dental
        PastaDientesController pasta = collision.GetComponent<PastaDientesController>();
        if (pasta != null && pasta.destapada)
        {
            tienePasta = true;
            timer = tiempoPasta;
            spriteRenderer.sprite = spriteConPasta; // Cambia la visual a cepillo con pasta
            return;
        }

        // Al entrar en el trigger, buscamos EXCLUSIVAMENTE la máscara de sarro
        ToothSarroMask sarro = collision.GetComponent<ToothSarroMask>();
        if (sarro == null)
        {
            // Si está en el objeto padre o un objeto vecino, revisa sus hijos
            sarro = collision.GetComponentInChildren<ToothSarroMask>();
        }

        if (sarro != null)
        {
            sarroActual = sarro;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si nos alejamos del objeto, limpiamos la referencia
        if (collision.GetComponent<ToothSarroMask>() != null || (sarroActual != null && collision.gameObject == sarroActual.gameObject))
        {
            sarroActual = null;
            DetenerEfectos();
        }
    }

    private void ReproducirEfectos()
    {
        if (foamParticles != null && !foamParticles.isPlaying) foamParticles.Play();
        if (audioSource != null && brushSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = brushSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void DetenerEfectos()
    {
        if (foamParticles != null && foamParticles.isPlaying) foamParticles.Stop();
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }
}