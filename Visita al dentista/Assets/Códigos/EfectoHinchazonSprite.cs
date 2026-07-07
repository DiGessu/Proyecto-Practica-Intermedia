using System.Collections;
using UnityEngine;

public class EfectoHinchazonPorSprite : MonoBehaviour
{
    [Header("Configuración de la Animación")]
    [Tooltip("Qué tan grande se estira el objeto (1.15 = 15% más grande).")]
    [SerializeField] private float escalaMaxima = 1.15f;
    [Tooltip("Duración total del movimiento de hinchazón.")]
    [SerializeField] private float duracionHinchazon = 0.4f;

    private Vector3 escalaOriginal;
    private bool estaHinchando = false;

    void Awake()
    {
        // Guardamos la escala original del objeto al iniciar el juego
        escalaOriginal = transform.localScale;
    }

    void OnEnable()
    {
        // ESCUCHA EXCLUSIVA: Nos desconectamos del evento viejo y ruidoso.
        // Ahora solo escuchamos cuando EstadoDiente confirma una CARIE real.
        EstadoDiente.OnAparicionDeCarie += IniciarAnimacionSegura;
    }

    void OnDisable()
    {
        // Nos desuscribimos de forma segura
        EstadoDiente.OnAparicionDeCarie -= IniciarAnimacionSegura;
    }

    private void IniciarAnimacionSegura()
    {
        // Si no se está ejecutando la animación actualmente, la iniciamos
        if (!estaHinchando)
        {
            StartCoroutine(AnimarHinchazonUnica());
        }
    }

    private IEnumerator AnimarHinchazonUnica()
    {
        estaHinchando = true;
        float tiempo = 0f;
        float mitadDuracion = duracionHinchazon / 2f;

        // 1. FASE CRECER: El objeto se infla un poco
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal, escalaOriginal * escalaMaxima, progreso);
            yield return null;
        }

        tiempo = 0f;

        // 2. FASE ACHICAR: Vuelve suavemente a su escala normal
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal * escalaMaxima, escalaOriginal, progreso);
            yield return null;
        }

        // Aseguramos que la escala quede exactamente en su tamaño de origen
        transform.localScale = escalaOriginal;
        estaHinchando = false;
    }
}