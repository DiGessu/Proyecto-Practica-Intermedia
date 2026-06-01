using UnityEngine;

public class ChorroAgua : MonoBehaviour
{
    [Header("Configuración de Desvanecimiento del Agua")]
    [SerializeField] private float velocidadDesvanecer = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    void OnEnable()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }
    }

    void Update()
    {
        // El agua se desvanece sola con el tiempo
        if (spriteRenderer != null && spriteRenderer.color.a > 0)
        {
            Color colorActual = spriteRenderer.color;
            colorActual.a -= velocidadDesvanecer * Time.deltaTime;
            spriteRenderer.color = colorActual;

            if (spriteRenderer.color.a <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // STAY: Se ejecuta continuamente en cada frame mientras el agua toque la espuma
    void OnTriggerStay2D(Collider2D collision)
    {
        EspumaDental espuma = collision.GetComponent<EspumaDental>();

        if (espuma != null)
        {
            // Le dice a la espuma que se gaste de forma continua
            espuma.DesgastarEspumaContinuo();
        }
    }
}