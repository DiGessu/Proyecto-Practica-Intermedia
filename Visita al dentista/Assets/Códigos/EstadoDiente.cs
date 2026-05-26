using System;
using UnityEngine;

public enum TipoHerramienta
{
    CEPILLO,
    CEPILLO_CON_PASTA,
    ALGODON,
    NINGUNA
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
    private const float VELOCIDAD_ENSUCIAMIENTO = 0.35f;

    public static event Action OnDienteLimpiado;
    public static event Action OnCualquierCambioDeEstado;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (capaEnsuciandose != null)
        {
            Color c = capaEnsuciandose.color;
            c.a += VELOCIDAD_ENSUCIAMIENTO * Time.deltaTime;
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

    public void IntentarLimpiar(TipoHerramienta herramientaUsada)
    {
        bool limpiezaExitosa = false;

        switch (estadoActual)
        {
            case TipoEstado.CARIE:
                if (herramientaUsada == TipoHerramienta.ALGODON)
                {
                    estadoActual = TipoEstado.SARRO;
                    limpiezaExitosa = true;
                }
                break;

            case TipoEstado.SARRO:
                if (herramientaUsada == TipoHerramienta.CEPILLO_CON_PASTA)
                {
                    estadoActual = TipoEstado.SUCIO;
                    limpiezaExitosa = true;
                }
                break;

            case TipoEstado.SUCIO:
                if (herramientaUsada == TipoHerramienta.CEPILLO)
                {
                    estadoActual = TipoEstado.LIMPIO;
                    limpiezaExitosa = true;
                    OnDienteLimpiado?.Invoke();
                }
                break;
        }

        if (limpiezaExitosa)
        {
            ActualizarEstadoVisual();
            OnCualquierCambioDeEstado?.Invoke();
        }
    }

    public void EnsuciarDiente()
    {
        if (cooldownTimer > 0f) return;
        if (capaEnsuciandose != null) return;

        SpriteRenderer nuevaCapa = null;

        switch (estadoActual)
        {
            case TipoEstado.LIMPIO:
                estadoActual = TipoEstado.SUCIO;
                nuevaCapa = srSucio;
                break;
            case TipoEstado.SUCIO:
                estadoActual = TipoEstado.SARRO;
                nuevaCapa = srSarro;
                break;
            case TipoEstado.SARRO:
                estadoActual = TipoEstado.CARIE;
                nuevaCapa = srCarie;
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

    private void ActualizarEstadoVisual()
    {
        switch (estadoActual)
        {
            case TipoEstado.LIMPIO:
                SetCapa(srSucio, false);
                SetCapa(srSarro, false);
                SetCapa(srCarie, false);
                break;
            case TipoEstado.SUCIO:
                SetCapa(srSucio, true);
                SetCapa(srSarro, false);
                SetCapa(srCarie, false);
                break;
            case TipoEstado.SARRO:
                SetCapa(srSucio, true);
                SetCapa(srSarro, true);
                SetCapa(srCarie, false);
                break;
            case TipoEstado.CARIE:
                SetCapa(srSucio, true);
                SetCapa(srSarro, true);
                SetCapa(srCarie, true);
                break;
        }
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

        if (capaActiva == null)
        {
            Debug.Log("[EstadoDiente] " + gameObject.name + " herramienta incorrecta para estado " + estadoActual);
            return;
        }

        if (capaEnsuciandose == capaActiva)
            capaEnsuciandose = null;

        Color c = capaActiva.color;
        c.a -= cantidad;
        capaActiva.color = c;

        Debug.Log("[EstadoDiente] " + gameObject.name + " alpha: " + c.a.ToString("F2") + " estado: " + estadoActual);

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
}