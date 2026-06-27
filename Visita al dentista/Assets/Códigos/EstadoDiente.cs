using System;
using UnityEngine;

public enum TipoHerramienta
{
    CEPILLO,
    CEPILLO_CON_PASTA,
    ALGODON
}

public enum TipoEstado
{
    LIMPIO,
    SUCIO,
    SARRO,
    CARIE
}

public class EstadoDiente : MonoBehaviour
{
    [HideInInspector] public TipoEstado estadoActual;

    private SpriteRenderer srSucio;
    private SpriteRenderer srSarro;
    private SpriteRenderer srCarie;

    private float cooldownTimer;
    private const float COOLDOWN_TRAS_LIMPIEZA = 5f;

    private SpriteRenderer capaEnsuciandose;

    // --- SE SEPARA LA VELOCIDAD PARA CONTROLARLA POR ESTADO ---
    private const float VELOCIDAD_ENSUCIAMIENTO_NORMAL = 0.525f;
    // Modifica este 1.85f (casi el triple de rápido) si quieres que la carie brote aún más instantáneamente
    private const float VELOCIDAD_ENSUCIAMIENTO_CARIE = 1.85f;
    private float velocidadEnsuciamientoActual;

    public static event Action OnDienteLimpiado;
    public static event Action OnCualquierCambioDeEstado;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (capaEnsuciandose != null)
        {
            Color c = capaEnsuciandose.color;
            // Se aplica la velocidad dinámica dependiendo de qué estado se esté cocinando
            c.a += velocidadEnsuciamientoActual * Time.deltaTime;

            if (c.a >= 1f)
            {
                c.a = 1f;
                capaEnsuciandose.color = c;
                capaEnsuciandose = null;
            }
            else
            {
                capaEnsuciandose.color = c;
            }
        }
    }

    public void Inicializar(Sprite spriteSucio, Sprite spriteSarro, Sprite spriteCarie)
    {
        estadoActual = TipoEstado.LIMPIO;
        velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_NORMAL;

        srSucio = CrearCapa("Sucio", spriteSucio, 1);
        srSarro = CrearCapa("Sarro", spriteSarro, 2);
        srCarie = CrearCapa("Carie", spriteCarie, 3);

        SetCapa(srSucio, false);
        SetCapa(srSarro, false);
        SetCapa(srCarie, false);
    }

    private SpriteRenderer CrearCapa(string nombre, Sprite sprite, int orden)
    {
        if (sprite == null) return null;
        var go = new GameObject(nombre);
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = orden;
        return sr;
    }

    public void EnsuciarDiente()
    {
        if (cooldownTimer > 0f) return;
        if (capaEnsuciandose != null) return;

        SpriteRenderer nuevaCapa = null;
        float dadoProbabilidad = UnityEngine.Random.Range(0f, 100f);

        // Por defecto, la velocidad es la normal del juego
        velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_NORMAL;

        switch (estadoActual)
        {
            case TipoEstado.LIMPIO:
                if (dadoProbabilidad <= 25f)
                {
                    estadoActual = TipoEstado.SARRO;
                    nuevaCapa = srSarro;
                    if (srSucio != null) { srSucio.color = Color.white; srSucio.enabled = true; }
                }
                else
                {
                    estadoActual = TipoEstado.SUCIO;
                    nuevaCapa = srSucio;
                }
                break;

            case TipoEstado.SUCIO:
                if (dadoProbabilidad <= 60f)
                {
                    estadoActual = TipoEstado.CARIE;
                    nuevaCapa = srCarie;
                    // ¡ACELERACIÓN EXTREMA!: Si salta directo a carie, la opacidad subirá volando
                    velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_CARIE;

                    if (srSucio != null) { srSucio.color = Color.white; srSucio.enabled = true; }
                    if (srSarro != null) { srSarro.color = Color.white; srSarro.enabled = true; }
                }
                else
                {
                    estadoActual = TipoEstado.SARRO;
                    nuevaCapa = srSarro;
                }
                break;

            case TipoEstado.SARRO:
                // Si el diente ya tiene sarro, se transforma en CARIE obligatoriamente...
                estadoActual = TipoEstado.CARIE;
                nuevaCapa = srCarie;
                // ... ¡Y se activa la velocidad ultra rápida para que aparezca de inmediato!
                velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_CARIE;
                break;
        }

        if (nuevaCapa != null)
        {
            Color c = nuevaCapa.color;
            c.a = 0f;
            nuevaCapa.color = c;
            nuevaCapa.enabled = true;
            capaEnsuciandose = nuevaCapa;
        }

        OnCualquierCambioDeEstado?.Invoke();
    }

    public void LimpiarGradual(TipoHerramienta herramienta, float cantidad)
    {
        SpriteRenderer capaActiva = null;
        TipoEstado siguienteEstado = estadoActual;

        switch (estadoActual)
        {
            case TipoEstado.CARIE:
                if (herramienta == TipoHerramienta.ALGODON)
                {
                    capaActiva = srCarie;
                    siguienteEstado = TipoEstado.SARRO;
                }
                break;
            case TipoEstado.SARRO:
                if (herramienta == TipoHerramienta.CEPILLO_CON_PASTA)
                {
                    capaActiva = srSarro;
                    siguienteEstado = TipoEstado.SUCIO;
                }
                break;
            case TipoEstado.SUCIO:
                if (herramienta == TipoHerramienta.CEPILLO)
                {
                    capaActiva = srSucio;
                    siguienteEstado = TipoEstado.LIMPIO;
                }
                break;
        }

        // --- SOLUCIÓN DE EMERGENCIA PARA LOS DIENTES TRABADOS ---
        // Si la herramienta es la correcta para el Sarro, pero la capa 'srSarro' es null 
        // o falló en el Inspector, forzamos el cambio de estado manualmente.
        if (estadoActual == TipoEstado.SARRO && herramienta == TipoHerramienta.CEPILLO_CON_PASTA && capaActiva == null)
        {
            Debug.LogWarning("[Salvavidas] Forzando limpieza de sarro en " + gameObject.name);
            estadoActual = TipoEstado.SUCIO;
            cooldownTimer = COOLDOWN_TRAS_LIMPIEZA;
            OnCualquierCambioDeEstado?.Invoke();
            return;
        }

        // Destrabamos el estado de ensuciamiento si el niño está limpiando
        if (capaActiva != null && capaEnsuciandose != null)
        {
            capaEnsuciandose = null;
        }

        // Si no es el caso de emergencia y la herramienta sigue siendo incorrecta, salimos
        if (capaActiva == null)
        {
            Debug.Log("[EstadoDiente] " + gameObject.name + " herramienta incorrecta para estado " + estadoActual);
            return;
        }

        // Nos aseguramos de que la capa esté visible para que el niño vea el progreso
        capaActiva.enabled = true;

        Color c = capaActiva.color;
        c.a -= cantidad;
        capaActiva.color = c;

        Debug.Log("[EstadoDiente] " + gameObject.name + " alpha: " + c.a.ToString("F2") + " estado: " + estadoActual);

        // Transición normal cuando el alpha llega a cero
        if (c.a <= 0.05f)
        {
            c.a = 1f;
            capaActiva.color = c;
            capaActiva.enabled = false;

            estadoActual = siguienteEstado;
            cooldownTimer = COOLDOWN_TRAS_LIMPIEZA;

            Debug.Log("[EstadoDiente] " + gameObject.name + " TRANSICION a " + estadoActual + " | cooldown " + COOLDOWN_TRAS_LIMPIEZA + "s");

            if (estadoActual == TipoEstado.LIMPIO)
                OnDienteLimpiado?.Invoke();

            OnCualquierCambioDeEstado?.Invoke();
        }
    }

    private void SetCapa(SpriteRenderer sr, bool activo)
    {
        if (sr != null) sr.enabled = activo;
    }

    public bool EstaSiendoLimpiadoPor(TipoHerramienta herramienta)
    {
        if (estadoActual == TipoEstado.LIMPIO) return false;

        switch (estadoActual)
        {
            case TipoEstado.CARIE:
                return (herramienta == TipoHerramienta.ALGODON);

            case TipoEstado.SARRO:
                return (herramienta == TipoHerramienta.CEPILLO_CON_PASTA);

            case TipoEstado.SUCIO:
                return (herramienta == TipoHerramienta.CEPILLO);
        }

        return false;
    }
}