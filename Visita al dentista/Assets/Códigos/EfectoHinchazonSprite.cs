using System.Collections;
using UnityEngine;

public class EfectoHinchazonPorSprite : MonoBehaviour
{
    [Header("Configuración del Diente Asociado")]
    [Tooltip("Arrastra aquí el SpriteRenderer del diente que quieres escuchar.")]
    [SerializeField] private SpriteRenderer dienteAsociado;

    [Header("Configuración de la Animación")]
    [Tooltip("Qué tan grande se estira el objeto (1.2 = 20% más grande).")]
    [SerializeField] private float escalaMaxima = 1.15f;
    [Tooltip("Duración total del movimiento de hinchazón.")]
    [SerializeField] private float duracionHinchazon = 0.4f;

    private EstadoDiente scriptEstadoDiente;
    private Vector3 escalaOriginal;
    private bool estaHinchando = false;
    private TipoEstado ultimoEstadoConocido;

    void Awake()
    {
        // Guardamos la escala original del objeto donde pongas ESTE script (ej: la cara o el cachete)
        escalaOriginal = transform.localScale;

        ObtenerScriptEstado();
    }

    void OnEnable()
    {
        // Nos suscribimos al evento global
        EstadoDiente.OnCualquierCambioDeEstado += VerificarCambioDeEstado;

        if (scriptEstadoDiente != null)
        {
            ultimoEstadoConocido = scriptEstadoDiente.estadoActual;
        }
    }

    void OnDisable()
    {
        EstadoDiente.OnCualquierCambioDeEstado -= VerificarCambioDeEstado;
    }

    private void ObtenerScriptEstado()
    {
        if (dienteAsociado != null)
        {
            // Buscamos el script EstadoDiente dentro del SpriteRenderer que arrastraste
            scriptEstadoDiente = dienteAsociado.GetComponent<EstadoDiente>();
        }
    }

    private void VerificarCambioDeEstado()
    {
        // Si el usuario cambió el diente en el inspector en caliente, volvemos a buscar el script
        if (scriptEstadoDiente == null && dienteAsociado != null)
        {
            ObtenerScriptEstado();
        }

        if (scriptEstadoDiente == null) return;

        // "¡Oye! El diente asociado cambió a CARIE, ¡muévete!"
        if (scriptEstadoDiente.estadoActual == TipoEstado.CARIE && ultimoEstadoConocido != TipoEstado.CARIE)
        {
            if (!estaHinchando)
            {
                StartCoroutine(AnimarHinchazonUnica());
            }
        }

        ultimoEstadoConocido = scriptEstadoDiente.estadoActual;
    }

    private IEnumerator AnimarHinchazonUnica()
    {
        estaHinchando = true;
        float tiempo = 0f;
        float mitadDuracion = duracionHinchazon / 2f;

        // 1. FASE CRECER: El objeto donde está este script se infla un poco
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal, escalaOriginal * escalaMaxima, progreso);
            yield return null;
        }

        tiempo = 0f;

        // 2. FASE ACHICAR: Vuelve a su tamaño normal
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal * escalaMaxima, escalaOriginal, progreso);
            yield return null;
        }

        // Nos aseguramos de dejarlo exacto
        transform.localScale = escalaOriginal;
        estaHinchando = false;
    }
}