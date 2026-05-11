using System;
using UnityEngine;
using UnityEngine.UI;


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


    public void LimpiarDiente()
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