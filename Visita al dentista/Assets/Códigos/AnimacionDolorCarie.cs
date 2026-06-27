using UnityEngine;

public class AnimacionDolorCarie : MonoBehaviour
{
    [Header("Configuración de la Hinchazón")]
    [Tooltip("Qué tan rápido late o pulsa la carie.")]
    [SerializeField] private float velocidadLatido = 6f;

    [Tooltip("Qué tan grande es el movimiento (0.04 es muy sutil).")]
    [SerializeField] private float intensidadHinchazon = 0.04f;

    [Tooltip("Duración de la animación en segundos antes de detenerse.")]
    [SerializeField] private float duracionAnimacion = 1f;

    private Vector3 escalaInicial;
    private float tiempoAleatorio;
    private float tiempoInicioActivo;
    private SpriteRenderer spriteRenderer;
    private bool yaSeRegistroTiempo;

    void Start()
    {
        escalaInicial = transform.localScale;
        tiempoAleatorio = Random.Range(0f, 100f);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer != null && spriteRenderer.enabled)
        {
            // En el primer frame que la carie se vuelve visible, guardamos el tiempo actual
            if (!yaSeRegistroTiempo)
            {
                tiempoInicioActivo = Time.time;
                yaSeRegistroTiempo = true;
            }

            // Calculamos cuánto tiempo ha pasado desde que se activó
            float tiempoTranscurrido = Time.time - tiempoInicioActivo;

            // Si aún no pasa el segundo (o el tiempo configurado), se ejecuta la animación
            if (tiempoTranscurrido < duracionAnimacion)
            {
                float factorMovimiento = Mathf.Sin(Time.time * velocidadLatido + tiempoAleatorio) * intensidadHinchazon;
                transform.localScale = escalaInicial + new Vector3(factorMovimiento, factorMovimiento, 0f);
            }
            else
            {
                // Si ya pasó el segundo, nos aseguramos de regresar el objeto exactamente a su escala original
                if (transform.localScale != escalaInicial)
                {
                    transform.localScale = escalaInicial;
                }
            }
        }
        else
        {
            // Si el SpriteRenderer se apaga (porque el algodón limpió la carie), 
            // reiniciamos el flag para que vuelva a animarse la próxima vez que aparezca
            yaSeRegistroTiempo = false;
        }
    }

    void OnDisable()
    {
        // Red de seguridad al desactivar el componente completo
        transform.localScale = escalaInicial;
        yaSeRegistroTiempo = false;
    }
}