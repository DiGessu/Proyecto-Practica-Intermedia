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

    private const float VELOCIDAD_ENSUCIAMIENTO_NORMAL = 0.525f;
    private const float VELOCIDAD_ENSUCIAMIENTO_CARIE = 1.85f;
    private float velocidadEnsuciamientoActual;

    public static event Action OnDienteLimpiado;
    public static event Action OnCualquierCambioDeEstado;
    public static event Action OnAparicionDeCarie;

    // >>> CAMBIO AQUÍ: Una bombilla que se enciende cuando se decide una carie nueva <<<
    private bool debemostrarAnimacionDeCarieAlTerminar = false;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (capaEnsuciandose != null)
        {
            Color c = capaEnsuciandose.color;
            c.a += velocidadEnsuciamientoActual * Time.deltaTime;

            if (c.a >= 1f)
            {
                c.a = 1f;
                capaEnsuciandose.color = c;
                capaEnsuciandose = null;

                // >>> CAMBIO AQUÍ: La caries ya es 100% visible en pantalla. ¡Ahora sí, que se hinche la boca! <<<
                if (debemostrarAnimacionDeCarieAlTerminar)
                {
                    debemostrarAnimacionDeCarieAlTerminar = false; // Apagamos la bombilla
                    OnAparicionDeCarie?.Invoke(); // Mandamos el grito al EfectoHinchazon
                }
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

        TipoEstado estadoAnterior = estadoActual;
        SpriteRenderer nuevaCapa = null;
        float dadoProbabilidad = UnityEngine.Random.Range(0f, 100f);

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
                estadoActual = TipoEstado.CARIE;
                nuevaCapa = srCarie;
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

        // >>> CAMBIO AQUÍ: En lugar de gritar la animación de inmediato, encendemos la bombilla de aviso <<<
        if (estadoActual == TipoEstado.CARIE && estadoAnterior != TipoEstado.CARIE)
        {
            debemostrarAnimacionDeCarieAlTerminar = true;
        }
    }

    public void LimpiarGradual(TipoHerramienta herramienta, float cantidad)
    {
        SpriteRenderer capaActiva = null;
        TipoEstado siguienteEstado = estadoActual;

        // REGLA ESTRICTA: Emparejamos cada estado exclusivamente con su herramienta y su capa visual
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

        // SI LA HERRAMIENTA NO CORRESPONDE AL ESTADO VISUAL ACTUAL, SE CANCELA TODO
        if (capaActiva == null)
        {
            Debug.Log("[EstadoDiente] " + gameObject.name + " herramienta incorrecta (" + herramienta + ") para el estado real: " + estadoActual);
            return;
        }

        // Si veníamos de un proceso de ensuciamiento, lo detenemos por completo al empezar a limpiar
        capaEnsuciandose = null;
        capaActiva.enabled = true;

        // Restamos el alpha gradualmente
        Color c = capaActiva.color;
        c.a -= cantidad;
        capaActiva.color = c;

        Debug.Log("[EstadoDiente] " + gameObject.name + " limpiando capa: " + capaActiva.name + " | Alpha restante: " + c.a.ToString("F2"));

        // Cuando la mancha se elimina por completo (Alpha llega a 0)
        if (c.a <= 0.05f)
        {
            c.a = 1f;
            capaActiva.color = c;
            capaActiva.enabled = false; // Ocultamos por completo la capa que se limpió

            // Hacemos el cambio físico de estado
            estadoActual = siguienteEstado;
            cooldownTimer = COOLDOWN_TRAS_LIMPIEZA;

            Debug.Log("[EstadoDiente] " + gameObject.name + " ¡Capa limpiada con éxito! Nuevo estado: " + estadoActual);

            // Si el diente quedó reluciente, avisamos al contador general del juego
            if (estadoActual == TipoEstado.LIMPIO)
            {
                OnDienteLimpiado?.Invoke();
            }

            // Avisamos a la boca que hubo un cambio para actualizar la interfaz
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