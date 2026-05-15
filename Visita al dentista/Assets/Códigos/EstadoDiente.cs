using System;
using UnityEngine;
using UnityEngine.UI;


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


[RequireComponent(typeof(SpriteRenderer))]
public class EstadoDiente : MonoBehaviour
{
    [Header("Configuración de Estado")]
    public TipoEstado estadoActual;
    public Image imagenDiente;

    [Header("Sprites por Estado")]
    public Sprite spriteLimpio;
    public Sprite spriteSucio;
    public Sprite spriteSarro;
    public Sprite spriteCarie;


    public static event Action OnDienteLimpiado;


    public void IntentarLimpiar(TipoHerramienta herramientaUsada)
    {
        bool limpiezaExitosa = false;

        switch (estadoActual)
        {
            case TipoEstado.SUCIO:
                if (herramientaUsada == TipoHerramienta.CEPILLO) limpiezaExitosa = true;
                break;

            case TipoEstado.SARRO:
                if (herramientaUsada == TipoHerramienta.CEPILLO_CON_PASTA) limpiezaExitosa = true;
                break;

            case TipoEstado.CARIE:
                if (herramientaUsada == TipoHerramienta.ALGODON) limpiezaExitosa = true;
                break;
        }

        if (limpiezaExitosa)
        {
            EjecutarLimpieza();
        }
        else
        {
            Debug.Log("Herramienta incorrecta para este estado.");
        }
    }

    private void EjecutarLimpieza()
    {
        estadoActual = TipoEstado.LIMPIO;
        imagenDiente.sprite = spriteLimpio;
        OnDienteLimpiado?.Invoke();
    }

    public void EnsuciarDiente(TipoEstado newTipo)
    {
        switch (estadoActual)
        {
            case TipoEstado.LIMPIO: estadoActual = TipoEstado.SUCIO; imagenDiente.sprite = spriteSucio; break;
            case TipoEstado.SUCIO: estadoActual = TipoEstado.SARRO; imagenDiente.sprite = spriteSarro; break;
            case TipoEstado.SARRO: estadoActual = TipoEstado.CARIE; imagenDiente.sprite = spriteCarie; break;
        }
    }

    private void Awake()
    {
        if (imagenDiente == null) imagenDiente = GetComponent<Image>();
    }

    private void Start()
    {
        imagenDiente.sprite = spriteLimpio;
    }
}