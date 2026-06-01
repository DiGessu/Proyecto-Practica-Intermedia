using UnityEngine;

public class EspumaDental : MonoBehaviour
{
    [Header("Configuración de Limpieza Continua")]
    [Tooltip("Velocidad a la que se desvanece la espuma (ej: 0.5 vacía la opacidad en 2 segundos de contacto continuo)")]
    [SerializeField] private float velocidadLimpieza = 0.4f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Este método es llamado constantemente por el chorro de agua en cada frame de contacto
    public void DesgastarEspumaContinuo()
    {
        if (spriteRenderer != null)
        {
            Color colorActual = spriteRenderer.color;

            // Restamos opacidad de forma continua y suave según el tiempo transcurrido
            colorActual.a -= velocidadLimpieza * Time.deltaTime;

            // Nos aseguramos de que no baje de 0
            colorActual.a = Mathf.Max(colorActual.a, 0);
            spriteRenderer.color = colorActual;

            // Si llegó a ser invisible por completo, se elimina el objeto de la escena
            if (spriteRenderer.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}